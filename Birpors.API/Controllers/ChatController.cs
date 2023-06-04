using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Birpors.API.Filters;
using Birpors.Application.CommonModels;
using Birpors.Application.CQRS.Chat.Queries.GetConversation;
using Birpors.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Birpors.API.Controllers
{
    [Route("api/chat")]
    public class ChatController : BaseController
    {

        [HttpGet("conversations")]
        [Auth(UserRoleEnum.All)]
        public async Task<ActionResult<ApiResult<GetConversationQueryResponse>>> GetConversation()
        {
            var userClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "userId");
            var userId = userClaim != null ? Convert.ToInt32(userClaim.Value) : 0;
            return await Mediator.Send(new GetConversationQuery(userId));
        }

    }
}

