using Microsoft.Maui.Controls;
using Proyecto_servicio.Paginas;

namespace Proyecto_servicio.Paginas;

public partial class PageCompraventa : ContentPage
{
	public PageCompraventa()
	{
		InitializeComponent();
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