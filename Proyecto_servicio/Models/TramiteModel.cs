using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_servicio.Models
{
    public class TramiteModel
    {
        public int ID_Tramite { get; set; }
        public string TipoTramite { get; set; }
        public string Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
    public class TramiteINEModel
    {
        public string CURP { get; set; }
        public byte[] ActaNacimiento { get; set; }
        public byte[] ComprobanteDomicilio { get; set; }
        public byte[] Identificacion { get; set; }
    }


}
