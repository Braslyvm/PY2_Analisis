using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PY_Analisis.Models;

namespace AGBACKEND;

    
   public class Consultorios
{
    private static int lastID = 0;
    private static int ConsultoriosOpen = 0;
    public int IdConsultorio { get; set; }
    public bool EstadoConsultorio { get; set; }
    public List<int>? IdEspecialidades { get; set; }
    public List<Cita> CitasAsignadas { get; set; }
    public int Duracion { get; set; }
    public bool Atendiendo{ get; set; }
    public Cita? Paciente { get; set; }

    public Consultorios()
    {
        lastID++;
        ConsultoriosOpen++;
        IdConsultorio = lastID;
        EstadoConsultorio = false;
        IdEspecialidades = new List<int>();
        CitasAsignadas = new List<Cita>();
        Duracion = 0;
        Atendiendo=false;
        Paciente = null;

    }
      /*
         Recalculates the total duration of assigned appointments
         that match the specialties of the consultory.
         */
    public void ContarDuracion()
    {
        if (IdEspecialidades == null || !CitasAsignadas.Any())
        {
            Duracion = 0;
            return;
        }

        Duracion = CitasAsignadas
            .Where(c => IdEspecialidades.Contains(c.Especialidad.IdEspecialidad))
            .Sum(c => c.Especialidad.Duracion);
    }
       /*
          Attempts to add a new appointment to this consultory.
          The consultory must be open and support the specialty of the appointment.
          
         */
    public bool AgregarCita(Cita nuevaCita)
    {
        if (!EstadoConsultorio || IdEspecialidades == null || 
            !IdEspecialidades.Contains(nuevaCita.Especialidad.IdEspecialidad))
            return false;

        
        if (CitasAsignadas.Contains(nuevaCita))
            return false;

        CitasAsignadas.Add(nuevaCita);
        ContarDuracion();
        return true;
    }

        /*Attempts to close the consultory if more than one consultory remains open.*/
        public bool CerrarConsultorio()
        {
            if (ConsultoriosOpen > 1 && EstadoConsultorio)
            {
                EstadoConsultorio = false;
                ConsultoriosOpen--;
                return true;
            }
            else
            {
                return false;
            }
        }
        /*Attempts to open the consultory if it is currently closed.*/
        public bool AbrirConsultorio()
        {
            if (ConsultoriosOpen >= 1 && !EstadoConsultorio)
            {
                EstadoConsultorio = true;
                ConsultoriosOpen++;
                return true;
            }
            else
            {
                return false;
            }
        }
        
        /* Registers a new specialty to the consultory if not already present.*/
        public bool RegistrarEspecialidad(int IdEspecialidad)
        {
            if (!IdEspecialidades.Contains(IdEspecialidad))
            {
                IdEspecialidades.Add(IdEspecialidad);
                return true;
            }

            return false;
        }
         /**
         * Removes a specialty from the consultory if it exists.*/
        public bool EliminarEspecialidad(int IdEspecialidad)
        {
            if (IdEspecialidades.Contains(IdEspecialidad))
            {
                IdEspecialidades.Remove(IdEspecialidad);
                return true;
            }

            return false;
        }
}