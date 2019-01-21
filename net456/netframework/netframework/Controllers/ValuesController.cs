using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace netframework.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : ApiController
    {
        [HttpGet]
        [Route("1")]
        public IEnumerable<string> RequestCollection()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet]
        [Route("2")]
        public string RequestRawString()
        {
            return "value";
        }
    }
}
