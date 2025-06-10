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

    public static Puerta Puerta1 { get; set; } = new Puerta("Puerta 1");
    public static Puerta Puerta2 { get; set; } = new Puerta("Puerta 2");
    public static Puerta Puerta3 { get; set; } = new Puerta("Puerta 3");
    public static Puerta Puerta4 { get; set; } = new Puerta("Puerta 4");
    public static Puerta Puerta5 { get; set; } = new Puerta("Puerta 5");
    public static List<Cita> ColaCitas { get; set; } = new List<Cita>();
    private static System.Timers.Timer? _timer; //deleay para llamadas a fitnes

    private static bool datosCargados = false;
    public IActionResult Index()
    {
        return View();
    }
    public HomeController()
    {
        if (_timer == null)
        {
            _timer = new System.Timers.Timer(10000); // 10 segundos
            _timer.Elapsed += TimerElapsed;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }
    }

    private void TimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        try
        {
            fitnes1(); 
        }
        catch (Exception ex)
        { 
             TempData["Error"] = $"Error en TimerElapsed: {ex.Message}";
        }
    }
    public async Task<IActionResult> Sala()
    {
        if (!datosCargados)
        {
            await CargarPaciente();
            await CargarEspecialidad();

            var Consultorio = new Consultorios();
            Consultorio.RegistrarEspecialidad(1);
            Consultorio.RegistrarEspecialidad(2);
            ListaConsultorios.Add(Consultorio);
            Puerta1.CambiarEstado(Consultorio);
            datosCargados = true;
            datosCargados = true;
        }
            ViewBag.Puerta1  = Puerta1 ;
            ViewBag.Puerta2  = Puerta2 ;
            ViewBag.Puerta3  = Puerta3 ;
            ViewBag.Puerta4  = Puerta4 ;
            ViewBag.Puerta5  = Puerta5 ;

        return View(ListaPaciente);
    }
    public IActionResult MostrarFila(string ver)
    {
        ViewBag.ListaEspecialidad = ListaEspecialidad;
        ViewBag.ListaConsultorios = ListaConsultorios;
        if (ver == "1")
        {
            return PartialView("EstadosConsultorios", Puerta1);
        }
        if (ver == "2"){
            return PartialView("EstadosConsultorios", Puerta2);
        }
        if (ver == "3"){
            return PartialView("EstadosConsultorios", Puerta3);
        }
        if (ver == "4"){
            return PartialView("EstadosConsultorios", Puerta4);
        }
        else{
            return PartialView("EstadosConsultorios", Puerta5);
        }
    }

    public IActionResult CerrarConsultorio(string nombre){
        if (nombre == Puerta1.Nombre)
        {
            Puerta1.Cerrar();
        }

        if (nombre == Puerta2.Nombre)
        {
            Puerta2.Cerrar();
        }
        if (nombre == Puerta3.Nombre)
        {
            Puerta3.Cerrar();
        }
        if (nombre == Puerta4.Nombre)
        {
            Puerta4.Cerrar();
        }
        if (nombre == Puerta5.Nombre)
        {
            Puerta5.Cerrar();
        }
        return RedirectToAction("Sala");
    }


    [HttpPost]
    public IActionResult AbrirConsultorio(string nombre, int idConsultorio)
    {
        var consultorio = ListaConsultorios.FirstOrDefault(c => c.IdConsultorio == idConsultorio);
        if (nombre == Puerta1.Nombre)
        {
            Puerta1.Cerrar();
            Puerta1.CambiarEstado(consultorio);
        }

        if (nombre == Puerta2.Nombre)
        {
            Puerta2.Cerrar();
            Puerta2.CambiarEstado(consultorio);
        }
        if (nombre == Puerta3.Nombre)
        {
            Puerta3.Cerrar();
            Puerta3.CambiarEstado(consultorio);
        }
        if (nombre == Puerta4.Nombre)
        {
            Puerta4.Cerrar();
            Puerta4.CambiarEstado(consultorio);
        }
        if (nombre == Puerta5.Nombre)
        {
            Puerta5.Cerrar();
            Puerta5.CambiarEstado(consultorio);
        }
        return RedirectToAction("Sala");
    }

   [HttpPost]
    public IActionResult AgregarEspecialidad(string nombre, int duracion)
    {
        if (ListaPaciente.Any(p => p.Nombre == nombre)) { 
            ModelState.AddModelError("Especialidad", "No es posible registrar duplicados de especialidad.");
        } 

            var nuevaEspecialidad = new Especialidad(nombre, duracion);

            ListaEspecialidad.Add(nuevaEspecialidad);
            Console.WriteLine("Nueva especialidad agregada:");


            foreach (var especialidad in ListaEspecialidad)
            {
                Console.WriteLine($"ID: {especialidad.IdEspecialidad}, Nombre: {especialidad.Nombre}, Duración: {especialidad.Duracion}");
            }
            return RedirectToAction("Sala");
        }
    

        
      
    

    [HttpPost]
    public IActionResult AgregarPacientes(string nombre, string apellido, int cedula)
    {
      
        if (ListaPaciente.Any(p => p.Cedula == cedula))
        {
            ModelState.AddModelError("Cedula", "La cédula ya está registrada.");
            
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

    
    var especialidad = ListaEspecialidad.FirstOrDefault(e => e.IdEspecialidad == idEspecialidad);
    if (especialidad == null)
    {
        TempData["Error"] = "Especialidad no encontrada.";
        return RedirectToAction("Sala");
    }

  
    if (Citas.Any(c => c.Especialidad.IdEspecialidad == idEspecialidad && c.IdPaciente == idPaciente))
    {
        TempData["Error"] = "El paciente ya tiene una cita asignada en esta especialidad.";
        return RedirectToAction("Sala");
    }

   
    var nuevaCita = new Cita(especialidad, idPaciente);
    Citas.Add(nuevaCita);
    paciente.Citas.Add(nuevaCita);
    Console.WriteLine($"Nueva cita agendada: ID:{nuevaCita.IdCita}, Especialidad:{especialidad.Nombre}, Paciente:{idPaciente}");
    TempData["Mensaje"] = "Cita agendada exitosamente.";
    fitnes1(); 
    return RedirectToAction("Sala");
}
    //esta funcion solo lee las citas que hay y las asigna a cola o a una fila
   // falta validar que no pueda entrar en el tiempo estimado.
   //validar que la fila tenga consultorio asignado
    public IActionResult fitnes1()
{     Console.WriteLine("⚙️ Timer ejecutando fitnes1: Verificando citas pendientes...");
    var citasPendientes = Citas.Where(c => !c.asignada).ToList();

    foreach (var cita in citasPendientes)
    {       Console.WriteLine($"🕵️ Revisando cita ID: {cita.IdCita}, Especialidad: {cita.Especialidad.Nombre}, Paciente ID: {cita.IdPaciente}");

        //si alguien en cola
        Cita citaYaEnCola = ColaCitas.FirstOrDefault(c => c.Especialidad.IdEspecialidad == cita.Especialidad.IdEspecialidad);

        if (citaYaEnCola == null)//si no hat nadie en la cola
        {
            bool asignada = AsignarPuertaACita(cita);

            if (asignada)
            {
                cita.asignada = true;
                Console.WriteLine($"✅ Cita {cita.IdCita} asignada directamente a una puerta.");
          
            }
            else
            {
                if (!ColaCitas.Contains(cita))
                {
                    ColaCitas.Add(cita);
                     Console.WriteLine($"🔄 Cita {cita.IdCita} no fue asignada, agregada a la cola.");
               
                }
            }
        }
        else{
            //si hay alguien en la cola
            bool asignadaDesdeCola = AsignarPuertaACita(citaYaEnCola);
            if (asignadaDesdeCola)
                {
                    citaYaEnCola.asignada = true;
                    ColaCitas.Remove(citaYaEnCola);
                 Console.WriteLine($"🚪 Cita {citaYaEnCola.IdCita} fue tomada desde la cola y asignada a una puerta.");
           
                }
                if (!ColaCitas.Contains(cita))
                {
                    ColaCitas.Add(cita); 
                    Console.WriteLine($"📥 Cita {cita.IdCita} agregada a la cola.");
          
                }
        }
    }
     Console.WriteLine("✔️ Proceso de asignación finalizado.");
    
    return Ok("Proceso de asignación completado.");
}

//esto solo asigna a la primer puerta que tenga menos tiempo 
private bool AsignarPuertaACita(Cita cita)
{
    var puertas = new List<Puerta> { Puerta1, Puerta2, Puerta3, Puerta4, Puerta5 };
    var puertasDisponibles = puertas
        .Where(p => p.Estado &&
                    p.Consultorio != null &&
                    p.Consultorio.IdEspecialidades.Contains(cita.Especialidad.IdEspecialidad))
        .OrderBy(p => p.Duracion)
        .ToList();

    if (puertasDisponibles.Any())
    {
        var mejorPuerta = puertasDisponibles.First();
        if (mejorPuerta.AgregarCita(cita))
        {
            return true;
        }
    }
    return false;
}}


       
    





