﻿using Microsoft.AspNetCore.Mvc;

namespace MusicStream.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
