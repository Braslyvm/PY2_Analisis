using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PY2_Espere_pronto_le_antendemos
{
    public class Cita
    {
        private static int lastIdCita = 0;

        public int IdCita { get; set; }
        public int IdConsultorio { get; set; }
        public int IdEspecialidad { get; set; }
        public int IdPaciente { get; set; }

        public Cita(int idConsultorio, int idEspecialidad, DateTime horaInicio, int idPaciente)
        {
            lastIdCita++;
            IdCita = lastIdCita;
            IdConsultorio = idConsultorio;
            IdEspecialidad = idEspecialidad;
            HoraInicio = horaInicio;
            IdPaciente = idPaciente;
        }
    }
}