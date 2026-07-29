// TicketPrinter.cs
// Servicio de simulación e integración de impresión térmica ESC/POS.
using System.Diagnostics;
using System.Text;

namespace SistemaGabinos.Infrastructure.Hardware;

public class TicketPrinter : ITicketPrinter
{
    public void ImprimirRecibo(TicketData ticket)
    {
        var sb = new StringBuilder();
        sb.AppendLine("==========================================");
        sb.AppendLine("            INSTITUTO GABINOS             ");
        sb.AppendLine("         COMPROBANTE DE COBRO             ");
        sb.AppendLine("==========================================");
        sb.AppendLine($"Fecha: {ticket.Fecha:dd/MM/yyyy HH:mm:ss}");
        if (!string.IsNullOrWhiteSpace(ticket.Folio))
        {
            sb.AppendLine($"Folio: {ticket.Folio}");
        }
        sb.AppendLine($"Alumno: {ticket.NombreAlumno}");
        sb.AppendLine($"CURP:   {ticket.Curp}");
        sb.AppendLine("------------------------------------------");
        sb.AppendLine("CONCEPTOS:");

        foreach (var item in ticket.Items)
        {
            sb.AppendLine($" - {item.Concepto,-28} ${item.Monto,8:N2}");
        }

        sb.AppendLine("------------------------------------------");
        sb.AppendLine($"TOTAL A COBRAR:           ${ticket.TotalACobrar,8:N2}");
        sb.AppendLine($"MONTO RECIBIDO ({ticket.MetodoPago}): ${ticket.MontoRecibido,8:N2}");
        sb.AppendLine($"CAMBIO ENTREGADO:          ${ticket.Cambio,8:N2}");
        sb.AppendLine("==========================================");
        sb.AppendLine("      ¡Gracias por su pago puntual!       ");
        sb.AppendLine("==========================================");

        var ticketFormateado = sb.ToString();

        // Enviar a la salida de depuración y consola (listo para enviar a puerto COM/RAW SPOOL)
        Debug.WriteLine(ticketFormateado);
        Console.WriteLine(ticketFormateado);
    }
}
