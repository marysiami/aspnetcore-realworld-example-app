using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Features.Surveys
{
    [Route("surveys")]
    public class SurveysController : Controller
    {
        private readonly IMediator _mediator;

        public SurveysController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        //[Authorize(AuthenticationSchemes = JwtIssuerOptions.Schemes)]
        public async Task<SurveysEnvelope> Create([FromBody] Create.Command command)
        {
            return await _mediator.Send(command);
        }

        [HttpGet("getAllSurveys")]
        public async Task<SurveysEnvelope> GetAllSurveys()
        {
            return await _mediator.Send(new AllSurveys.Query());
        }

        [HttpGet("getSurvey/{id}")]
        public async Task<SurveysEnvelope> GetSurveyById(int id)
        {
            return await _mediator.Send(new Details.Query(id));
        }

        [HttpGet("getSurveys/{username}")]
        public async Task<SurveysEnvelope> GetSurveyByUsername(string username)
        {
            return await _mediator.Send(new AllSurveysByUsername.Query(username));
        }

        [HttpDelete("removeSurvey/{id}")]
        //[Authorize(AuthenticationSchemes = JwtIssuerOptions.Schemes)]
        public async Task RemoveAt(int id)
        {
            await _mediator.Send(new RemoveAt.Command(id));
        }
    }
}
