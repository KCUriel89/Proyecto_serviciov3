using Org.BouncyCastle.Asn1.Ocsp;
using Proyecto_servicio.DataBase;
using Proyecto_servicio.Models;
namespace Proyecto_servicio.Paginas;

public partial class DetalleTramiteINEPage : ContentPage
{
    private readonly DatabaseService db = new();
    private readonly int idTramite;

    private byte[] acta;
    private byte[] comprobante;
    private byte[] identificacion;

    public DetalleTramiteINEPage(int id)
    {
        InitializeComponent();
        idTramite = id;
        Cargar();
    }

    private async void Cargar()
    {
        var datos = await db.ObtenerINECompleto(idTramite);

        if (datos == null)
        {
            await DisplayAlert("Error", "No se pudo cargar el trámite", "OK");
            return;
        }

        // Información general
        lblTipo.Text = "INE";
        lblEstado.Text = datos.Estado;
        lblId.Text = $"ID: {datos.IdTramite}";
        lblFecha.Text = $"Fecha: {datos.Fecha:dd/MM/yyyy}";
        lblUsuario.Text = $"CURP: {datos.CURP}";

        lblContenido.Text = "Documentos entregados para trámite de INE";

        // Documentos
        acta = datos.ActaNacimiento;
        comprobante = datos.ComprobanteDomicilio;
        identificacion = datos.Identificacion;

        btnDocumento1.IsVisible = acta != null;
        btnDocumento2.IsVisible = comprobante != null;
        btnDocumento3.IsVisible = identificacion != null;

        if (btnDocumento1.IsVisible)
            btnDocumento1.Clicked += async (s, e) =>
                await Shell.Current.GoToAsync(nameof(VisorDocumentoPage),
    new Dictionary<string, object>
    {
        { "data", acta }
    });


        if (btnDocumento1.IsVisible)
            btnDocumento1.Clicked += async (s, e) =>
                await Shell.Current.GoToAsync(nameof(VisorDocumentoPage),
    new Dictionary<string, object>
    {
        { "data", comprobante }
    });

        if (btnDocumento1.IsVisible)
            btnDocumento1.Clicked += async (s, e) =>
               await Shell.Current.GoToAsync(nameof(VisorDocumentoPage),
    new Dictionary<string, object>
    {
        { "data", identificacion }
    });
    }

    private async void AbrirDocumento(byte[] data)
    {
        try
        {
            string ext = ObtenerExtension(data);
            string path = Path.Combine(FileSystem.CacheDirectory,
                                       $"ine_{Guid.NewGuid()}{ext}");

            File.WriteAllBytes(path, data);

            await Launcher.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(path)
            });
        }
        catch
        {
            await DisplayAlert("Error", "No se pudo abrir el archivo", "OK");
        }
    }

    string ObtenerExtension(byte[] data)
    {
        if (data == null || data.Length < 4)
            return ".bin";

        // PDF
        if (data[0] == 0x25 && data[1] == 0x50)
            return ".pdf";

        // JPG
        if (data[0] == 0xFF && data[1] == 0xD8)
            return ".jpg";

        // PNG
        if (data[0] == 0x89 && data[1] == 0x50)
            return ".png";

        return ".bin";
    }

    private async void BtnPdf_Clicked(object sender, EventArgs e)
    {
        if (identificacion == null)
        {
            await DisplayAlert("INE", "No hay identificación disponible", "OK");
            return;
        }

        AbrirDocumento(identificacion);
    }

    private async void BtnImprimir_Clicked(object sender, EventArgs e)
    {
        await DisplayAlert("Impresión", "Los documentos del INE se enviarán a impresión", "OK");
    }
}
