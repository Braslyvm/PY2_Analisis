using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PY2_Espere_pronto_le_antendemos
{
    public class Paciente
    {
        private static int lastIdPaciente = 0;
        public int IdPaciente { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }

        public List<Cita>? Citas { get; set; }

        public Paciente(string nombre, string apellido)
        {
            lastIdPaciente++;
            IdPaciente = lastIdPaciente;
            Nombre = nombre;
            Apellido = apellido;
             public List<Cita> Citas { get; private set; } = new List<Cita>();
    }
}
}