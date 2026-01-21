using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_servicio.Models
{
    public class TramiteINEItem
    {
        public int IdTramite { get; set; }
        public string CURP { get; set; }
        public string Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
    }

}
