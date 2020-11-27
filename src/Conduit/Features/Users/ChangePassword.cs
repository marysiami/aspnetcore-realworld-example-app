using Conduit.Infrastructure;
using Conduit.Infrastructure.Errors;
using Conduit.Infrastructure.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Conduit.Features.Users
{
    public class ChangePassword
    {
        public class Query : IRequest
        {
            public Query(string email, string newPassword)
            {
                NewPassword = newPassword;
                Email = email;
            }

            public string NewPassword { get; }
            public string Email { get; }
        }

        public class QueryHandler : IRequestHandler<Query>
        {
            private readonly ConduitContext _context;
            private readonly IPasswordHasher _passwordHasher;

            public QueryHandler(ConduitContext context, IPasswordHasher passwordHasher)
            {
                _context = context;
                _passwordHasher = passwordHasher;
            }

            public async Task<Unit> Handle(Query message, CancellationToken cancellationToken)
            {
                var person = await _context.Persons.Where(x => x.Email == message.Email).FirstOrDefaultAsync();

                if (person == null)
                {
                    throw new RestException(HttpStatusCode.NotFound);
                }

                var salt = Guid.NewGuid().ToByteArray();
                person.Hash = _passwordHasher.Hash(message.NewPassword, salt);
                person.Salt = salt;

                _context.Persons.Update(person);
                await _context.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }
    }
}
