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
    public class SendCode
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

                var fromAddress = new MailAddress("conduitmailzio@gmail.com");
                var toAddress = new MailAddress(message.Email);
                const string fromPassword = "!@#qweASDzxc";
                const string subject = "Potwierdzenie konta";
                var existingCode = ConfirmAccountData.ConfirmAccountListData.Where(x => x.Email == message.Email).Select(x => x.Code).FirstOrDefault();
                string code = string.IsNullOrEmpty(existingCode) ? Guid.NewGuid().ToString() : existingCode;
                string body = $"Twoj link aktywacyjny: https://pbwi.herokuapp.com/activateAccount/{person.Email}/{code}";

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

                    if (string.IsNullOrEmpty(existingCode))
                    {
                        ConfirmAccountData.ConfirmAccountListData.Add(new ConfirmAccountData.ConfirmAccountDataModel() { Email = message.Email, Code = code });
                    }
                }

                return Unit.Value;
            }
        }
    }
}
