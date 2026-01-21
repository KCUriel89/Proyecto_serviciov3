using Proyecto_servicio.Paginas;

namespace Proyecto_servicio.Paginas;

public partial class PaginaPrincipal : ContentPage
{
    public PaginaPrincipal()
    {
        InitializeComponent();
    }

    private void OnMenuButtonClicked(object sender, EventArgs e)
    {
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
    private async void OnMisTramitesClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MisTramitesPage());
    }
   
           private async void OnMisContratosClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MisContratosPage());
    }
    private async void Imagen_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new InformacionEmpresa());
    }
}
