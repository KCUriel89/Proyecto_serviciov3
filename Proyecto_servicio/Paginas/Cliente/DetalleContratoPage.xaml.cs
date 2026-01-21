using Proyecto_servicio.DataBase;
using Proyecto_servicio.Models;
using Proyecto_servicio.Helpers;
namespace Proyecto_servicio.Paginas;

public partial class DetalleContratoPage : ContentPage
{
    private readonly DatabaseService db = new();
    private readonly int idTramite;

    private byte[] doc1;
    private byte[] doc2;
    private byte[] doc3;

    public DetalleContratoPage(int id)
    {
        InitializeComponent();
        idTramite = id;
        Cargar();
    }

    private async void Cargar()
    {
        var datos = await db.ObtenerContratoCompleto(idTramite);

        if (datos == null)
        {
            await DisplayAlert("Error", "No se pudo cargar el trámite", "OK");
            return;
        }

        // Información principal
        lblTipo.Text = datos.TipoTramite;
        lblEstado.Text = datos.Estado;
        lblId.Text = $"ID: {datos.IdTramite}";
        lblFecha.Text = $"Fecha: {datos.Fecha:dd/MM/yyyy}";
        lblUsuario.Text = $"Observaciones: {datos.Observaciones}";

        // Contenido
        lblContenido.Text = datos.Contenido;

        // Documentos
        doc1 = datos.Documento1;
        doc2 = datos.Documento2;
        doc3 = datos.Documento3;

        btnDocumento1.IsVisible = doc1 != null;
        btnDocumento2.IsVisible = doc2 != null;
        btnDocumento3.IsVisible = doc3 != null;

        if (btnDocumento1.IsVisible) btnDocumento1.Clicked += (s, e) => AbrirDocumento(doc1);
        if (btnDocumento2.IsVisible) btnDocumento2.Clicked += (s, e) => AbrirDocumento(doc2);
        if (btnDocumento3.IsVisible) btnDocumento3.Clicked += (s, e) => AbrirDocumento(doc3);
    }

    private async void AbrirDocumento(byte[] data)
    {
        try
        {
            string ext = ObtenerExtension(data);
            string path = Path.Combine(FileSystem.CacheDirectory,
                                       $"doc_{Guid.NewGuid()}{ext}");

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
        if (doc1 == null)
        {
            await DisplayAlert("PDF", "No hay contrato disponible", "OK");
            return;
        }

        AbrirDocumento(doc1);
    }

    private async void BtnImprimir_Clicked(object sender, EventArgs e)
    {
        await DisplayAlert("Impresión", "El documento se enviará a impresión", "OK");
        // Aquí después puedes integrar impresión real
    }
}