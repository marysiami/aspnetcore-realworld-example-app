using Conduit.Infrastructure;
using Conduit.Infrastructure.Errors;
using FluentValidation;
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
    public class ConfirmAccount
    {
        public class Query : IRequest
        {
            public Query(string email, string code)
            {
                Email = email;
                Code = code;
            }

            public string Email { get; }
            public string Code { get; }
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

                var accountFromList = ConfirmAccountData.ConfirmAccountListData
                    .Where(x => x.Email == message.Email && x.Code == message.Code)
                    .FirstOrDefault();

                if (accountFromList == null)
                {
                    throw new RestException(HttpStatusCode.NotFound);
                }

                person.IsConfirmed = true;

                _context.Persons.Update(person);
                await _context.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }
    }
}
