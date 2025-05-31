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
    public List<Paciente> ListaPaciente { get; set; } = new List<Paciente>();
    public List<Especialidad> ListaEspecialidad { get; set; } = new List<Especialidad>();  
    
      public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Sala()
    {
        await CargarPaciente();
        await CargarEspecialidad();
        return View(ListaPaciente);
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

    public async Task CargarPaciente()
    {
        string ruta = Path.Combine(Directory.GetCurrentDirectory(), "ArchivosJSON", "Pacientes.json");
        using FileStream leer = System.IO.File.OpenRead(ruta);
        ListaPaciente = await JsonSerializer.DeserializeAsync<List<Paciente>>(leer);

    }
     public async Task CargarEspecialidad()
    {
        string ruta = Path.Combine(Directory.GetCurrentDirectory(), "ArchivosJSON", "Especialidades.json");
        using FileStream leer = System.IO.File.OpenRead(ruta);
        ListaEspecialidad = await JsonSerializer.DeserializeAsync<List<Especialidad>>(leer);
    }
}  

