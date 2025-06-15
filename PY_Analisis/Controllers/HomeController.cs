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
    public static List<Cita> Citas { get; set; } = new List<Cita>();  //objeto citas 

    public static List<Cita> ColaCitas { get; set; } = new List<Cita>(); //citas procesadas sin consultorio 
    private static System.Timers.Timer? _timer; //deleay para llamadas a fitnes
    public static bool Ramdon { get; set; } = false;
    List<Cita> citasPrioritariaslist = new List<Cita>();//lista uxiliar para dar prioridad cuando se cierra o abre consultorio

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
        try{
           // fitnes1();
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
            // borrrar Pruebas 
            var consultorio = new Consultorios();
            consultorio.RegistrarEspecialidad(1);
            consultorio.RegistrarEspecialidad(2);
            ListaConsultorios.Add(consultorio);
            consultorio.AbrirConsultorio();

            var paciente1 = ListaPaciente.FirstOrDefault();
            var paciente2 = ListaPaciente.Skip(1).FirstOrDefault();
            var especialidad1 = ListaEspecialidad.FirstOrDefault(e => e.IdEspecialidad == 1);
            var especialidad2 = ListaEspecialidad.FirstOrDefault(e => e.IdEspecialidad == 2);

            if (paciente1 != null && especialidad1 != null)
            {
                var cita1 = new Cita(especialidad1, paciente1.IdPaciente);
                consultorio.AgregarCita(cita1);
                paciente1.Citas.Add(cita1);
            }

            if (paciente2 != null && especialidad2 != null)
            {
                var cita2 = new Cita(especialidad2, paciente2.IdPaciente);
                consultorio.AgregarCita(cita2);
                paciente2.Citas.Add(cita2);
            }

            // borrrar Pruebas 

            datosCargados = true;
        }
        ViewBag.Ramdon = Ramdon;    
        ViewBag.Citas = Citas;
        ViewBag.Consultorios = ListaConsultorios;
        return View(ListaPaciente);
    }

   public IActionResult Consultoriosfila()
    {
        var lista = ListaConsultorios; // O donde guardes tus datos
        return PartialView("_Consultoriosfila", lista);
    }

    public IActionResult Citascola()
    {
        var citas = Citas; // O desde donde provengan
        return PartialView("_Citascola", citas);
    }

    public IActionResult MostrarConsultorio(int id)
    {
        var consultorio = ListaConsultorios.FirstOrDefault(c => c.IdConsultorio == id);
        return PartialView("EstadosConsultorios", consultorio);
    }
    public IActionResult MostrarFilas(int id)
    {
        var consultorio = ListaConsultorios.FirstOrDefault(c => c.IdConsultorio == id);
        ViewBag.Pacientes = ListaPaciente;
        Console.WriteLine(id);
        return PartialView("Colafilas", consultorio);
    }

    [HttpPost]
    public IActionResult CerrarConsultorio(int IdConsultorio){

        var consultorio = ListaConsultorios.FirstOrDefault(c => c.IdConsultorio == IdConsultorio);
        consultorio.EstadoConsultorio = false;
        var lista = consultorio.CitasAsignadas;
        consultorio.CitasAsignadas = new List<Cita>();

        List<Cita> todasLasCitas = new List<Cita>();
        foreach (var con in ListaConsultorios)
        {
            todasLasCitas.AddRange(con.CitasAsignadas);
            con.CitasAsignadas = new List<Cita>();
            con.ContarDuracion();
        }

        todasLasCitas.AddRange(lista);

        ReacomodarCitas(todasLasCitas);
        return RedirectToAction("Sala");
    }

    [HttpPost]

    public IActionResult AbrirConsultorio(int IdConsultorio)
    {
        var consultorio = ListaConsultorios.FirstOrDefault(c => c.IdConsultorio == IdConsultorio);
        consultorio.EstadoConsultorio = true;

       
        List<Cita> todasLasCitas = new List<Cita>();
        foreach (var con in ListaConsultorios)
        {
            todasLasCitas.AddRange(con.CitasAsignadas);
            con.CitasAsignadas = new List<Cita>();
            con.ContarDuracion();
        }

        ReacomodarCitas(todasLasCitas);

        return RedirectToAction("Sala");
    }

    [HttpPost]


    [HttpPost]
    public IActionResult AgregarEspecialidad(string nombre, int duracion)
    {
        if (ListaPaciente.Any(p => p.Nombre == nombre))
        {
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
    public IActionResult RamdonConsultorios()
    {
        Ramdon = true ;
        TempData["Mensaje"] = "Se generaron los consultorios ramdon";

        for (int i = 0; i < 5; i++) {
            var nueva = new Consultorios();
            Random random = new();
            for (int y = 0; y < 4; y++)
            {
                int idAleatorio = random.Next(1, ListaEspecialidad.Count + 1);
                nueva.RegistrarEspecialidad(idAleatorio);
                
            }
            ListaConsultorios.Add(nueva);
        }

        return RedirectToAction("Sala");
    }

    public IActionResult EliminarEspecialidadDeConsultorio(int idConsultorio, int idEspecialidad)
    {
        var consultorio = ListaConsultorios.FirstOrDefault(c => c.IdConsultorio == idConsultorio);
        if (consultorio == null)
        {
            TempData["Error"] = "Consultorio no encontrado.";
            return RedirectToAction("Sala");
        }
        var citasPrioritarias = new List<Cita>();
        if (consultorio.EliminarEspecialidad(idEspecialidad))
        {
            TempData["Mensaje"] = "Especialidad eliminada exitosamente.";
            List<Cita> todasLasCitas = new List<Cita>();
            foreach (var con in ListaConsultorios)
            {
                todasLasCitas.AddRange(con.CitasAsignadas);
                con.CitasAsignadas = new List<Cita>();
                con.ContarDuracion();
            }
            ReacomodarCitas(todasLasCitas);

        }
        else
        {
            TempData["Error"] = "La especialidad no estaba registrada en este consultorio.";
        }

        return RedirectToAction("Sala");
    }

    [HttpPost]
    public IActionResult AgregarEspecialidadAConsultorio(int idConsultorio, int idEspecialidad)
    {
        var consultorio = ListaConsultorios.FirstOrDefault(c => c.IdConsultorio == idConsultorio);
        if (consultorio == null)
        {
            TempData["Error"] = "Consultorio no encontrado.";
            return RedirectToAction("Sala");
        }

        if (consultorio.IdEspecialidades.Count >= 5)
        {
            TempData["Error"] = "Este consultorio ya tiene el máximo de especialidades.";
            return RedirectToAction("Sala");
        }

        if (consultorio.RegistrarEspecialidad(idEspecialidad))
        {
            List<Cita> todasLasCitas = new List<Cita>();
            foreach (var con in ListaConsultorios)
            {
                todasLasCitas.AddRange(con.CitasAsignadas);
                con.CitasAsignadas = new List<Cita>();
                con.ContarDuracion();
            }
            ReacomodarCitas(todasLasCitas);
            
            TempData["Mensaje"] = "Especialidad agregada exitosamente.";
        }
        else
        {
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
        ColaCitas.Add(nuevaCita);
        ReacomodarColas();
        return RedirectToAction("Sala");
    }
    private bool Fitnes(Cita cita, List<Consultorios> consultorios)
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

  
    public void ReacomodarCitas(List<Cita> citasPrioritarias)
    {
        citasPrioritarias = citasPrioritarias.OrderBy(c => c.IdCita).ToList();
        foreach (var cita in citasPrioritarias)
        {
            Fitnes(cita, ListaConsultorios);
        }
    }
    public void ReacomodarColas()
    {
        var citasPrioritarias = ColaCitas.OrderBy(c => c.IdCita).ToList();
        foreach (var cita in citasPrioritarias)
        {
            if (Fitnes(cita, ListaConsultorios)){
                 ColaCitas.Remove(cita);
            };
        }
    }
}


       
    
