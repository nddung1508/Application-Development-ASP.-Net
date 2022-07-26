﻿using AppDev.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using WebApplication2.Data;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
  [Authorize]
  public class TodoesController : Controller
  {
    private ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    public TodoesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
      _context = context;
      _userManager = userManager;
    }
    [HttpGet]
    [HttpGet]
    public IActionResult Index(string category)
    {
      var currentUserId = _userManager.GetUserId(User);
      if (!string.IsNullOrWhiteSpace(category))
      {
        var result = _context.Todoes
          .Include(t => t.Category)
          .Where(t => t.Category.Description.Equals(category)
             && t.UserId == currentUserId
          )
          .ToList();

        return View(result);
      }

      IEnumerable<Todo> todoes = _context.Todoes
        .Include(t => t.Category)
        .Where(t => t.UserId == currentUserId)
        .ToList();
      return View(todoes);
    }

    [HttpGet]
    public IActionResult Create()
    {
      var viewModel = new TodoCategoriesViewModel()
      {
        Categories = _context.Categories.ToList()
      };
      return View(viewModel);
    }
    [HttpPost]
    public IActionResult Create(TodoCategoriesViewModel viewModel)
    {
      if (!ModelState.IsValid) //indicates if it was possible to bind
       //the incoming values from the request to the
       //model correctly  and whether any explicitly specified
       //validation rules were broken during the model binding process.
      {
        viewModel = new TodoCategoriesViewModel
        {
          Categories = _context.Categories.ToList()
        };
        return View(viewModel);
      }
      var newTodo = new Todo
      {
        Description = viewModel.Todo.Description,
        CategoryId = viewModel.Todo.CategoryId
      };

      _context.Add(newTodo);
      _context.SaveChanges();
      return RedirectToAction("Index");
      
    }
    [HttpGet]
    public IActionResult Delete(int id)
    {
      var todoInDb = _context.Todoes.SingleOrDefault(t => t.Id == id);
      if (todoInDb is null)
      {
        return NotFound();
      }

      _context.Todoes.Remove(todoInDb);
      _context.SaveChanges();
      return RedirectToAction("Index");
    }
    [HttpGet]
    public IActionResult Edit(int id)
    {
      var todoInDb = _context.Todoes.SingleOrDefault(t => t.Id == id);
      
      if (todoInDb is null)
      {
        return NotFound();
      }

      var viewModel = new TodoCategoriesViewModel
      {
        Todo = todoInDb,
        Categories = _context.Categories.ToList()
      };
      return View(viewModel);
    
  }

    [HttpPost]
    public IActionResult Edit(TodoCategoriesViewModel viewModel)
    {
      var todoInDb = _context.Todoes.SingleOrDefault(t => t.Id == viewModel.Todo.Id);

      if (todoInDb is null)
      {
        return BadRequest();
      }
      if (!ModelState.IsValid)
      {
        viewModel = new TodoCategoriesViewModel
        {
          Todo = viewModel.Todo,
          Categories = _context.Categories.ToList()
        };
        return View(viewModel);
      }

      todoInDb.Description = viewModel.Todo.Description;
      todoInDb.Status = viewModel.Todo.Status;
      todoInDb.CategoryId = viewModel.Todo.CategoryId;
      _context.SaveChanges();
      return RedirectToAction("Index");
    }
    [HttpGet]
    public IActionResult Detail(int id)
    {
      var todoInDb = _context.Todoes
        .Include(t => t.Category)
        .SingleOrDefault(t => t.Id == id);
      if (todoInDb is null)
      {
        return NotFound();
      }

      return View(todoInDb);
    }
  }
}
