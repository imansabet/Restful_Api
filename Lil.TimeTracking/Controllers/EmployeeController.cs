using Lil.TimeTracking.Models;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

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
            var lEmployees = new List<Resources.LinkedResource<Resources.Employee>>();
            foreach(var e in response)
            {
                var lEmp = new Resources.LinkedResource<Resources.Employee>(e);
                lEmp.Links.Add(new Resources.Resource("Projects",$"/api/Employee/{e.Id}/Projects"));
                lEmployees.Add(lEmp);
            }

            return Ok(lEmployees);
        }

        // GET api/<EmployeeController>/5/Projects
        [HttpGet("{id}/Projects")]
        [ProducesResponseType<List<Resources.Project>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProjects(int id)  
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }
            else
            {
                await _context.Entry(employee).Collection(e => e.Projects).LoadAsync();
                var projects = new List<Resources.Project>();

                foreach (var p in employee.Projects)
                {
                    var rProject = p.Adapt<Resources.Project>();
                    projects.Add(rProject);
                }

                return Ok(projects);
            }
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
            var lEmployee = new Resources.LinkedResource<Resources.Employee>(response);
            lEmployee.Links.Add(new Resources.Resource("Projects", $"/api/Employee/{response.Id}/Projects"));


            return Ok(lEmployee);
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
        [ProducesResponseType<ObjectResult>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ObjectResult>(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType<ObjectResult>(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put(int id, [FromBody] Resources.Employee value)
        {
            if (!ModelState.IsValid)
            {
                return Problem("Invalid employee request", statusCode: StatusCodes.Status400BadRequest);
            }
            try
            {
                var dbEmployee = value.Adapt<Models.Employee>();
                
                _context.Entry<Models.Employee>(dbEmployee).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                
                await _context.SaveChangesAsync();


                return NoContent();
            }
            catch (DbUpdateConcurrencyException dbex)
            {
                var dbEmployee = _context.Employees.Find(id);
                if(dbEmployee == null)
                {
                    return NotFound();
                }
                else
                {
                    return Problem("Problem persisting employee resource", statusCode: StatusCodes.Status500InternalServerError);
                }

            }
            catch (Exception ex)
            {
                return Problem("Problem persisting employee resource", statusCode: StatusCodes.Status500InternalServerError);

            }
        }

        [HttpPatch("{id}")]
        [ProducesResponseType<Resources.Employee>(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ObjectResult>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ObjectResult>(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType<ObjectResult>(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Patch(int id, [FromBody] JsonPatchDocument<Resources.Employee> value)
        {
            try
            {
                var dbEmployee = await _context.Employees.FindAsync(id);
                if(dbEmployee == null)
                {
                    return NotFound();
                }
                var employee = dbEmployee.Adapt<Resources.Employee>();
                value.ApplyTo(employee, ModelState);

                var patchedEmployee = employee.Adapt<Models.Employee>();

                _context.Entry<Models.Employee>(dbEmployee).CurrentValues.SetValues(patchedEmployee);

                await _context.SaveChangesAsync();

                return NoContent();
            }
            
            catch (Exception ex)
            {
                return Problem("Problem persisting employee resource", statusCode: StatusCodes.Status500InternalServerError);

            }
        }




        // DELETE api/<EmployeeController>/5
        [HttpDelete("{id}")]
        [ProducesResponseType<Resources.Employee>(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ObjectResult>(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType<ObjectResult>(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var dbEmployee = await _context.Employees.FindAsync(id);
                if (dbEmployee == null)
                {
                    return NotFound();
                }
                _context.Employees.Remove(dbEmployee);

                await _context.SaveChangesAsync();

                return NoContent();
            }

            catch (Exception ex)
            {
                return Problem("Problem deleting employee resource", statusCode: StatusCodes.Status500InternalServerError);

            }
        }
    }
}
