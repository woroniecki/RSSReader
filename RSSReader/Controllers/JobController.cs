using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoWrapper.Wrappers;
using Dtos.Jobs;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.JobServices;
using static Microsoft.AspNetCore.Http.StatusCodes;
using static RSSReader.Data.Response;


namespace RSSReader.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobController : Controller
    {
        const string UPDATE_BLOGS_KEY = "MLv3j0ieDnvJda1";

        [HttpPost("update_blogs")]
        public async Task<ApiResponse> UpdateBlogs([FromBody] RunJobRequest data, [FromServices] IUpdateBlogsJobService service)
        {
            if (data.Key == UPDATE_BLOGS_KEY)
            {
                var result = await service.UpdateBlogs();
                return new ApiResponse(MsgSucceed, result, Status200OK);
            }

            return ErrUnauhtorized;
        }
    }
}
