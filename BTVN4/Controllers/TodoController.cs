using Microsoft.AspNetCore.Mvc;
using BTVN4.Models;
using System.Collections.Generic;
using System.Linq;

namespace BTVN4.Controllers
{
    public class TodoController : Controller
    {
        private static List<Todo> todos = new List<Todo>
        {
            new Todo { Id = 1, Name = "Đi chợ", IsCompleted = true },
            new Todo { Id = 2, Name = "Chơi thể thao", IsCompleted = false },
            new Todo { Id = 3, Name = "Chơi game", IsCompleted = false },
            new Todo { Id = 4, Name = "Học bài", IsCompleted = true }
        };

        public IActionResult Index()
        {
            return View(todos);
        }

        public IActionResult Details(int id)
        {
            var todo = todos.FirstOrDefault(t => t.Id == id);
            if (todo == null)
            {
                return NotFound();
            }
            return View(todo);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Todo todo)
        {
            if (ModelState.IsValid)
            {
                todos.Add(todo);
                return RedirectToAction("Index");
            }
            return View(todo);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var todo = todos.FirstOrDefault(t => t.Id == id);
            if (todo == null)
            {
                return NotFound();
            }
            return View(todo);
        }

        [HttpPost]
        public IActionResult Edit(Todo todo)
        {
            if (ModelState.IsValid)
            {
                var existingTodo = todos.FirstOrDefault(t => t.Id == todo.Id);
                if (existingTodo != null)
                {
                    existingTodo.Name = todo.Name;
                    existingTodo.IsCompleted = todo.IsCompleted;
                    return RedirectToAction("Index");
                }
                return NotFound();
            }
            return View(todo);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var todo = todos.FirstOrDefault(t => t.Id == id);
            if (todo == null)
            {
                return NotFound();
            }
            return View(todo);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var todo = todos.FirstOrDefault(t => t.Id == id);
            if (todo != null)
            {
                todos.Remove(todo);
            }
            return RedirectToAction("Index");
        }
    }
}
