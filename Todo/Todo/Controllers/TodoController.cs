using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Todo.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Todo.Controllers
{

    [ApiVersion("1.0")]
    [ApiVersion("1.1")]
    [Route("api/[controller]")]
    public class TodoController : Controller
    {
        private readonly TodoContext _context;
        public TodoController(TodoContext context)
        {
            _context = context;
            if (_context.TodoItems.Count() == 0)
            {
                TodoItem item = new TodoItem { Name = "Item1" };
                _context.TodoItems.Add(item);
                _context.SaveChanges();
            }
        }

        [HttpGet]
        public IEnumerable<TodoItem> GetAll()
        {
            return _context.TodoItems.ToList();
        }

        [HttpGet("{id}", Name = "GetTodo")]
        public IActionResult GetById(long id)
        {

            var item = _context.TodoItems.FirstOrDefault(t => t.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            return new ObjectResult(item);
        }

        //Create
        /// <summary>
        /// Creates new Item
        /// </summary>
        /// <param name="item"></param>
        /// <returns>New Created Item</returns>
        /// <response code="200">Returns newly created item</response>
        /// <response code="400">If the item is null</response>
        [HttpPost]
        public IActionResult Create([FromBody] TodoItem item)
        {

            if (item == null) return NotFound();

            _context.TodoItems.Add(item);
            _context.SaveChanges();

            return CreatedAtRoute("GetTodo", new { id = item.Id }, item);

        }

        //Update
        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] TodoItem item)
        {

            if (item == null || item.Id != id) return NotFound();

            var todoItem = _context.TodoItems.FirstOrDefault(t => t.Id == id);
            if (todoItem == null) return NotFound();

            todoItem.TodoItemId = item.TodoItemId;
            todoItem.Name = item.Name;
            todoItem.IsComplete = item.IsComplete;

            _context.TodoItems.Update(todoItem);
            _context.SaveChanges();

            //return CreatedAtRoute("GetTodo", new { id = item.Id }, item);
            return new NoContentResult();

        }

        //Delete
        /// <summary>
        /// Deletes a specific Todo Item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)] //Ignore this function in swagger docs
        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {

            var todoItem = _context.TodoItems.FirstOrDefault(t => t.Id == id);
            if (todoItem == null) return NotFound();

            _context.TodoItems.Remove(todoItem);
            _context.SaveChanges();

            //return CreatedAtRoute("GetTodo", new { id = item.Id }, item);
            return new NoContentResult();

        }

        [HttpGet("api/v1/About")] 
        [MapToApiVersion("1.0")]
        public ContentResult About()
        {
            return Content("An API to sample Swagger with Swashbuckle in ASP.NET Core.");
        }

        //http://localhost:60592/api/Todo/api/v2/about?api-version=1.1 (works default is 1.0)
        [HttpGet("api/v2/About")]
        [MapToApiVersion("1.1")]
        public ContentResult About2() => Content("An API (v2) to sample Swagger with Swashbuckle in ASP.NET Core.");

    }
}
