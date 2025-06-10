using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using PY_Analisis.Models; 

namespace AGBACKEND;

public class Puerta
{
    public bool Estado { get; set; }
    public Consultorios? Consultorio { get; set; }
    public string Nombre { get; set; }
    public int Duracion { get; set; }
    public Stopwatch TiempoFuncional { get; set; }
    public static List<Cita> CitasF { get; set; } = new List<Cita>();
    public Puerta( string nombre)
    {
        Estado = false;
        Consultorio = null;
        Nombre = nombre;
        TiempoFuncional = new Stopwatch();
        TiempoFuncional.Start();

        
    }

    public void CambiarEstado()
    {
        Estado = true;
        ContarDuracion();
    }

    public void Cerrar()
    {
        Estado = false;
        Consultorio = null;
        Duracion=0;
    }
   public void ContarDuracion()
    {
        if (Consultorio == null || Consultorio.IdEspecialidades == null)
        {
            Duracion = 0;
            return;
        }
        var citasAsociadas = CitasF
            .Where(c => Consultorio.IdEspecialidades.Contains(c.Especialidad.IdEspecialidad))
            .ToList();

        Duracion = citasAsociadas.Sum(c => c.Especialidad.Duracion);
    }
    public bool AgregarCita(Cita nuevaCita)
    {
        
        if (Consultorio == null || Consultorio.IdEspecialidades == null)
            return false;

        if (!Consultorio.IdEspecialidades.Contains(nuevaCita.Especialidad.IdEspecialidad))
            return false;
        CitasF.Add(nuevaCita);
        ContarDuracion();
        return true;
    }
    



}