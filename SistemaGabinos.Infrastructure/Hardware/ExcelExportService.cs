// ExcelExportService.cs
// Servicio de exportación de reportes financieros a CSV con BOM UTF-8.
// El BOM garantiza que Microsoft Excel abra el archivo con acentos correctos.
using System.Globalization;
using System.Text;
using SistemaGabinos.Application.DTOs;

namespace SistemaGabinos.Infrastructure.Hardware;

public class ExcelExportService : IExcelExportService
{
    private static readonly byte[] BomUtf8 = [0xEF, 0xBB, 0xBF];

    public byte[] GenerarCsvCorteCaja(CorteCajaDto corteCaja)
    {
        var sb = new StringBuilder();

        // Encabezado de resumen
        sb.AppendLine("CORTE DE CAJA - GABINOS ACADEMY");
        sb.AppendLine($"Periodo:,{corteCaja.FechaInicio:dd/MM/yyyy},{corteCaja.FechaFin:dd/MM/yyyy}");
        sb.AppendLine();
        sb.AppendLine("RESUMEN POR MÉTODO DE PAGO");
        sb.AppendLine($"Efectivo:,${corteCaja.TotalEfectivo:N2}");
        sb.AppendLine($"Tarjeta:,${corteCaja.TotalTarjeta:N2}");
        sb.AppendLine($"Transferencia:,${corteCaja.TotalTransferencia:N2}");
        sb.AppendLine($"TOTAL RECAUDADO:,${corteCaja.TotalRecaudado:N2}");
        sb.AppendLine();
        sb.AppendLine("RESUMEN POR CONCEPTO");
        sb.AppendLine($"Inscripciones:,${corteCaja.TotalInscripciones:N2}");
        sb.AppendLine($"Libros:,${corteCaja.TotalLibros:N2}");
        sb.AppendLine($"Mensualidades:,${corteCaja.TotalMensualidades:N2}");
        sb.AppendLine();
        sb.AppendLine($"Total de Transacciones:,{corteCaja.TotalTransacciones}");
        sb.AppendLine();

        // Tabla detallada de transacciones
        sb.AppendLine("DETALLE DE TRANSACCIONES");
        sb.AppendLine("Folio,Alumno,Concepto,Método de Pago,Monto,Fecha");

        foreach (var pago in corteCaja.Pagos)
        {
            string concepto = pago.Concepto.ToString();
            string metodo = pago.MetodoPago.ToString();
            string monto = pago.Monto.ToString("N2", CultureInfo.InvariantCulture);
            string fecha = pago.Fecha.ToString("dd/MM/yyyy HH:mm");
            string nombreLimpio = pago.NombreAlumno.Replace(",", " ");

            sb.AppendLine($"{pago.Folio},{nombreLimpio},{concepto},{metodo},{monto},{fecha}");
        }

        return ConcatenarBom(sb);
    }

    public byte[] GenerarCsvDeudores(List<AlumnoDeudorDto> deudores, decimal totalGlobal)
    {
        var sb = new StringBuilder();

        sb.AppendLine("CUENTAS POR COBRAR - GABINOS ACADEMY");
        sb.AppendLine($"Fecha de Generación:,{DateTime.Now:dd/MM/yyyy HH:mm}");
        sb.AppendLine($"TOTAL GLOBAL POR COBRAR:,${totalGlobal:N2}");
        sb.AppendLine($"Alumnos Deudores:,{deudores.Count}");
        sb.AppendLine();

        sb.AppendLine("Alumno,CURP,Teléfono,Conceptos Pendientes,Total Pendiente");

        foreach (var deudor in deudores)
        {
            string nombreLimpio = deudor.NombreAlumno.Replace(",", " ");
            string conceptos = deudor.ConceptosPendientesTexto.Replace(",", " /");
            string total = deudor.TotalPendiente.ToString("N2", CultureInfo.InvariantCulture);

            sb.AppendLine($"{nombreLimpio},{deudor.Curp},{deudor.Telefono},{conceptos},{total}");
        }

        return ConcatenarBom(sb);
    }

    private static byte[] ConcatenarBom(StringBuilder sb)
    {
        byte[] contenido = Encoding.UTF8.GetBytes(sb.ToString());
        byte[] resultado = new byte[BomUtf8.Length + contenido.Length];
        Buffer.BlockCopy(BomUtf8, 0, resultado, 0, BomUtf8.Length);
        Buffer.BlockCopy(contenido, 0, resultado, BomUtf8.Length, contenido.Length);
        return resultado;
    }
}
