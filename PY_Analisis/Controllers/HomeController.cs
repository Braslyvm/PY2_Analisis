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
    public static List<Consultorios> ListaConsultorios { get; set; } = new List<Consultorios>();
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
    public IActionResult AgregarEspecialidad(string nombre, string duracion)
    {
        if (ListaPaciente.Any(p => p.Nombre == nombre))
        {
            ModelState.AddModelError("Especialidad", "No es posible registrar duplicados de especialidad.");
            //return PartialView("AgregarPacientes"); redireccionar a donde corresponda
        }
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
        return RedirectToAction("Sala");
    }

    [HttpPost]
    public IActionResult AgregarPacientes(string nombre, string apellido, int cedula)
    {
        if (ListaPaciente.Any(p => p.Cedula == cedula))
        {
            ModelState.AddModelError("Cedula", "La cédula ya está registrada.");
            //return PartialView("AgregarPacientes"); redireccionar a donde corresponda
        }
        var nuevoPaciente = new Paciente(nombre, apellido, cedula);
        ListaPaciente.Add(nuevoPaciente);
        Console.WriteLine("Nueva PACIENTE agregada:");
        foreach (var paciente in ListaPaciente)
        {
            Console.WriteLine($"ID: {paciente.IdPaciente}, Nombre: {paciente.Nombre}, Apellido: {paciente.Apellido},cedula: {paciente.Cedula}");
        }
        return RedirectToAction("Sala");
    }
    [HttpPost]
    public IActionResult CrearConsultorio(string nuevoPaciente)
    {
        return PartialView("CrearConsultorio");
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

 public IActionResult CrearConsultorio()
    {
        if (ListaConsultorios.Count >= 15)
        {
            Console.WriteLine("LLeno");
            TempData["Error"] = "La lista de consultorios ya esta llena";

        }
        else
        {
            Consultorios nueva = new Consultorios();
            ListaConsultorios.Add(nueva);
            TempData["Mensaje"] = "Se ha creado el consultorio exitosamente.";
        }

         return RedirectToAction("Sala");
    }
}

    


