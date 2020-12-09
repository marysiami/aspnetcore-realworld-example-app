using Conduit.Infrastructure;
using Conduit.Infrastructure.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace Conduit.Features.Users
{
    public class RemindPassword
    {
        public class Query : IRequest
        {
            public Query(string email)
            {
                Email = email;
            }

            public string Email { get; }
        }

        public class QueryHandler : IRequestHandler<Query>
        {
            private readonly ConduitContext _context;

            public QueryHandler(ConduitContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(Query message, CancellationToken cancellationToken)
            {
                var person = await _context.Persons.Where(x => x.Email == message.Email).FirstOrDefaultAsync();

                if (person == null)
                {
                    throw new RestException(HttpStatusCode.NotFound, new { Article = Constants.NOT_FOUND });
                }

                SendMail(message);
                return Unit.Value;
            }

            private void SendMail(Query message)
            {
                var fromAddress = new MailAddress("conduitmailzio@gmail.com");
                var toAddress = new MailAddress(message.Email);
                const string fromPassword = "!@#qweASDzxc";
                const string subject = "Zmiana hasła";
                string code = Guid.NewGuid().ToString();
                string body = $"Link do zmiany hasła : http://localhost:4100/codeToChangePassword/{message.Email}/{code}";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword),
                    Timeout = 20000
                };
                using (var emailMessage = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(emailMessage);
                    RemindPasswordData.RemindPasswordListData.Add(new RemindPasswordData.RemindPasswordDataModel() { Email = message.Email, Code = code });
                }
            }
        }
    }
}
