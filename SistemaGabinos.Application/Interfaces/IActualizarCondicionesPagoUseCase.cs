namespace SistemaGabinos.Application.Interfaces;

public interface IActualizarCondicionesPagoUseCase
{
    string Ejecutar(int alumnoId, decimal nuevoCosto, decimal nuevaBeca);
}
