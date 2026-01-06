using Proyecto_servicio.DataBase;
using Proyecto_servicio.Models;
namespace Proyecto_servicio.Paginas.Admin;

public partial class EliminarCuentasPage : ContentPage
{
    private readonly DatabaseService db = new DatabaseService();

    public EliminarCuentasPage()
    {
        InitializeComponent();
        CargarDatos();
    }

    private async void CargarDatos()
    {
        usuariosCollection.ItemsSource = await db.ObtenerUsuariosAsync();
        trabajadoresCollection.ItemsSource = await db.ObtenerTrabajadoresAsync();
    }

    // ===== ELIMINAR USUARIO =====
    private async void OnEliminarUsuarioClicked(object sender, EventArgs e)
    {
        var usuario = usuariosCollection.SelectedItem as UsuarioItem;
        if (usuario == null)
        {
            await DisplayAlert("Aviso", "Selecciona un usuario.", "OK");
            return;
        }

        bool confirmar = await DisplayAlert(
            "Confirmar",
            $"¿Eliminar al usuario {usuario.NombreCompleto}?",
            "Sí", "No");

        if (!confirmar) return;

        await db.EliminarUsuarioAsync(usuario.ID_Usuario);
        CargarDatos();
    }

    // ===== ELIMINAR TRABAJADOR =====
    private async void OnEliminarTrabajadorClicked(object sender, EventArgs e)
    {
        var trabajador = trabajadoresCollection.SelectedItem as TrabajadorItem;
        if (trabajador == null)
        {
            await DisplayAlert("Aviso", "Selecciona un trabajador.", "OK");
            return;
        }

        bool confirmar = await DisplayAlert(
            "Confirmar",
            $"¿Eliminar al trabajador {trabajador.NombreCompleto}?",
            "Sí", "No");

        if (!confirmar) return;

        await db.EliminarTrabajadorAsync(trabajador.ID_Trabajador);
        CargarDatos();
    }

}