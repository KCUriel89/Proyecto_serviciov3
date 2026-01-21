using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Plugin.Fingerprint; // ← importante
using QuestPDF.Infrastructure;
namespace Proyecto_servicio
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            // 🔐 Licencia de QuestPDF (OBLIGATORIO)
            QuestPDF.Settings.License = LicenseType.Community;

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("BACKCOUNTRY-Regular.ttf", "BACKCOUNTRY");
                });

            return builder.Build();
        }
    }

}
