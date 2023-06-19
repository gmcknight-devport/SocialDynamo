using Microsoft.AspNetCore.Mvc;

namespace KubeTest.Controllers
{
    [ApiController]
    [Route("api")]
    public class TestCont
    {
        List<string> str = new()
        {
            "THing1", "Thing2", "SomethingElse"
        };

        [HttpGet("get")]
        public List<string> Get()
        {
            return str;
        }
    }
}
