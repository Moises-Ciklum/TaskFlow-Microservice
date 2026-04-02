using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Core.Entities;
using TaskFlow.Infrastructure.Data;

namespace TaskFlow.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        
        public TasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/task
        [HttpGet]
        public async Task<IActionResult> GetTasks()
        {
            // SELECT * FROM Tasks;
            var tasks = await _context.Tasks.ToListAsync();
            return Ok(tasks); // Devuelve un HTTP 200 con el JSON
        }

        // POST: api/tasks
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] TaskItem task)
        {
            // INSERT INTO Tasks ...
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            // Devuelve un HTTP 201 (Created)
            return CreatedAtAction(nameof(GetTasks), new { id = task.Id }, task);
        }
    }
}
