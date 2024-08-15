using Lil.TimeTracking.Models;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lil.TimeTracking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private TimeTrackingDbContext _context;

        public EmployeeController(TimeTrackingDbContext context)
        {
            _context = context;
        }
        // GET: api/<EmployeeController>
        [HttpGet]
        [ProducesResponseType<IEnumerable<Resources.Employee>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            var response = _context.Employees.ProjectToType<Resources.Employee>().AsEnumerable();

            return Ok(response);
        }

        // GET api/<EmployeeController>/5
        [HttpGet("{id}")]
        [ProducesResponseType<Resources.Employee>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            var dbEmployee = await _context.Employees.FindAsync(id);

            if (dbEmployee == null)
            {
                return NotFound();
            }

            var response = dbEmployee.Adapt<Resources.Employee>();
            return Ok(response);
        }

        // POST api/<EmployeeController>
        [HttpPost]
        [ProducesResponseType<Resources.Employee>(StatusCodes.Status201Created)]
        [ProducesResponseType<ObjectResult>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ObjectResult>(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] Resources.Employee value)
        {
            if (!ModelState.IsValid)
            {
                return Problem("Invalid employee request", statusCode: StatusCodes.Status400BadRequest);
            }
            try
            {
                var dbEmployee = value.Adapt<Models.Employee>();
                await _context.Employees.AddAsync(dbEmployee);
                await _context.SaveChangesAsync();

                var respone = dbEmployee.Adapt<Resources.Employee>();
                return CreatedAtAction(nameof(Get), new { id = respone.Id }, respone);
            }catch(Exception ex)
            {
                return Problem("Problem persisting employee resource", statusCode: StatusCodes.Status500InternalServerError);

            }

        }

        // PUT api/<EmployeeController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<EmployeeController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
