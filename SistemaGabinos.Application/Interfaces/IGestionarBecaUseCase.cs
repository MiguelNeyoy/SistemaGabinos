namespace SistemaGabinos.Application.Interfaces;

public interface IGestionarBecaUseCase
{
    string AsignarBeca(int alumnoId);
    string QuitarBeca(int alumnoId);
}
