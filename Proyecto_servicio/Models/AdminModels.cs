using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_servicio.Models
{
    public class UsuarioItem
    {
        public int ID_Usuario { get; set; }
        public string NombreCompleto { get; set; }
        public string Email { get; set; }
    }
    public class TrabajadorItem
    {
        public int ID_Trabajador { get; set; }
        public string NombreCompleto { get; set; }
        public string Email { get; set; }
    }

}
