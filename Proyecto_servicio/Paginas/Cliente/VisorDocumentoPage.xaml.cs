namespace Proyecto_servicio.Paginas;

[QueryProperty(nameof(Data), "data")]
public partial class VisorDocumentoPage : ContentPage
{
    private byte[] _data;

    public byte[] Data
    {
        get => _data;
        set
        {
            _data = value;
            Cargar(value);
        }
    }

    public VisorDocumentoPage()
    {
        InitializeComponent();
    }

    void Cargar(byte[] data)
    {
        if (data == null) return;

        string ext = ObtenerExtension(data);
        string path = Path.Combine(FileSystem.CacheDirectory,
                                   $"doc_{Guid.NewGuid()}{ext}");

        File.WriteAllBytes(path, data);

        if (ext == ".pdf")
        {
            visorPdf.Source = path;
            visorPdf.IsVisible = true;
        }
        else
        {
            visorImagen.Source = path;
            visorImagen.IsVisible = true;
        }
    }

    string ObtenerExtension(byte[] data)
    {
        if (data[0] == 0x25 && data[1] == 0x50) return ".pdf";
        if (data[0] == 0xFF && data[1] == 0xD8) return ".jpg";
        if (data[0] == 0x89 && data[1] == 0x50) return ".png";
        return ".bin";
    }
}
