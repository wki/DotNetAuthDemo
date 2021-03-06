﻿using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auth2Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] {"value1", "value2"};
        }

        [Authorize(Policy = "Users")]
        [HttpGet("secret")]
        public ActionResult<string> Secret()
        {
            var name = "";
            var id = "";
            var user = ControllerContext?.HttpContext?.User;
            if (user != null && user.HasClaim(c => c.Type == ClaimTypes.Name))
                name = user.Claims.First(c => c.Type == ClaimTypes.Name).Value;
            if (user != null && user.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
                id = user.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

            return Ok($"huhu:{name}/{id}");
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}