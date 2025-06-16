using PY_Analisis.Models;
using AGBACKEND;

public class Cita
{
    
    private static int lastIdCita = 0;

    public int IdCita { get; set; }
    public int IdPaciente { get; set; }
    public Especialidad Especialidad { get; set; }
    public bool asignada{ get; set; }
    public int Nprioridad { get; set; }
    public EstadoCita Estado { get; set; } = EstadoCita.EnEspera;

    
    
    

     

    public Cita(Especialidad especialidad, int idPaciente)
    {
        lastIdCita++;
        IdCita = lastIdCita;
        Especialidad = especialidad;
        IdPaciente = idPaciente;
        asignada=false;
        Nprioridad=0;
       Estado = EstadoCita.EnEspera;

      
    }
    public enum EstadoCita
        {
            EnEspera,
            Atendiendo,
            Atendido
        }

    public int ConsultarDuracion()
    {
        return Especialidad.Duracion;
    }
}
