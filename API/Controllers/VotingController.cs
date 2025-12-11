using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using votesystembackend.Application.DTOs;
using votesystembackend.Application.Interfaces;
using votesystembackend.API.Responses;
using votesystembackend.Application.Responses;

namespace votesystembackend.API.Controllers
{
    [ApiController]
    [Route("api/voting")]
    public class VotingController : ControllerBase
    {
        private readonly IVoteService _service;

        public VotingController(IVoteService service)
        {
            _service = service;
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> Create(CreateVoteSessionRequest req)
        {
            var userId = Guid.Parse(User.FindFirst("id")?.Value ?? Guid.Empty.ToString());
            var res = await _service.CreateSessionAsync(userId, req);
            var envelope = new ApiResponse<object> { Success = res.Success, Message = res.Message, Data = res.Data, Code = res.StatusCode };
            return StatusCode(res.StatusCode, envelope);
        }

        [Authorize]
        [HttpPost("{id}/vote")]
        public async Task<IActionResult> Vote(Guid id, CastVoteRequest req)
        {
            var userId = Guid.Parse(User.FindFirst("id")?.Value ?? Guid.Empty.ToString());
            var res = await _service.CastVoteAsync(userId, id, req);
            var envelope = new ApiResponse<object> { Success = res.Success, Message = res.Message, Data = res.Data, Code = res.StatusCode };
            return StatusCode(res.StatusCode, envelope);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSession(Guid id)
        {
            var res = await _service.GetSessionAsync(id);
            var envelope = new ApiResponse<object> { Success = res.Success, Message = res.Message, Data = res.Data, Code = res.StatusCode };
            return StatusCode(res.StatusCode, envelope);
        }

        [HttpGet("public")]
        public async Task<IActionResult> Public(int page = 1, int pageSize = 20)
        {
            var res = await _service.GetPublicSessionsAsync(page, pageSize);
            var envelope = new ApiResponse<object> { Success = res.Success, Message = res.Message, Data = res.Data, Code = res.StatusCode };
            return StatusCode(res.StatusCode, envelope);
        }


        [HttpGet("expired")]
        public async Task<IActionResult> Expired(int page = 1, int pageSize = 20)
        {
            var res = await _service.GetExpiredSessionsAsync(page, pageSize);
            var envelope = new ApiResponse<object> { Success = res.Success, Message = res.Message, Data = res.Data, Code = res.StatusCode };
            return StatusCode(res.StatusCode, envelope);
        }

        [HttpGet("personal")]
        public async Task<IActionResult> SessionsPerUser(int page = 1, int pageSize = 20)
        {
            var userId = Guid.Parse(User.FindFirst("id")?.Value ?? Guid.Empty.ToString());
            if (userId == Guid.Empty)
            {
                var errorEnvelope = new ApiResponse<object> { Success = false, Message = "Unauthorized", Data = null, Code = 401 };
                return StatusCode(401, errorEnvelope);
            }
            var res = await _service.GetSessionsAsync(page, pageSize, userId);
            var envelope = new ApiResponse<object> { Success = res.Success, Message = res.Message, Data = res.Data, Code = res.StatusCode };
            return StatusCode(res.StatusCode, envelope);
        }



    }
}
