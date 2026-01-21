using Proyecto_servicio.Helpers;
using Proyecto_servicio.DataBase;
using Proyecto_servicio.Services;
using Proyecto_servicio.Paginas.Trabajador;
using Proyecto_servicio.Paginas.Admin;
namespace Proyecto_servicio.Paginas
{
    public partial class LoginPage : ContentPage
    {
        private readonly DatabaseService db = new DatabaseService();

        public LoginPage()
        {
            InitializeComponent();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            string email = entryUsuario.Text;
            string password = entryPassword.Text;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Error", "Ingresa email y contraseña.", "OK");
                return;
            }

            // 🔹 USUARIOS
            var user = await db.LoginUsuarioEmailAsync(email, password);
            if (user != null)
            {
                // 🔴 GUARDAR SESIÓN
                UserSession.IdUsuario = (int)user["ID_Usuario"];
                UserSession.Email = (string)user["Email"];
                UserSession.Rol = "Usuario";

                await Navigation.PushAsync(new PaginaPrincipal());
                return;
            }

            // 🔹 ADMINISTRADORES
            var admin = await db.LoginAdminEmailAsync(email, password);
            if (admin != null)
            {
                UserSession.IdUsuario = (int)admin["ID_Administrador"];
                UserSession.Email = (string)admin["Email"];
                UserSession.Rol = "Admin";

                await Navigation.PushAsync(new AdminPage());
                return;
            }


            // 🔹 TRABAJADORES
            var trabajador = await db.LoginTrabajadorEmailAsync(email, password);
            if (trabajador != null)
            {
                // 🔴 GUARDAR SESIÓN
                UserSession.IdUsuario = (int)trabajador["ID_Trabajador"];
                UserSession.Email = (string)trabajador["Email"];
                UserSession.Rol = "Trabajador";

   
                await Navigation.PushAsync(new TrabajadorPage());
                return;
            }

            // ❌ No existe
            await DisplayAlert("Error", "Credenciales incorrectas.", "OK");
        }

        private async void ForgotPasswordClicked(object sender, EventArgs e)
        {
            // Instantiate EmailService and pass it to ForgotPasswordPage constructor
            var emailService = new EmailService();
            var forgotPage = new ForgotPasswordPage(emailService);

            await Navigation.PushAsync(forgotPage);
        }
        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegisterPage());
        }
    }
}