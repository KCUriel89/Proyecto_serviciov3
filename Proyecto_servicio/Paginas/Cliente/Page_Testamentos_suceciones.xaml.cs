namespace Proyecto_servicio.Paginas;

public partial class Page_Testamentos_suceciones : ContentPage
{
	public Page_Testamentos_suceciones()
	{
		InitializeComponent();
	}

    private void OnMenuButtonClickedTestamentos(object sender, EventArgs e)
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
        await Navigation.PushAsync(new PaginaPrincipal());
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