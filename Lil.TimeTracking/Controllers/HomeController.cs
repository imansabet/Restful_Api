﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lil.TimeTracking.Controllers
{
    [Route("")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType<IEnumerable<Resources.Resource>>(StatusCodes.Status200OK)]
        public IActionResult Get()
        {
            var resources = new List<Resources.Resource>
            {
                new Resources.Resource("Employess","/api/Employee"),
                new Resources.Resource("Projects","/api/Project"),
                new Resources.Resource("Time Entries","/api/TimeEntry"),
                new Resources.Resource("Project Assignments","/api/ProjectAssignment"),
            };
            return Ok(resources);
        }
    }
}
