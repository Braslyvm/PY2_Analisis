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
    
    public static List<Cita> ColaCitas { get; set; } = new List<Cita>();
    private static System.Timers.Timer? _timer; //deleay para llamadas a fitnes

    List<Cita> citasPrioritariaslist = new List<Cita>();

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
            Consultorio.AbrirConsultorio();
            datosCargados = true;
           
        }


        ViewBag.Consultorios = ListaConsultorios;


        return View(ListaPaciente);
    }


  public IActionResult MostrarConsultorio(int id){
        var consultorio = ListaConsultorios.FirstOrDefault(c => c.IdConsultorio == id);

        return PartialView("EstadosConsultorios", consultorio);
    }
    
    [HttpPost]
    public IActionResult CerrarConsultorio(int IdConsultorio)
    {
        var consultorio = ListaConsultorios.FirstOrDefault(c => c.IdConsultorio == IdConsultorio);

        consultorio.EstadoConsultorio = false;
       
        var citasPrioritarias = new List<Cita>();
        if (consultorio.CitasAsignadas.Any())
        {
            foreach (var cita in consultorio.CitasAsignadas)
            {
                cita.asignada = false; 
                citasPrioritarias.Add(cita);
                
            }
            consultorio.CitasAsignadas.Clear();
            consultorio.Duracion=0; 
            fitnes1();
        }
        return RedirectToAction("Sala");
    }
    
    [HttpPost]
     public IActionResult AbrirConsultorio(int IdConsultorio)
    {
        var consultorio = ListaConsultorios.FirstOrDefault(c => c.IdConsultorio == IdConsultorio);

        consultorio.EstadoConsultorio = true;
        var citasPrioritarias = new List<Cita>();
        foreach (var c in ListaConsultorios)
        {
            if (c.IdConsultorio != IdConsultorio && c.EstadoConsultorio)
            {
                
                var citasAReorganizar = c.CitasAsignadas.Skip(1).ToList();

                foreach (var cita in citasAReorganizar)
                {
                    cita.asignada = false;
                    citasPrioritarias.Add(cita);
                    
                }
                c.CitasAsignadas = c.CitasAsignadas.Take(1).ToList();
                c.ContarDuracion(); 
            }
        }
        fitnes1();

        return RedirectToAction("Sala");
    }
     [HttpPost]


   [HttpPost]
    public IActionResult AgregarEspecialidad(string nombre, int duracion)
    {
        if (ListaPaciente.Any(p => p.Nombre == nombre)) { 
            ModelState.AddModelError("Especialidad", "No es posible registrar duplicados de especialidad.");
        } 

            var nuevaEspecialidad = new Especialidad(nombre, duracion);
            ListaEspecialidad.Add(nuevaEspecialidad);

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
    }
    public async Task CargarEspecialidad()
    {

        string ruta = Path.Combine(Directory.GetCurrentDirectory(), "ArchivosJSON", "Especialidades.json");
        using FileStream leer = System.IO.File.OpenRead(ruta);
        ListaEspecialidad = await JsonSerializer.DeserializeAsync<List<Especialidad>>(leer);
 
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
{
    Console.WriteLine("⚙️ Timer ejecutando fitnes1: Verificando citas pendientes...");

    // citas pendientes (no asignadas)
    var citasPendientes = Citas.Where(c => !c.asignada).ToList();

    //  citas previamente asignadas que vienen de consultorios cerrados CAMBIAR EL ESTADO DE LA CITA SEGUN CORRESPONDA 
    var citasPrioritarias  = citasPrioritariaslist
        .Where(c => !c.asignada)
        .ToList();
    ReorganizarCitas(citasPrioritarias, citasPendientes, ListaConsultorios, ColaCitas);

    // 4. Mostrar estado final de los consultorios
    Console.WriteLine("\n📋 Estado actual de consultorios y sus citas asignadas:");
    foreach (var consultorio in ListaConsultorios)
    {
        Console.WriteLine($"🟢 Consultorio {consultorio.IdConsultorio} (Abierto: {consultorio.EstadoConsultorio} duracion: {consultorio.Duracion})");
        if (consultorio.CitasAsignadas.Any())
        {
            foreach (var cita in consultorio.CitasAsignadas)
            {
                Console.WriteLine($"   ↳ Cita ID: {cita.IdCita}, Paciente: {cita.IdPaciente}, Especialidad: {cita.Especialidad?.Nombre ?? "Desconocida"}");
            }
        }
        else
        {
            Console.WriteLine("   ↳ Sin citas asignadas.");
        }
    }

    Console.WriteLine("✅ Proceso de asignación finalizado.");
    return Ok("Proceso de asignación completado.");
}



//esto solo asigna a la primer puerta que tenga menos tiempo 
private bool AsignarConsultorioACita(Cita cita, List<Consultorios> consultorios)
{
    var disponibles = consultorios
        .Where(c => c.EstadoConsultorio &&
                    c.IdEspecialidades.Contains(cita.Especialidad.IdEspecialidad))
        .OrderBy(c => c.Duracion)
        .ToList();

    if (disponibles.Any())
    {
        var mejor = disponibles.First();
        return mejor.AgregarCita(cita);
    }

    return false;
}

public void ReorganizarCitas(List<Cita> citasPrioritarias, List<Cita> citasNormales, List<Consultorios> consultorios, List<Cita> ColaCitas)
{
    Console.WriteLine("🔄 Reorganizando citas: Prioridad primero, luego por duración...");

    // 1. Asignar citas prioritarias primero (no importa duración)
    foreach (var cita in citasPrioritarias)
    {
        bool asignada = AsignarConsultorioACita(cita, consultorios);
        if (asignada)
        {
            cita.asignada = true;
            Console.WriteLine($"✅ Cita PRIORITARIA ID {cita.IdCita} asignada.");
        }
        else
        {
            if (!ColaCitas.Contains(cita))
            {
                ColaCitas.Insert(0, cita); // Insertar al inicio de la cola
                Console.WriteLine($"🕒 Cita PRIORITARIA ID {cita.IdCita} añadida al INICIO de la cola.");
            }
        }
    }

    //  Asignar citas normales (ordenadas por menor duración)
    var ordenadas = citasNormales
        .Where(c => !c.asignada)
        .OrderBy(c => c.ConsultarDuracion())
        .ToList();

    foreach (var cita in ordenadas)
    {
        bool asignada = AsignarConsultorioACita(cita, consultorios);
        if (asignada)
        {
            cita.asignada = true;
            Console.WriteLine($"✅ Cita NORMAL ID {cita.IdCita} asignada.");
        }
        else
        {
            if (!ColaCitas.Contains(cita))
            {
                ColaCitas.Add(cita); // Insertar al final de la cola
                Console.WriteLine($"🕒 Cita NORMAL ID {cita.IdCita} añadida al FINAL de la cola.");
            }
        }
    }

    Console.WriteLine("🎯 Reorganización finalizada.");
}




}


       
    





