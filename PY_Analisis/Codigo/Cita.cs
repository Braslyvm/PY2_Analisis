using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace AGBACKEND;

    public class Cita
    {
        private static int lastIdCita = 0;

        public int IdCita { get; set; }
        public int? IdConsultorio { get; set; }
        public int IdEspecialidad { get; set; }
        public int IdPaciente { get; set; }
        public Stopwatch TiempoEspera { get; set; }

        public Cita( int idEspecialidad, int idPaciente)
        {
            lastIdCita++;
            IdCita = lastIdCita;
            IdConsultorio = null;
            IdEspecialidad = idEspecialidad;
            IdPaciente = idPaciente;
            TiempoEspera = new Stopwatch();
        }
         public void ReasignarConsultorio(int nuevoIdConsultorio)
        {
            IdConsultorio = nuevoIdConsultorio;
            Console.WriteLine($"La cita {IdCita} ha sido reasignada al consultorio {nuevoIdConsultorio}.");
        }
    }
