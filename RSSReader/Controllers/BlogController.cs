using System.Linq;
using System.Threading.Tasks;
using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RSSReader.Helpers;
using ServiceLayer._CQRS;
using ServiceLayer._CQRS.BlogQueries;
using ServiceLayer.BlogServices;
using static Microsoft.AspNetCore.Http.StatusCodes;
using static RSSReader.Data.Response;

namespace RSSReader.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class BlogController : Controller
    {
        private IQueriesBus _queriesBus;

        public BlogController(IQueriesBus queriesBus)
        {
            _queriesBus = queriesBus;
        }

        [HttpGet("subscribedList")]
        public async Task<ApiResponse> List([FromServices] IBlogListService service)
        {
            var list = await service.GetListAsync(this.GetCurUserId());
            return new ApiResponse(MsgSucceed, list, Status200OK);
        }

        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<ApiResponse> Search(string value)
        {
            var response = await _queriesBus.Get(
                new SearchBlogsQuery()
                {
                    SearchValue = value
                });

            return new ApiResponse(MsgSucceed, response, Status200OK);
        }
    }
}
