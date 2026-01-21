namespace Proyecto_servicio.Paginas.Trabajador;

public partial class TrabajadorPage : ContentPage
{
	public TrabajadorPage()
	{
		InitializeComponent();
	}
    private async void Imagen_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new InformacionEmpresa());
 
	}
	private async void OnRevisarCitasCuentasClicked(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new RevisarCitasCuentasPage());
    }

	private async void OnRevisionContratosClicked(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new RevisionContratosPage());
    }
}