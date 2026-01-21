using Microsoft.Maui.Controls;
using Proyecto_servicio.Paginas;
namespace Proyecto_servicio
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(VisorDocumentoPage), typeof(VisorDocumentoPage));
        }

    }
}
