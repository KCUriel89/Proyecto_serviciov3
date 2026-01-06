using Proyecto_servicio.DataBase;
using Proyecto_servicio.Helpers;
using Proyecto_servicio.Models;
namespace Proyecto_servicio.Paginas;

public partial class PageINE : ContentPage
{
    private readonly DatabaseService _db = new();

    public PageINE()
	{
		InitializeComponent();
        if (!UserSession.IsLoggedIn)
        {
            DisplayAlert(
                "Sesión",
                "Tu sesión expiró. Inicia sesión nuevamente.",
                "OK");

            Application.Current.MainPage = new NavigationPage(new LoginPage());
            return;
        }
    }
    byte[] acta;
    byte[] comprobante;
    byte[] identificacion;

    private void EntryCURP_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is Entry entry && entry.Text != null)
        {
            entry.Text = entry.Text.ToUpper();
        }
    }

    private void OnMenuButtonClickedINE(object sender, EventArgs e)
    {
        MenuOptions.IsVisible = !MenuOptions.IsVisible;
    }

    private async void OnPagina1Clicked(object sender, EventArgs e)
    {
        MenuOptions.IsVisible = false;
        await Navigation.PushAsync(new PaginaPrincipal());
    }

    private async void OnPagina2Clicked(object sender, EventArgs e)
    {
        MenuOptions.IsVisible = false;
        await Navigation.PushAsync(new PageCompraventa());
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

    private async Task<byte[]> SeleccionarArchivoAsync()
    {
        var result = await FilePicker.Default.PickAsync(new PickOptions
        {
            PickerTitle = "Selecciona un archivo",
            FileTypes = FilePickerFileType.Images
        });

        if (result == null)
            return null;

        using var stream = await result.OpenReadAsync();
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        return ms.ToArray();
    }



    private async void BtnActa_Clicked(object sender, EventArgs e)
    {
        acta = await SeleccionarArchivoAsync();
        await DisplayAlert("OK", "Acta cargada", "Aceptar");
    }

    private async void BtnComprobante_Clicked(object sender, EventArgs e)
    {
        comprobante = await SeleccionarArchivoAsync();
        await DisplayAlert("OK", "Comprobante cargado", "Aceptar");
    }

    private async void BtnIdentificacion_Clicked(object sender, EventArgs e)
    {
        identificacion = await SeleccionarArchivoAsync();
        await DisplayAlert("OK", "Identificación cargada", "Aceptar");
    }


    private async void BtnEnviar_Clicked(object sender, EventArgs e)
    {
        if (acta == null || comprobante == null || identificacion == null)
        {
            await DisplayAlert("Error", "Debes subir todos los documentos", "OK");
            return;
        }

        TramiteINEService service = new TramiteINEService();

        await service.CrearTramiteINEAsync(
            entryCURP.Text,
            acta,
            comprobante,
            identificacion
        );

        await DisplayAlert("Éxito", "Trámite INE registrado correctamente", "OK");
        await Navigation.PopAsync();
    }



}