using System;
using Microsoft.AspNetCore.Mvc;
using Rng.Services;

namespace Rng.Controllers
{
    [Route("api/rng")]
    public class RngController : Controller
    {
        private IRandonNumberGenerator generator;

        public RngController(IRandonNumberGenerator gen)
        {
            this.generator = gen;
        }

        // GET api/rng/32
        [HttpGet("{length}")]
        public string Get(int length)
        {
            return generator.Generate(length);
        }
    }
}
