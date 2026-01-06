using Proyecto_servicio.DataBase;
namespace Proyecto_servicio.Paginas.Admin;

public partial class AgregarTrabajadorPage : ContentPage
{
    private readonly DatabaseService db = new();

    public AgregarTrabajadorPage()
    {
        InitializeComponent();
    }
    private async void OnGuardarTrabajadorClicked(object sender, EventArgs e)
    {
        string nombre = entryNombre.Text?.Trim();
        string apP = entryApP.Text?.Trim();
        string apM = entryApM.Text?.Trim();
        string email = entryEmail.Text?.Trim().ToLower();
        string cargo = entryCargo.Text?.Trim();
        string departamento = entryDepartamento.Text?.Trim();
        string password = entryPassword.Text;
        string confirmarPassword = entryConfirmPassword.Text;

        if (string.IsNullOrWhiteSpace(nombre) ||
            string.IsNullOrWhiteSpace(apP) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Error", "Completa todos los campos obligatorios.", "OK");
            return;
        }
        if (password != confirmarPassword)
        {
            await DisplayAlert("Error", "Las contraseñas no coinciden.", "OK");
            return;
        }
        if (await db.TrabajadorExisteAsync(email))
        {
            await DisplayAlert("Error", "Ya existe un trabajador con ese correo.", "OK");
            return;
        }

        await db.InsertarTrabajadorAsync(
            nombre,
            apP,
            apM,
            email,
            cargo,
            departamento,
            password
        );

        await DisplayAlert("Éxito", "Trabajador agregado correctamente.", "OK");
        await Navigation.PopAsync();
    }

    private async void OnCancelarClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}