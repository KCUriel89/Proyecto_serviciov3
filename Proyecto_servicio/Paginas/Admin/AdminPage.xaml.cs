namespace Proyecto_servicio.Paginas.Admin;

public partial class AdminPage : ContentPage
{
	public AdminPage()
	{
		InitializeComponent();
	}
    private async void OnEliminarCuentasClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new EliminarCuentasPage());
    }

    private async void OnAgregarTrabajadorClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AgregarTrabajadorPage());
    }
    private async void Imagen_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new InformacionEmpresa());
    }
}