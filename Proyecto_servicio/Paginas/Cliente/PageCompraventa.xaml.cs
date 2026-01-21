using Microsoft.Maui.Controls;
using Proyecto_servicio.Paginas;
using Proyecto_servicio.Helpers;
using Proyecto_servicio.DataBase;
namespace Proyecto_servicio.Paginas;

public partial class PageCompraventa : ContentPage
{
    byte[] contratoFirmado;
    byte[] idVendedor;
    byte[] idComprador;
    byte[] contratoPdfGenerado;
    public PageCompraventa()
    {
        InitializeComponent();

        if (!UserSession.IsLoggedIn)
        {
            Application.Current.MainPage = new NavigationPage(new LoginPage());
        }
    }

    private async Task<byte[]> SeleccionarArchivoAsync()
    {
        var file = await FilePicker.Default.PickAsync();

        if (file == null)
            return null;   // usuario canceló

        using var stream = await file.OpenReadAsync();
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        return ms.ToArray();
    }



    private async void BtnIdVendedor_Clicked(object sender, EventArgs e)
    {
        var archivo = await SeleccionarArchivoAsync();

        if (archivo == null)
        {
            await DisplayAlert("Cancelado", "No se seleccionó ningún archivo", "OK");
            return;
        }

        idVendedor = archivo;
        await DisplayAlert("OK", "Identificación del vendedor cargada", "Aceptar");
    }

    private async void BtnIdComprador_Clicked(object sender, EventArgs e)
    {
        var archivo = await SeleccionarArchivoAsync();

        if (archivo == null)
        {
            await DisplayAlert("Cancelado", "No se seleccionó ningún archivo", "OK");
            return;
        }

        idComprador = archivo;
        await DisplayAlert("OK", "Identificación del comprador cargada", "Aceptar");
    }

    private async void BtnGenerar_Clicked(object sender, EventArgs e)
    {
        if (idVendedor == null || idComprador == null)
        {
            await DisplayAlert("Error", "Debes subir las identificaciones", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(entryTipoBien.Text) ||
            string.IsNullOrWhiteSpace(entryVendedor.Text) ||
            string.IsNullOrWhiteSpace(entryComprador.Text) ||
            string.IsNullOrWhiteSpace(entryMonto.Text))
        {
            await DisplayAlert("Error", "Todos los campos son obligatorios", "OK");
            return;
        }

        decimal monto = decimal.Parse(entryMonto.Text);

        contratoPdfGenerado = ContratoPDFGenerator.GenerarContrato(
            entryVendedor.Text,
            entryComprador.Text,
            entryTipoBien.Text,
            monto
        );

        TramiteCompraventaService service = new TramiteCompraventaService();

        await service.CrearTramiteCompraventaAsync(
            entryTipoBien.Text,
            entryVendedor.Text,
            entryComprador.Text,
            monto,
            contratoPdfGenerado,
            contratoFirmado,
            idVendedor,
            idComprador
        );

        await DisplayAlert("Éxito", "Contrato y trámite guardados", "OK");
    }

    private async void BtnImprimir_Clicked(object sender, EventArgs e)
    {
        if (contratoPdfGenerado == null)
        {
            await DisplayAlert("Error", "Primero genera el contrato", "OK");
            return;
        }

        var path = Path.Combine(FileSystem.CacheDirectory, "contrato.pdf");
        File.WriteAllBytes(path, contratoPdfGenerado);

        await Launcher.OpenAsync(new OpenFileRequest
        {
            File = new ReadOnlyFile(path)
        });
    }


    // Add this event handler to match the XAML
    private void OnMenuButtonClickedCV(object sender, EventArgs e)
    {
        // Toggle menu visibility
        MenuOptions.IsVisible = !MenuOptions.IsVisible;
    }

    private async void OnPagina1Clicked(object sender, EventArgs e)
    {
        MenuOptions.IsVisible = false;
        await Navigation.PushAsync(new PageINE());
    }

    private async void OnPagina2Clicked(object sender, EventArgs e)
    {
        MenuOptions.IsVisible = false;
        await Navigation.PushAsync(new PaginaPrincipal());
    }

    private async void OnPagina3Clicked(object sender, EventArgs e)
    {
        MenuOptions.IsVisible = false;
        await Navigation.PushAsync(new Page_Testamentos_suceciones());
    }
    private async void OnCerrarSesionClicked(object sender, EventArgs e)
    {
        // Limpiar datos de sesión (ejemplo usando Microsoft.Maui.Storage)
        try
        {
            Preferences.Clear();
        }
        catch
        {
            // Manejo de errores opcional
        }

        await Navigation.PopToRootAsync();

    }




}