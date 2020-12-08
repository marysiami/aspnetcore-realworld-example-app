using Conduit.Infrastructure;
using Conduit.Infrastructure.Errors;
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
    public class BanUser
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
                var person = await _context.Persons
                    .Where(x => x.Email == message.Email)
                    .FirstOrDefaultAsync();

                if (person == null)
                {
                    throw new RestException(HttpStatusCode.NotFound);
                }

                person.IsBanned = true;

                _context.Persons.Update(person);
                //await _context.SaveChangesAsync(cancellationToken);

                var articles = await _context.Articles
                    .Where(x => x.Author.Email == message.Email)
                    .ToListAsync();

                if (articles == null || articles.Count == 0)
                {
                    return Unit.Value;
                }
                else
                {
                    foreach (var article in articles)
                    {
                        article.IsBanned = true;

                        _context.Articles.Update(article);
                        //await _context.SaveChangesAsync(cancellationToken);
                    }
                }

                var comments = await _context.Comments
                    .Where(x => x.Author.Email == message.Email)
                    .ToListAsync();

                if (comments == null || comments.Count == 0)
                {
                    return Unit.Value;
                }
                else
                {
                    foreach (var comment in comments)
                    {
                        comment.IsBanned = true;
                        _context.Comments.Update(comment);
                    }
                }

                await _context.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }
    }
}
