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
    
        { if (_timer == null)
        {
           
            _timer = new System.Timers.Timer(5000); // 5 segundos
            _timer.Elapsed += (sender, e) => AtenderPaciente();
            _timer.AutoReset = true;
            _timer.Enabled = true;
            
            
        }

        return View();
    }
    
    public async Task<bool> Cronometro(int milisegundos)
{
    await Task.Delay(milisegundos); 
    return true;
}
 /* This function initializes example data by loading JSON files into the object lists */

   public async Task<IActionResult> Sala()
    {
        if (!datosCargados)
        {
            await CargarPaciente();
            await CargarEspecialidad();
            datosCargados = true;
        }
        ViewBag.Ramdon = Ramdon;    
        ViewBag.ColaCitas = ColaCitas;
        ViewBag.Consultorios = ListaConsultorios;
        return View(ListaPaciente);
    }

    /* This function returns the view to the users  */

   [HttpGet]
    public IActionResult Consultoriosfila()
    {
        return PartialView("_Consultoriosfila", ListaConsultorios);
    }
        /* This function returns the view to the users  */

    [HttpGet]
    public IActionResult Citascola()
    {
        return PartialView("_Citascola", ColaCitas);
    }
        /* This function returns the lists of Medical office  */
    public IActionResult MostrarConsultorio(int id)
    {
        var consultorio = ListaConsultorios.FirstOrDefault(c => c.IdConsultorio == id);
        return PartialView("EstadosConsultorios", consultorio);
    }
     /* This function returns the lists of Medical office  */
    public IActionResult MostrarFilas(int id)
    {
        var consultorio = ListaConsultorios.FirstOrDefault(c => c.IdConsultorio == id);
        ViewBag.Pacientes = ListaPaciente;
        Console.WriteLine(id);
        return PartialView("Colafilas", consultorio);
    }
 /* This function clouse the medical office*/
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
     /* This function open the  Medical office  */
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
    /* This function adds a new specialty to the system.
   It first checks if a patient with the same name exists to avoid conflicts (though this may be a logic mistake).
   Then it creates the specialty with the given name and duration and adds it to the specialty list.
   Finally, it redirects the user to the main room view. */
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
    /* Adds a new patient to the list if the ID is not already registered */
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


    /* This function returns the view to the users  */
    public IActionResult AgregarPacientes()
    {
        return PartialView("AgregarPacientes");
    }
    
    /* This function returns the view to the users  */
    public IActionResult AgregarEspecialidad()
    {
        return PartialView("AgregarEspecialidad");
    }
    /* Shows the current list of patients and appointment queue */
      public IActionResult verCola()
    {
        ViewBag.Pacientes = ListaPaciente;
        ViewBag.citas = ColaCitas;
        return PartialView("vercola");
    }
    
    /* This function returns the view to the users  */
    public IActionResult consultorio()
    {
        return PartialView("consultorio", ListaConsultorios);
    }
    /* Load the list of patients from the JSON file */
    public async Task CargarPaciente()
    {
        string ruta = Path.Combine(Directory.GetCurrentDirectory(), "ArchivosJSON", "Pacientes.json");
        using FileStream leer = System.IO.File.OpenRead(ruta);
        ListaPaciente = await JsonSerializer.DeserializeAsync<List<Paciente>>(leer);
    }
    /* Load the list of specialties from the JSON file */
    public async Task CargarEspecialidad()
    {

        string ruta = Path.Combine(Directory.GetCurrentDirectory(), "ArchivosJSON", "Especialidades.json");
        using FileStream leer = System.IO.File.OpenRead(ruta);
        ListaEspecialidad = await JsonSerializer.DeserializeAsync<List<Especialidad>>(leer);

    }
    /* Create a new consultorio if there is space available */
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
    /* Generate 5 consultorios with random specialties and add them to the list */

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
/* this funcition take the idConsultorio and idEspecialidad search the  Medical office and delete the Specialty */
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
    /* this funcition take the idConsultorio and idEspecialidad search the  Medical office and insert the Specialty */
 
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
    /*this fuctiion takes the idPaciente and idEspeciadlidad and register the new  Schedule an appointment*/
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


        if (Citas.Any(c =>
        c.Especialidad.IdEspecialidad == idEspecialidad &&
        c.IdPaciente == idPaciente &&
        (c.Estado == Cita.EstadoCita.EnEspera || c.Estado == Cita.EstadoCita.Atendiendo)))

        {
            TempData["Error"] = "El paciente ya tiene una cita activa en esta especialidad.";
            return RedirectToAction("Sala");
        }


        var nuevaCita = new Cita(especialidad, idPaciente);
        Citas.Add(nuevaCita);
        paciente.Citas.Add(nuevaCita);
        TempData["Mensaje"] = "Cita agendada exitosamente.";
        ColaCitas.Add(nuevaCita);
       
        return RedirectToAction("Sala");
    }
    /* Iterates through all consultorios (medical offices) and attempts to attend to the next available patient
who is not currently being attended. Each consultorio will only attempt to attend one patient at a time.
*/
   public async void AtenderPaciente()
{
    foreach (var consul in ListaConsultorios)
    {
        if (!consul.Atendiendo && consul.CitasAsignadas.Any())
        {
            const int MAX_INTENTOS = 6;
            int intentos = 0;

            while (intentos < MAX_INTENTOS)
            {
                if (intentos >= consul.CitasAsignadas.Count)
                    break; // Ya no hay más citas que revisar

                var cita = consul.CitasAsignadas[intentos];
                var paciente = ListaPaciente.FirstOrDefault(p => p.IdPaciente == cita.IdPaciente);

                // Si el paciente no está siendo atendido, lo atendemos
                if (paciente != null && paciente.Estado != Paciente.EstadoCita.Atendiendo)
                {
                    consul.Atendiendo = true;
                    paciente.Estado = Paciente.EstadoCita.Atendiendo;
                    cita.Estado = Cita.EstadoCita.Atendiendo;


                    consul.Paciente = cita;
                    consul.CitasAsignadas.Remove(cita);

                    _ = AtenderCitaAsync(consul, paciente, cita);
                    break;
                }

                intentos++;
            }
        }
    }
}

     /*Simulates the process of attending a medical appointment. It delays for the duration of the appointment,
    updates the state of the patient and the consultorio, and frees up the consultorio when done.*/

    private async Task AtenderCitaAsync(Consultorios consul, Paciente paciente, Cita cita)
    {
         
         consul.Paciente=cita;
        consul.CitasAsignadas.Remove(cita);
        await Task.Delay(cita.Especialidad.Duracion * 1000); 
        paciente.Estado = Paciente.EstadoCita.Atendido;
        cita.Estado = Cita.EstadoCita.Atendido;

        consul.Atendiendo = false;
         consul.ContarDuracion();
        
       
    }



/* This is the most important function. It selects the available medical office (consultorio)
 with the minimum current total time (duration) that supports the specialty required by the appointment,
  then attempts to assign the appointment using AgregarCita.*/
    private bool Fitnes(Cita cita, List<Consultorios> consultorios)
{
    if (consultorios.Any(c => c.Paciente == cita || c.CitasAsignadas.Contains(cita)))
        return false;

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

  /**
 Resets all consultorios, keeping only those currently attending a patient.
  Clears all assigned appointments and reassigns unhandled priority appointments
  based on priority and duration using the MejorCola function.
 */
  public void ReacomodarCitas(List<Cita> citasPrioritarias)
{
    // Resetear los consultorios pero conservar los que están atendiendo
    foreach (var consultorio in ListaConsultorios)
    {
        var citaActual = consultorio.Paciente;

        consultorio.CitasAsignadas.Clear();
        consultorio.Duracion = 0;

        if (consultorio.Atendiendo && citaActual != null)
        {
            // Reasignar la cita actual al consultorio
          
            consultorio.Duracion = citaActual.Especialidad.Duracion;
        }
        else
        {
            consultorio.Atendiendo = false;
            consultorio.Paciente = null;
        }
    }

    var citasParaReasignar = citasPrioritarias
        .Where(c => !ListaConsultorios.Any(con => con.Paciente == c))
        .OrderBy(c => c.Nprioridad)
        .ThenBy(c => c.Especialidad.Duracion)
        .ToList();

    MejorCola(citasParaReasignar, 100);
}



    [HttpPost]
    public IActionResult ReacomodarColasinterfas()
    {
        ReacomodarColas();
        return RedirectToAction("Sala");
    }

    /*Sorts the global appointment queue (ColaCitas) by priority and ID, then reassigns the appointments using the MejorCola function.*/
   public void ReacomodarColas()
{
    var citasPrioritarias = ColaCitas
        .OrderBy(c => c.Nprioridad)
        .ThenBy(c => c.IdCita)
        .ToList();

    MejorCola(citasPrioritarias, 100);

    
}
    /**
    Tries multiple randomized permutations of the appointment queue to find the most optimal assignment
    across consultorios (medical offices), minimizing total duration.
    Applies the best configuration to the actual consultorios and increases the priority
    of unassigned appointments for future consideration.
 */

  public void MejorCola(List<Cita> colaOriginal, int intentosMax)
{
    var mejorCola = new List<Cita>(colaOriginal);
    var mejorDuracion = int.MaxValue;
    var mejorAsignacion = new List<Cita>();
    var tiemposSimulaciones = new List<int>(); // Para guardar todas las duraciones

    for (int intentos = 0; intentos < intentosMax; intentos++)
    {
        var colaSimulada = colaOriginal.OrderBy(c => Guid.NewGuid()).ToList();
        var copiaConsultorios = ClonarConsultorios(ListaConsultorios);
        var citasAsignadas = new List<Cita>();

        foreach (var cita in colaSimulada)
        {
            if (copiaConsultorios.Any(c => c.CitasAsignadas.Contains(cita)))
                continue;

            if (Fitnes(cita, copiaConsultorios))
                citasAsignadas.Add(cita);
        }
        var duracionActual = CalcularDuracionGlobal(copiaConsultorios);
        tiemposSimulaciones.Add(duracionActual); // Guardar la duración
        if (duracionActual < mejorDuracion)
        {
            mejorDuracion = duracionActual;
            mejorCola = colaSimulada;
            mejorAsignacion = new List<Cita>(citasAsignadas);
        }
    }

    
    // Aplicar la mejor solución sobre los consultorios reales
    foreach (var cita in mejorAsignacion)
    {
        if (ListaConsultorios.Any(c => c.Paciente == cita))
            continue;

        Fitnes(cita, ListaConsultorios);
        ColaCitas.Remove(cita);
    }

    var noAsignadas = mejorCola.Except(mejorAsignacion).ToList();

    foreach (var cita in noAsignadas)
    {
        if (!ColaCitas.Contains(cita))
        {
            cita.Nprioridad++;
            ColaCitas.Add(cita);
        }
    }
}
    /*Calculates the total duration of all consultorios by summing up their individual durations. Also prints the average duration per consultorio to the console.*/

    private int CalcularDuracionGlobal(List<Consultorios> consultorios)
    {
        int total = consultorios.Sum(c => c.Duracion);
        double promedio = consultorios.Count > 0 ? (double)total / consultorios.Count : 0;

     
        return total;
    }

    /* Creates a deep copy of the list of consultorios for simulation purposes.The cloned consultorios contain the same metadata but with empty appointment lists and reset state (not attending, zero duration).*/
 
    private List<Consultorios> ClonarConsultorios(List<Consultorios> originales)
    {
        var nuevos = new List<Consultorios>();

        foreach (var c in originales)
        {
            var copia = new Consultorios
            {
                IdConsultorio = c.IdConsultorio,
                EstadoConsultorio = c.EstadoConsultorio,
                IdEspecialidades = new List<int>(c.IdEspecialidades),
                CitasAsignadas = new List<Cita>(),
                Atendiendo = false,
                Duracion = 0,
                Paciente = null
            };

            nuevos.Add(copia);
        }

        return nuevos;
    }
}

       
    
