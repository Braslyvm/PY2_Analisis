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
    public static List<Cita> Citas { get; set; } = new List<Cita>();


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
   

    public IActionResult AgregarPacientes()
    {
        return PartialView("AgregarPacientes");
    }
    public IActionResult AgregarEspecialidad()
    {
        return PartialView("AgregarEspecialidad");
    }
    public IActionResult consultorio()
    {
        return PartialView("consultorio", ListaConsultorios);
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

    public IActionResult EliminarEspecialidadDeConsultorio(int idConsultorio, int idEspecialidad){
        var consultorio = ListaConsultorios.FirstOrDefault(c => c.IdConsultorio == idConsultorio);
        if (consultorio == null){
            TempData["Error"] = "Consultorio no encontrado.";
            return RedirectToAction("Sala");
        }
        if (consultorio.EliminarEspecialidad(idEspecialidad)){
            TempData["Mensaje"] = "Especialidad eliminada exitosamente.";
        }
        else{
            TempData["Error"] = "La especialidad no estaba registrada en este consultorio.";
        }
        return RedirectToAction("Sala");
    }
    [HttpPost]
    public IActionResult AgregarEspecialidadAConsultorio(int idConsultorio, int idEspecialidad)
    {
        var consultorio = ListaConsultorios.FirstOrDefault(c => c.IdConsultorio == idConsultorio);
        if (consultorio == null){
            TempData["Error"] = "Consultorio no encontrado.";
            return RedirectToAction("Sala");
        }

        if (consultorio.IdEspecialidades.Count >= 5){
            TempData["Error"] = "Este consultorio ya tiene el máximo de especialidades.";
            return RedirectToAction("Sala");
        }

        if (consultorio.RegistrarEspecialidad(idEspecialidad)) {
            TempData["Mensaje"] = "Especialidad agregada exitosamente.";
        }
        else {
            TempData["Error"] = "La especialidad ya está registrada en este consultorio.";
        }
        return RedirectToAction("Sala");
    }

    
   public IActionResult AgendarCita()
{
    return PartialView("AgendarCita", ListaPaciente);
}

    [HttpPost]
    public IActionResult AgendarCita(int idPaciente, int idEspecialidad)
    {
        var paciente = ListaPaciente.FirstOrDefault(p => p.IdPaciente == idPaciente);
        if (paciente == null)
        {
            TempData["Error"] = "Paciente no encontrado.";
            return RedirectToAction("Sala");
        }

       
        if (Citas.Any(c => c.IdEspecialidad == idEspecialidad && c.IdPaciente == idPaciente))
        {
            TempData["Error"] = "El paciente ya tiene una cita asignada en esta especialidad.";
            return RedirectToAction("Sala");
        }

        var nuevaCita = new Cita(idPaciente, idEspecialidad);
        Citas.Add(nuevaCita);

        Console.WriteLine($"Nueva cita agendada: ID:{nuevaCita.IdCita}, Especialidad:{idEspecialidad}, Paciente:{idPaciente}");
        TempData["Mensaje"] = "Cita agendada exitosamente.";

        return RedirectToAction("Sala");
    }

    
}

    


