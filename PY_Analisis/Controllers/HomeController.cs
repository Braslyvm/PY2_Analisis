using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PY_Analisis.Models;
using AGBACKEND;

namespace PY_Analisis.Controllers;

public class HomeController : Controller
{
    public static List<Paciente> ListaPaciente { get; set; } = new List<Paciente>();
    public static List<Especialidad> ListaEspecialidad { get; set; } = new List<Especialidad>();  
    private static bool datosCargados = false;
      public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Sala()
    {
       if (!datosCargados)
    {
        await CargarPaciente();
        await CargarEspecialidad();
        datosCargados = true;
    }
        return View(ListaPaciente);
    }
      [HttpPost]
   [HttpPost]
    public IActionResult AgregarEspecialidad(string nombre, string duracion)
    {
        if (TimeOnly.TryParse(duracion, out TimeOnly duracionTime))
        {
            var nuevaEspecialidad = new Especialidad(nombre, duracionTime);
            ListaEspecialidad.Add(nuevaEspecialidad);
            Console.WriteLine("Nueva especialidad agregada:");


            foreach (var especialidad in ListaEspecialidad)
    {
        Console.WriteLine($"ID: {especialidad.IdEspecialidad}, Nombre: {especialidad.Nombre}, Duración: {especialidad.Duracion}");
    }

         
            return RedirectToAction("AgregarEspecialidad");
        }

        ModelState.AddModelError("", "Formato de duración no válido.");
        return View("AgregarPacientes");
    }


    // metodos get formularios 

  
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

    public async Task CargarPaciente()
    {
        string ruta = Path.Combine(Directory.GetCurrentDirectory(), "ArchivosJSON", "Pacientes.json");
        using FileStream leer = System.IO.File.OpenRead(ruta);
        ListaPaciente = await JsonSerializer.DeserializeAsync<List<Paciente>>(leer);
            foreach (var paciente in ListaPaciente)
        {
            Console.WriteLine($"ID: {paciente.IdPaciente}, Nombre: {paciente.Nombre}, Apellido: {paciente.Apellido}");
        }
    }
    public async Task CargarEspecialidad()
    {

        string ruta = Path.Combine(Directory.GetCurrentDirectory(), "ArchivosJSON", "Especialidades.json");
        using FileStream leer = System.IO.File.OpenRead(ruta);
        ListaEspecialidad = await JsonSerializer.DeserializeAsync<List<Especialidad>>(leer);
          foreach (var especialidad in ListaEspecialidad)
    {
        Console.WriteLine($"ID: {especialidad.IdEspecialidad}, Nombre: {especialidad.Nombre}, Duración: {especialidad.Duracion}");
    }
    }
}

    


