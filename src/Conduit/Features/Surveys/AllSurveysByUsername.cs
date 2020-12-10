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

namespace Conduit.Features.Surveys
{
    public class AllSurveysByUsername
    {
        public class Query : IRequest<SurveysEnvelope>
        {
            public Query(string username)
            {
                Username = username;
            }

            public string Username { get; set; }
        }

        public class QueryHandler : IRequestHandler<Query, SurveysEnvelope>
        {
            private readonly ConduitContext _context;

            public QueryHandler(ConduitContext context)
            {
                _context = context;
            }

            public async Task<SurveysEnvelope> Handle(Query message, CancellationToken cancellationToken)
            {
                var surveys = await _context.Surveys
                    .Where(x => x.Author.Username == message.Username)
                    .Include(x => x.AnswersAndQuestions)
                    .Include(x => x.Author)
                    .Include(x => x.FillingUser)
                    .AsNoTracking()
                    .ToListAsync();

                if (surveys == null || surveys.Count == 0)
                {
                    throw new RestException(HttpStatusCode.NotFound, new { Article = Constants.NOT_FOUND });
                }

                return new SurveysEnvelope()
                {
                    Surveys = surveys,
                    SurveysCount = surveys.Count()
                };
            }
        }
    }
}
