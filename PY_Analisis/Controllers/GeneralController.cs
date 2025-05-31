using Microsoft.AspNetCore.Mvc;
using AGBACKEND; // Importa tus clases

namespace PY_Analisis.Controllers;

public class GeneralController : Controller
{
    //Listas de objetos 
    public static List<Especialidad> Especialidades = new();

    //metodos 
    public IActionResult CrearEspecialidad(string nombre, TimeOnly duracion)
    {

        duracionTime = ObtenerSegundosTotales(duracion);
        // Crear la especialidad
        var nuevaEspecialidad = new Especialidad(nombre, duracionTime);
        Especialidades.Add(nuevaEspecialidad);
        return true;
    }






    // metodos auxiliares de la clase 
    public static int ObtenerSegundosTotales(TimeOnly duracion)
    {
        return (int)duracion.ToTimeSpan().TotalSeconds;
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
    
