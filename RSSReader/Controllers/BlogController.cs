using System.Linq;
using System.Threading.Tasks;
using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RSSReader.Helpers;
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
        [HttpGet("{blogid}")]
        public async Task<ApiResponse> Get(int blogid)
        {
            return null;
        }

        [HttpGet("subscribedList")]
        public async Task<ApiResponse> List([FromServices] IBlogListService service)
        {
            var list = await service.GetListAsync(this.GetCurUserId());
            return new ApiResponse(MsgSucceed, list, Status200OK);
        }

        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<ApiResponse> Search([FromBody] string searchValue, [FromServices] IBlogSearchService service)
        {
            var list = await service.Search(searchValue);
            return new ApiResponse(MsgSucceed, list, Status200OK);
        }
    }
}
