using PY_Analisis.Models;
using AGBACKEND;

public class Cita
{
    
    private static int lastIdCita = 0;

    public int IdCita { get; set; }
    public int IdPaciente { get; set; }
    public Especialidad Especialidad { get; set; }
    public bool asignada{ get; set; }

    public Cita(Especialidad especialidad, int idPaciente)
    {
        lastIdCita++;
        IdCita = lastIdCita;
        Especialidad = especialidad;
        IdPaciente = idPaciente;
        asignada=false;
      
    }

    public int ConsultarDuracion()
    {
        return Especialidad.Duracion;
    }
}
