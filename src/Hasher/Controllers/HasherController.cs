using System;
using Microsoft.AspNetCore.Mvc;

namespace Hasher.Controllers
{
    [Route("api/hasher")]
    public class HasherController : Controller
    {
 
        // POST api/values
        [HttpPost]
        public string Post([FromBody]string valueToHash)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(valueToHash);
            var hashAlgoritm = System.Security.Cryptography.MD5.Create();
            bytes = hashAlgoritm.ComputeHash(bytes);

            return Convert.ToBase64String(bytes);
        }
    }
}
