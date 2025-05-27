using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PY2_Espere_pronto_le_antendemos
{
    /// <summary>
    ///
    /// </summary>
    public class Consultorios
    {
        private static int lastID = 0;//contador de consultorios para id automatica
        private static int ConsultoriosOpen = 0;
        public int IdConsultorio { get; set; }
        public bool EstadoConsultorio { get; set; }
        public List<int>? IdEspecialidades { get; set; }

        public Consultorios()
        {
            lastID++;
            ConsultoriosOpen++;
            IdConsultorio = lastID;
            EstadoConsultorio = true;
            IdEspecialidades = new List<int>();
        }

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

        public bool AbrirConsultorio()
        {
            if (ConsultoriosOpen > 1 && !EstadoConsultorio)
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

        public bool RegistrarEspecialidad(int IdEspecialidad)
        {
            if (!IdEspecialidades.Contains(IdEspecialidad))
            {
                IdEspecialidades.Add(IdEspecialidad);
                return true;
            }

            return false;
        }

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
}