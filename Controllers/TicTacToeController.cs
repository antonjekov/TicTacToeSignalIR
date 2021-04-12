using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicTacToeSignalRWebApp.Controllers
{
    [Authorize]
    public class TicTacToeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
