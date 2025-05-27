using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PY2_Espere_pronto_le_antendemos
{
    public class Especialidad
    {
        private static int lastIdEspecialidad = 0;
        public int IdEspecialidad { get; set; }
        public string Nombre { get; set; }
        public TimeOnly Duracion { get; set; }

        public Especialidad(string nombre, TimeOnly duracion)
        {
            lastIdEspecialidad++;
            IdEspecialidad = lastIdEspecialidad;
            Nombre = nombre;
            Duracion = duracion;
        }
    }
}