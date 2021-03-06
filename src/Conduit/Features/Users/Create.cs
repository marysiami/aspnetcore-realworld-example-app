﻿using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Conduit.Domain;
using Conduit.Infrastructure;
using Conduit.Infrastructure.Errors;
using Conduit.Infrastructure.Security;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Conduit.Features.Users
{
    public class Create
    {
        public class UserData
        {
            public string Username { get; set; }

            public string Email { get; set; }

            public string Password { get; set; }
        }

        public class UserDataValidator : AbstractValidator<UserData>
        {
            public UserDataValidator()
            {
                RuleFor(x => x.Username).NotNull().NotEmpty();
                RuleFor(x => x.Email).NotNull().NotEmpty();
                RuleFor(x => x.Password).NotNull().NotEmpty();
            }
        }

        public class Command : IRequest<UserEnvelope>
        {
            public UserData User { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.User).NotNull().SetValidator(new UserDataValidator());
            }
        }

        public class Handler : IRequestHandler<Command, UserEnvelope>
        {
            private readonly ConduitContext _context;
            private readonly IPasswordHasher _passwordHasher;
            private readonly IJwtTokenGenerator _jwtTokenGenerator;
            private readonly IMapper _mapper;

            public Handler(ConduitContext context, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator, IMapper mapper)
            {
                _context = context;
                _passwordHasher = passwordHasher;
                _jwtTokenGenerator = jwtTokenGenerator;
                _mapper = mapper;
            }

            public async Task<UserEnvelope> Handle(Command message, CancellationToken cancellationToken)
            {
                if (await _context.Persons.Where(x => x.Username == message.User.Username).AnyAsync(cancellationToken))
                {
                    throw new RestException(HttpStatusCode.BadRequest, new { Username = Constants.IN_USE });
                }

                if (await _context.Persons.Where(x => x.Email == message.User.Email).AnyAsync(cancellationToken))
                {
                    throw new RestException(HttpStatusCode.BadRequest, new { Email = Constants.IN_USE });
                }

                var salt = Guid.NewGuid().ToByteArray();
                var person = new Person
                {
                    Username = message.User.Username,
                    Email = message.User.Email,
                    Hash = _passwordHasher.Hash(message.User.Password, salt),
                    Salt = salt,
                    IsAdmin = message.User.Username.ToUpper().StartsWith("ADMIN"),
                    IsConfirmed = message.User.Username.ToUpper().StartsWith("ADMIN")
                };

                SendMail(person);

                _context.Persons.Add(person);
                await _context.SaveChangesAsync(cancellationToken);
                var user = _mapper.Map<Person, User>(person);
                user.Token = await _jwtTokenGenerator.CreateToken(person.Username);
                return new UserEnvelope(user);
            }

            private void SendMail(Person person)
            {
                //asdkaddsaadsads@outlook.com
                //cxjasdZXCAS@23
                var fromAddress = new MailAddress("asdkaddsaadsads@outlook.com");
                var toAddress = new MailAddress(person.Email);
                const string fromPassword = "cxjasdZXCAS@23";
                const string subject = "Potwierdzenie konta";
                string code = Guid.NewGuid().ToString();
                //string body = $"Twój kod do aktywacji konta: {code}";
                string body = $"Twoj link aktywacyjny: https://pbwi.herokuapp.com/activateAccount/{person.Email}/{code}";

                var smtp = new SmtpClient
                {
                    Host = "SMTP.office365.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword),
                    Timeout = 20000
                };
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);
                    ConfirmAccountData.ConfirmAccountListData.Add(new ConfirmAccountData.ConfirmAccountDataModel() { Email = person.Email, Code = code });
                }
            }
        }
    }
}
