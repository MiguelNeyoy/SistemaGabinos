// PdfRenderService.cs
// Implementación de la Capa de Generación de Documentos con QuestPDF.
// Genera un PDF binario real con membrete, tabla de conceptos, totales y firmas.
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace SistemaGabinos.Infrastructure.Hardware;

public class PdfRenderService : IPdfRenderService
{
    public PdfRenderService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] RenderizarReciboPdf(TicketData data)
    {
        string folio = !string.IsNullOrWhiteSpace(data.Folio) ? data.Folio : "REC-OFFLINE";
        string alumno = !string.IsNullOrWhiteSpace(data.NombreAlumno) ? data.NombreAlumno : "Público General";
        string curp = !string.IsNullOrWhiteSpace(data.Curp) ? data.Curp : "N/A";

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.Letter);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(10));

                // ── ENCABEZADO (Membrete) ──
                page.Header().Column(col =>
                {
                    col.Item().AlignCenter().Text("GABINOS ACADEMY").Bold().FontSize(18);
                    col.Item().AlignCenter().Text("COMPROBANTE OFICIAL DE PAGO").FontSize(11).SemiBold();
                    col.Item().PaddingVertical(3).AlignCenter().Text("Av. Principal #123, Col. Centro | Tel: (664) 123-4567").FontSize(8).Light();
                    col.Item().PaddingTop(8).LineHorizontal(1).LineColor(Colors.Grey.Medium);
                });

                // ── CONTENIDO ──
                page.Content().PaddingTop(15).Column(col =>
                {
                    // Datos del alumno y folio
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text(t =>
                            {
                                t.Span("Cliente: ").SemiBold();
                                t.Span(alumno);
                            });
                            c.Item().Text(t =>
                            {
                                t.Span("CURP: ").SemiBold();
                                t.Span(curp);
                            });
                        });
                        row.RelativeItem().AlignRight().Column(c =>
                        {
                            c.Item().Text(t =>
                            {
                                t.Span("Folio: ").SemiBold();
                                t.Span(folio);
                            });
                            c.Item().Text(t =>
                            {
                                t.Span("Fecha: ").SemiBold();
                                t.Span(data.Fecha.ToString("dd/MM/yyyy HH:mm"));
                            });
                        });
                    });

                    col.Item().PaddingVertical(10).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);

                    // Tabla de conceptos
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(4); // Concepto
                            columns.RelativeColumn(2); // Método
                            columns.RelativeColumn(2); // Subtotal
                        });

                        // Encabezado de tabla
                        table.Header(header =>
                        {
                            header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Medium)
                                .Padding(5).Text("CONCEPTO").SemiBold().FontSize(9);
                            header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Medium)
                                .Padding(5).AlignCenter().Text("MÉTODO").SemiBold().FontSize(9);
                            header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Medium)
                                .Padding(5).AlignRight().Text("SUBTOTAL").SemiBold().FontSize(9);
                        });

                        // Filas de datos
                        foreach (var item in data.Items)
                        {
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten3)
                                .Padding(5).Text(item.Concepto).FontSize(9);
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten3)
                                .Padding(5).AlignCenter().Text(data.MetodoPago.ToString()).FontSize(9);
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten3)
                                .Padding(5).AlignRight().Text($"${item.Monto:N2}").FontSize(9);
                        }
                    });

                    // Totales
                    col.Item().PaddingTop(10).AlignRight().Column(totales =>
                    {
                        totales.Item().Row(row =>
                        {
                            row.RelativeItem().AlignRight().Text("TOTAL A PAGAR:").SemiBold();
                            row.ConstantItem(120).AlignRight().Text($"${data.TotalACobrar:N2}").SemiBold();
                        });
                        totales.Item().Row(row =>
                        {
                            row.RelativeItem().AlignRight().Text($"MONTO RECIBIDO ({data.MetodoPago}):");
                            row.ConstantItem(120).AlignRight().Text($"${data.MontoRecibido:N2}");
                        });
                        totales.Item().Row(row =>
                        {
                            row.RelativeItem().AlignRight().Text("CAMBIO ENTREGADO:");
                            row.ConstantItem(120).AlignRight().Text($"${data.Cambio:N2}");
                        });
                    });

                    // Firmas
                    col.Item().PaddingTop(50).Row(row =>
                    {
                        row.RelativeItem().AlignCenter().Column(c =>
                        {
                            c.Item().LineHorizontal(0.5f).LineColor(Colors.Black);
                            c.Item().PaddingTop(3).AlignCenter().Text("Firma de Conformidad").FontSize(8);
                        });
                        row.ConstantItem(60);
                        row.RelativeItem().AlignCenter().Column(c =>
                        {
                            c.Item().LineHorizontal(0.5f).LineColor(Colors.Black);
                            c.Item().PaddingTop(3).AlignCenter().Text("Firma de Caja").FontSize(8);
                        });
                    });
                });

                // ── PIE DE PÁGINA ──
                page.Footer().AlignCenter().Text(t =>
                {
                    t.Span("Este documento es un comprobante de pago oficial emitido por Gabinos Academy.").FontSize(7).Light();
                });
            });
        }).GeneratePdf();
    }
}
