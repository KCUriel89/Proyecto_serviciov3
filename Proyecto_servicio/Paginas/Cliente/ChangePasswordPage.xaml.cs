using Proyecto_servicio.DataBase;
namespace Proyecto_servicio.Paginas;

public partial class ChangePasswordPage : ContentPage
{
	private readonly string _correo;
	private readonly DatabaseService _db = new DatabaseService();
    public ChangePasswordPage(string correo)
	{
		InitializeComponent();
        _correo = correo;
	}

	private async void OnCambiarPasswordClicked(object sender, EventArgs e)
	{
        try
        {
            string codigo = txtCodigo.Text?.Trim();
            string nuevaPassword = txtNuevaPassword.Text;
            string confirmnuevaPassword = txtConfirmarNuevaPassword.Text;

            if (string.IsNullOrEmpty(codigo) || string.IsNullOrEmpty(nuevaPassword))
            {
                await DisplayAlert("Error", "Completa todos los campos", "OK");
                return;
            }

            // ✅ Validar código
            bool valido = await _db.ValidarCodigoAsync(_correo, codigo);

            if (!valido)
            {
                await DisplayAlert("Error", "Código inválido o ya usado", "OK");
                return;
            }
            if (nuevaPassword != confirmnuevaPassword)
            {
                await DisplayAlert("Error", "Las contraseñas no coinciden.", "OK");
                return;
            }

            // ✅ Cambiar contraseña
            await _db.ActualizarPasswordPorCorreoAsync(_correo, nuevaPassword);

            // ✅ Marcar código como usado
            await _db.MarcarCodigoUsadoAsync(_correo, codigo);

            await DisplayAlert("Éxito", "Contraseña actualizada", "OK");

            // ✅ Regresar al Login
            await Navigation.PopToRootAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }
}