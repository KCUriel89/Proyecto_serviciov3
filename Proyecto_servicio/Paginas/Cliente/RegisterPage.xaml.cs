using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Platform;
using Proyecto_servicio.DataBase;

namespace Proyecto_servicio.Paginas;

public partial class RegisterPage : ContentPage
{
    private readonly DatabaseService db = new DatabaseService();

    public RegisterPage()
    {
        InitializeComponent();

        pickerFechaNacimiento.MaximumDate = DateTime.Now;
        pickerFechaNacimiento.MinimumDate = new DateTime(1950, 1, 1);
    }
    double? _latitud;
    double? _longitud;
    string _direccionAuto;


    private async Task<bool> ObtenerUbicacionGPSAsync()
    {
        try
        {
            // Ajustar precisión según plataforma
            var accuracy = DeviceInfo.Platform == DevicePlatform.WinUI
                ? GeolocationAccuracy.Default  // Windows no suele tener GPS
                : GeolocationAccuracy.High;    // Android puede usar GPS preciso

            var request = new GeolocationRequest(accuracy, TimeSpan.FromSeconds(10));
            var location = await Geolocation.Default.GetLocationAsync(request);

            if (location == null)
            {
                // No se pudo obtener ubicación
                _latitud = null;
                _longitud = null;
                _direccionAuto = null;
                return false;
            }

            _latitud = location.Latitude;
            _longitud = location.Longitude;

            // 🔄 Reverse Geocoding (GPS → Dirección)
            try
            {
                var placemarks = await Geocoding.Default.GetPlacemarksAsync(_latitud.Value, _longitud.Value);
                var place = placemarks?.FirstOrDefault();

                if (place != null)
                {
                    _direccionAuto =
                        $"{place.Thoroughfare} {place.SubThoroughfare}, " +
                        $"{place.Locality}, {place.AdminArea}, {place.PostalCode}, México";
                }
                else
                {
                    // En Windows, usualmente no hay placemarks
                    _direccionAuto = "Ubicación detectada (dirección no disponible)";
                }
            }
            catch
            {
                // Si falla el geocoding, igual mostramos coordenadas
                _direccionAuto = "Ubicación detectada (dirección no disponible)";
            }

            return true;
        }
        catch (PermissionException)
        {
            await DisplayAlert(
                "Permiso denegado",
                "Activa la ubicación del dispositivo y permite que la app acceda a ella.",
                "OK");
            return false;
        }
        catch (FeatureNotSupportedException)
        {
            await DisplayAlert(
                "Función no soportada",
                "Tu dispositivo no soporta geolocalización.",
                "OK");
            return false;
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Error",
                $"No se pudo obtener la ubicación: {ex.Message}",
                "OK");
            return false;
        }
    }



    private async void OnUsarUbicacionClicked(object sender, EventArgs e)
    {
        bool ok = await ObtenerUbicacionGPSAsync();

        if (!ok || _latitud == null || _longitud == null)
        {
            await DisplayAlert(
                "Ubicación no disponible",
                "Activa la ubicación del dispositivo y permite que la app acceda a ella.",
                "OK");
            return;
        }

        // Mostrar mapa siempre que haya coordenadas
        mapWebView.IsVisible = true;
        MostrarMapa(_latitud.Value, _longitud.Value);

        // Mostrar dirección si existe, si no, indicar solo coordenadas
        string mensaje = _direccionAuto ?? $"Ubicación detectada: Lat {_latitud.Value}, Lng {_longitud.Value}";
        await DisplayAlert(
            "Ubicación obtenida",
            mensaje,
            "OK");
    }





    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        // ===== DATOS PERSONALES =====
        string nombre = entryNombre.Text?.Trim();
        string apP = entryApP.Text?.Trim();
        string apM = entryApM.Text?.Trim();
        string email = entryEmail.Text?.Trim().ToLower();
        string telefono = entryTelefono.Text?.Trim();
        DateTime fechaNacimiento = pickerFechaNacimiento.Date;
        DateTime fechaRegistro = DateTime.Now;

        // ===== CONTRASEÑAS =====
        string password = entryPassword.Text;
        string confirmPassword = entryConfirm.Text;

        // ===== VALIDACIONES BÁSICAS =====
        if (string.IsNullOrWhiteSpace(nombre) ||
            string.IsNullOrWhiteSpace(apP) ||
            string.IsNullOrWhiteSpace(apM) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Error", "Completa todos los campos obligatorios.", "OK");
            return;
        }

        if (password != confirmPassword)
        {
            await DisplayAlert("Error", "Las contraseñas no coinciden.", "OK");
            return;
        }

        // ===== VALIDAR UBICACIÓN GPS =====
        if (_latitud == null || _longitud == null)
        {
            await DisplayAlert(
                "Ubicación requerida",
                "Debes presionar 'Usar mi ubicación actual' antes de registrarte.",
                "OK");
            return;
        }

        // ===== VALIDAR USUARIO EXISTENTE =====
        if (await db.UserExistsAsync(email))
        {
            await DisplayAlert("Error", "El correo ya está registrado.", "OK");
            return;
        }

        // ===== REGISTRAR USUARIO =====
        string direccionFinal;

        // Si Windows no pudo obtener dirección, usar coordenadas
        if (!string.IsNullOrWhiteSpace(_direccionAuto))
        {
            direccionFinal = _direccionAuto;
        }
        else
        {
            direccionFinal = $"Lat: {_latitud.Value}, Lng: {_longitud.Value}";
        }

        await db.RegisterUserAsync(
            nombre,
            apP,
            apM,
            email,
            password,
            telefono,
            direccionFinal,
            fechaNacimiento,
            fechaRegistro,
            _latitud.Value,
            _longitud.Value
        );

        await DisplayAlert("Registro exitoso", "Tu cuenta fue creada correctamente.", "OK");
        await Navigation.PopAsync();
    }

    private void MostrarMapa(double lat, double lng)
    {
        string html = $@"
    <html>
    <body style='margin:0;padding:0;'>
        <iframe
            width='100%'
            height='100%'
            frameborder='0'
            style='border:0'
            src='https://www.google.com/maps?q={lat},{lng}&z=16&output=embed'
            allowfullscreen>
        </iframe>
    </body>
    </html>";

        mapWebView.Source = new HtmlWebViewSource
        {
            Html = html
        };
    }


    private async void OnGoLoginClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
