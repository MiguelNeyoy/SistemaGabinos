namespace SistemaGabinos.Application.Interfaces;

public interface ICambiarEstadoAlumnoUseCase
{
    string DarDeBaja(int alumnoId);
    string Reactivar(int alumnoId);
}
