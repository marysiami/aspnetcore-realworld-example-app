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

namespace Conduit.Features.Articles
{
    public class AllReportedArticles
    {
        public class Query : IRequest<ArticlesEnvelope>
        {
            public Query()
            {

            }
        }

        public class QueryHandler : IRequestHandler<Query, ArticlesEnvelope>
        {
            private readonly ConduitContext _context;

            public QueryHandler(ConduitContext context)
            {
                _context = context;
            }

            public async Task<ArticlesEnvelope> Handle(Query message, CancellationToken cancellationToken)
            {
                var reportedArticles = await _context.Articles
                    .Where(x => x.IsReported == true)
                    .AsNoTracking()
                    .ToListAsync();

                if (reportedArticles == null || reportedArticles.Count == 0)
                {
                    throw new RestException(HttpStatusCode.NotFound, new { Article = Constants.NOT_FOUND });
                }

                return new ArticlesEnvelope()
                {
                    Articles = reportedArticles,
                    ArticlesCount = reportedArticles.Count()
                };
            }
        }
    }
}
