using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using AutoWrapper.Wrappers;
using static Microsoft.AspNetCore.Http.StatusCodes;
using static RSSReader.Data.Response;
using static RSSReader.Data.UserRepository;
using RSSReader.Models;

namespace RSSReader.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class BlogController : Controller
    {
        [HttpGet("{id}/list")]
        public async Task<ApiResponse> GetUserPostDataList()
        {
            return new ApiResponse(MsgSucceed, new List<UserPostData>(), Status200OK);
        }
    }
}
