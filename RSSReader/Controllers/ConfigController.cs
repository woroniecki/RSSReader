using Microsoft.AspNetCore.Mvc;
using ServiceLayer.CronServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RSSReader.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfigController : Controller
    {
        private IAppConfig _appCofnig;

        public ConfigController(IAppConfig appCofnig)
        {
            _appCofnig = appCofnig;
        }

        [HttpGet]
        public object Index()
        {
            return new
            {
                config = _appCofnig.GetConfigName(),
                version = _appCofnig.GetAppVersion()
            };
        }
    }
}
