﻿using AppDev.Models;
using AppDev.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using WebApplication2.Data;

namespace AppDev.Controllers
{
  [Authorize(Roles = Role.ADMIN)]
  public class AdminController : Controller
  {
    private readonly UserManager<ApplicationUser> _userManager;

    private readonly ApplicationDbContext _context;

    public AdminController(
      UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
      _userManager = userManager;
      _context = context;
    }

    public IActionResult Index()
    {
      return View();
    }

    [HttpGet]
    public IActionResult Users()
    {
      var usersWithPermission = _userManager.GetUsersInRoleAsync(Role.USER).Result;
      return View(usersWithPermission);
    }
  }
}
