using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGBACKEND;

    public class Cita
    {
        private static int lastIdCita = 0;

        public int IdCita { get; set; }
        public int IdConsultorio { get; set; }
        public int IdEspecialidad { get; set; }
        public int IdPaciente { get; set; }
        public TimeOnly Duracion { get; set; }

        public Cita(int idConsultorio, int idEspecialidad, int idPaciente)
        {
            lastIdCita++;
            IdCita = lastIdCita;
            IdConsultorio = idConsultorio;
            IdEspecialidad = idEspecialidad;
            IdPaciente = idPaciente;
        }
    }
