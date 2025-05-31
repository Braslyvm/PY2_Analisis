using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PY_Analisis.Models;

namespace PY_Analisis.Controllers;

public class HomeController : Controller
{
      public IActionResult Index()
    {
        return View();
    }

    public IActionResult Sala()
    {
        return View();
    }

    // metodos get formularios 

    public IActionResult CrearEspecialidad()
    {
        return View();
    }
            public IActionResult AgregarPacientes()
    {
       return PartialView("AgregarPacientes");
    }
     public IActionResult AgregarEspecialidad()
    {
       return PartialView("AgregarEspecialidad");
    }
     public IActionResult CrearConsultorio()
    {
       return PartialView("CrearConsultorio");
    }

    }  

