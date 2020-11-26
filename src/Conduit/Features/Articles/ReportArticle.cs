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

namespace Conduit.Features.Articles
{
    public class ReportArticle
    {
        public class Query : IRequest
        {
            public Query(int id, bool isReported)
            {
                Id = id;
                IsReported = isReported;
            }

            public int Id { get; }
            public bool IsReported { get; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(x => x.Id).NotNull().NotEmpty();
            }
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
                var artickle = await _context.Articles.Where(x => x.ArticleId == message.Id).FirstOrDefaultAsync();

                if (artickle == null)
                {
                    throw new RestException(HttpStatusCode.NotFound, new { Article = Constants.NOT_FOUND });
                }

                artickle.IsReported = message.IsReported;

                _context.Articles.Update(artickle);
                await _context.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }
    }
}
