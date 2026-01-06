using Proyecto_servicio.Helpers;
using Proyecto_servicio.DataBase;
using System.Data; // Add this using directive
using System.Linq; // Add this using directive

namespace Proyecto_servicio.Paginas;

public partial class MisTramitesPage : ContentPage
{
    DatabaseService db = new DatabaseService();
    public MisTramitesPage()
	{
		InitializeComponent();
        CargarTramites();
    }
    private async Task CargarTramites()
    {
        var tramites = await db.GetTramitesUsuarioAsync();
        tramitesCollection.ItemsSource = tramites;
    }


}