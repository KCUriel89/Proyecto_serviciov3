using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_servicio.Models
{
    public class ContratoCompleto
    {
        public int IdTramite { get; set; }
        public string TipoTramite { get; set; }
        public string Estado { get; set; }
        public string Observaciones { get; set; }
        public DateTime Fecha { get; set; }

        public string Contenido { get; set; }

        public byte[] Documento1 { get; set; }
        public byte[] Documento2 { get; set; }
        public byte[] Documento3 { get; set; }
    }
}
