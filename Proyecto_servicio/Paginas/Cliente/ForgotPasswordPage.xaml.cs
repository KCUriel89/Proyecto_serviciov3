using Proyecto_servicio.Services;
using System.Text.RegularExpressions;
using Proyecto_servicio.DataBase;
namespace Proyecto_servicio.Paginas;
public partial class ForgotPasswordPage : ContentPage
{
    private readonly EmailService _emailService;
    private readonly DatabaseService _db = new DatabaseService();
    public ForgotPasswordPage(EmailService emailService)
    {
        InitializeComponent();
        _emailService = emailService;
    }

    private async void OnEnviarCodigoClicked(object sender, EventArgs e)
    {
        try
        {
            string correo = txtCorreoDestino.Text?.Trim();

            if (string.IsNullOrEmpty(correo))
            {
                await DisplayAlert("Error", "Escribe tu correo", "OK");
                return;
            }

            // ✅ 1. VALIDAR SI EL CORREO EXISTE
            bool existe = await _db.CorreoExiteUsuariosAsync(correo);

            if (!existe)
            {
                await DisplayAlert("Error", "Este correo no está registrado", "OK");
                return; // ⛔ NO ENVÍA CORREO
            }

            // ✅ 2. GENERAR CÓDIGO
            string codigo = new Random().Next(100000, 999999).ToString();

            // ✅ 3. GUARDAR CÓDIGO EN SQL
            await _db.GuardarCodigoRecuperacionAsync(correo, codigo);

            // ✅ 4. ENVIAR CORREO
            await _emailService.EnviarCorreoAsync(
                correo,
                "Recuperación de contraseña",
                $"Tu código de recuperación es: {codigo}"
            );

            await DisplayAlert("Éxito", "El código fue enviado a tu correo", "OK");

            // ✅ 5. IR A SEGUNDA PÁGINA
            await Navigation.PushAsync(new ChangePasswordPage(correo));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

}