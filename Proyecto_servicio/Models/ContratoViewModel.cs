using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_servicio.Models
{
    public class ContratoViewModel
    {
        public int ID_Tramite { get; set; }
        public string TipoTramite { get; set; } // Compraventa, Testamento, Sucesion
        public string Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
    }

}
