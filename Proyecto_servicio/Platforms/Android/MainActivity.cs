using Android.App;
using Android.Content.PM;
using Android.OS;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;

namespace Proyecto_servicio
{
    [Activity(Theme = "@style/Maui.SplashTheme",
              MainLauncher = true,
              ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode |
                                     ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // 🔹 Configuración clave para Plugin.Fingerprint
            // Esto asegura que el plugin sepa cuál es la Activity actual
            CrossFingerprint.SetCurrentActivityResolver(() => this);
        }
    }
}
