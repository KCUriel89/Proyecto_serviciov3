using Proyecto_servicio.Helpers;
using Proyecto_servicio.DataBase;
using Proyecto_servicio.Models;
using System.Data; // Add this using directive
using System.Linq; // Add this using directive

namespace Proyecto_servicio.Paginas;

public partial class MisTramitesPage : ContentPage
{
    DatabaseService db = new();

    public MisTramitesPage()
    {
        InitializeComponent();
        Cargar();
    }

    async void Cargar()
    {
        lista.ItemsSource = await db.ObtenerMisTramitesINEAsync(UserSession.IdUsuario);
    }

    private async void OnSeleccionado(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count == 0)
            return;

        var tramite = e.CurrentSelection[0] as TramiteINEItem;

        lista.SelectedItem = null;

        await Navigation.PushAsync(new DetalleTramiteINEPage(tramite.IdTramite));
    }
}
