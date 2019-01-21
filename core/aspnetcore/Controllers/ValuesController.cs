using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace aspnetcore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        [Route("1")]
        public ActionResult<IEnumerable<string>> RequestCollection()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet]
        [Route("2")]
        public ActionResult<string> RequestRawString()
        {
            return "value";
        }
    }
}
