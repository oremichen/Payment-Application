using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ContentServiceManagementAPI.Controllers
{
    [Route("api/[controller]")]
    public class TestController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Content service management  is available");
        }


        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            return Ok($"{id} returned successfully");
        }
    }
}
