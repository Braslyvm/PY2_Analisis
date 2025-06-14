using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGBACKEND;

    public class Paciente
    {
        public static int lastIdPaciente = 0;
        public int IdPaciente { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public int Cedula { get; set; }
        public bool Atencion { get; set; }
        public List<Cita> Citas { get; set; } = new List<Cita>();
        public Paciente.EstadoCita Estado { get; set; } = Paciente.EstadoCita.EnEspera;

        public Paciente(string nombre, string apellido,int cedula) 
        {
            lastIdPaciente++;
            IdPaciente = lastIdPaciente;
            Nombre = nombre;
            Apellido = apellido;
            Cedula= cedula;
            Atencion=false;
        }
        public enum EstadoCita
        {
            EnEspera,
            Atendiendo,
            Atendido
        }
        

      
    }
