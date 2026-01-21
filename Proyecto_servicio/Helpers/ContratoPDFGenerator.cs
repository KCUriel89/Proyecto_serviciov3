using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

public static class ContratoPDFGenerator
{
    public static byte[] GenerarContrato(
     string vendedor,
     string comprador,
     string tipoBien,
     decimal monto)
    {
        var pdf = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);

                // ===== CUERPO DEL CONTRATO =====
                page.Content().Column(column =>
                {
                    column.Item().Text("CONTRATO DE COMPRAVENTA")
                        .FontSize(20)
                        .Bold()
                        .AlignCenter();

                    column.Item().PaddingTop(20).Text($"VENDEDOR: {vendedor}");
                    column.Item().Text($"COMPRADOR: {comprador}");
                    column.Item().Text($"BIEN: {tipoBien}");
                    column.Item().Text($"PRECIO: ${monto:N2}");

                    column.Item().PaddingTop(24).Text(
                        "Ambas partes declaran que celebran el presente contrato de compraventa conforme al Código Civil " +
                        "vigente y aceptan los términos aquí establecidos."
                    );

                    // Esto empuja el footer hacia abajo
                    column.Item().ExtendVertical();
                });

                // ===== ZONA FIJA DE FIRMAS =====
                page.Footer().PaddingBottom(40).Column(column =>
                {
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().AlignCenter().Column(col =>
                        {
                            col.Item().Height(45).BorderBottom(1);
                            col.Item().PaddingTop(5).AlignCenter().Text("Firma del Vendedor");
                        });

                        row.ConstantItem(120); // espacio entre firmas

                        row.RelativeItem().AlignCenter().Column(col =>
                        {
                            col.Item().Height(45).BorderBottom(1);
                            col.Item().PaddingTop(5).AlignCenter().Text("Firma del Comprador");
                        });
                    });

                    column.Item().PaddingTop(20)
                        .AlignCenter()
                        .Text("Contrato generado por el sistema MONEKI")
                        .FontSize(10)
                        .Italic();
                });
            });
        });

        return pdf.GeneratePdf();
    }
}
