using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_servicio.Models
{
    public class TramiteINEViewModel
    {
        public int ID_Tramite { get; set; }
        public string CURP { get; set; }
        public string Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
    public class INECompleto
    {
        public int IdTramite { get; set; }
        public string CURP { get; set; }
        public string Estado { get; set; }
        public DateTime Fecha { get; set; }

        public byte[] ActaNacimiento { get; set; }
        public byte[] ComprobanteDomicilio { get; set; }
        public byte[] Identificacion { get; set; }
    }


}
