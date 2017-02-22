using Hasher.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hasher.Controllers
{
    [Route("api/hasher")]
    public class HasherController : Controller
    {
        private readonly IHasher hasher;

        public HasherController(IHasher hasherToUse)
        {
            hasher = hasherToUse;
        }
 
        // POST api/values
        [HttpPost]
        public string Post([FromBody]string valueToHash)
        {
            return hasher.HashString(valueToHash);
        }
    }
}
