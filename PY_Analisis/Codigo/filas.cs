using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGBACKEND;

public class Filas
{
    public bool Estado { get; set; }
    public Consultorios? Consultorio { get; set; }

    public Filas()
    {
        Estado = false;
        Consultorio = null;
    }

    public void CambiarEstado()
    {
        Estado = true;
    }

    public void Cerrar()
    {
        Estado = false;
        Consultorio = null;
    }
}