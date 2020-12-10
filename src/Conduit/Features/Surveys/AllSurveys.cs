using Conduit.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Conduit.Features.Surveys
{
    public class AllSurveys
    {
        public class Query : IRequest<SurveysEnvelope>
        {
            public Query()
            {

            }
        }

        public class QueryHandler : IRequestHandler<Query, SurveysEnvelope>
        {
            private readonly ConduitContext _context;
            private readonly ICurrentUserAccessor _currentUserAccessor;

            public QueryHandler(ConduitContext context, ICurrentUserAccessor currentUserAccessor)
            {
                _context = context;
                _currentUserAccessor = currentUserAccessor;
            }

            public async Task<SurveysEnvelope> Handle(Query message, CancellationToken cancellationToken)
            {
                var surveys = await _context.Surveys
                    .Include(x => x.AnswersAndQuestions)
                    .Include(x => x.Author)
                    .Include(x => x.FillingUser)
                    .AsNoTracking()
                    .ToListAsync();

                return new SurveysEnvelope()
                {
                    Surveys = surveys,
                    SurveysCount = surveys.Count()
                };
            }
        }
    }
}
