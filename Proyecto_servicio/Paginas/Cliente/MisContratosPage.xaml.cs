using Proyecto_servicio.Helpers;
using Proyecto_servicio.DataBase;
using Proyecto_servicio.Models;
using System.Data; // Add this using directive
using System.Linq; // Add this using directive

namespace Proyecto_servicio.Paginas;

public partial class MisContratosPage : ContentPage
{
    DatabaseService db = new();

    public MisContratosPage()
    {
        InitializeComponent();
        Cargar();
    }

    async void Cargar()
    {
        lista.ItemsSource = await db.ObtenerMisContratosAsync(UserSession.IdUsuario);
    }

    private async void OnSelect(object sender, SelectionChangedEventArgs e)
    {
        var contrato = e.CurrentSelection.FirstOrDefault() as ContratoViewModel;
        if (contrato == null) return;

        await Navigation.PushAsync(new DetalleContratoPage(contrato.ID_Tramite));
    }
}