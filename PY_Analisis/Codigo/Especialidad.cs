using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGBACKEND;

    public class Especialidad
    {
        public static int lastIdEspecialidad = 0;
        public int IdEspecialidad { get; set; }
        public string Nombre { get; set; }
       public TimeSpan Duracion { get; set; } 

        public Especialidad(string nombre, TimeSpan duracion)
        {
            lastIdEspecialidad++;
            IdEspecialidad = lastIdEspecialidad;
            Nombre = nombre;
            Duracion = duracion;
        }
    }
