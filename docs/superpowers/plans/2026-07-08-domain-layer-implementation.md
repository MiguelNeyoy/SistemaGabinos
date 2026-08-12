# Domain Layer Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Implement the Domain layer (`SistemaGabinos.Domain`) with entities, enums, repository interfaces, and domain exceptions — zero external dependencies.

**Architecture:** Clean Architecture, Enfoque A (Repository interfaces in Domain, implementations in Infrastructure). Domain depends only on `net10.0` runtime and `System.*` namespaces. No EF Core, no frameworks.

**Tech Stack:** .NET 10, C#, Clean Architecture

## Global Constraints

- Target framework: `net10.0`
- Nullable enabled: yes
- ImplicitUsings: enabled
- Zero NuGet packages in Domain project
- All entities use `class` with private setters
- Repository interfaces return nullable types for single-entity queries
- All domain exceptions inherit from `DomainException` base class
- File structure: `Entities/`, `Enums/`, `Interfaces/`, `Exceptions/` subdirectories
- Each file begins with a comment explaining what the file/entity does and its responsibility

---

### Task 1: Create Domain folder structure and base DomainException

**Files:**
- Create: `SistemaGabinos.Domain/Exceptions/DomainException.cs`
- Create: `SistemaGabinos.Domain/Exceptions/AlumnoYaRegistradoException.cs`
- Create: `SistemaGabinos.Domain/Exceptions/AlumnoInactivoException.cs`
- Create: `SistemaGabinos.Domain/Exceptions/TutorObligatorioException.cs`
- Create: `SistemaGabinos.Domain/Exceptions/PagoFueraDeFechaException.cs`

**Interfaces:**
- Produces: `DomainException` base class (abstract, extends `Exception`), and 4 concrete exception classes

- [ ] **Step 1: Create `Exceptions/DomainException.cs`**

```csharp
// DomainException.cs
// Excepción base para todos los errores de la capa de dominio.
// Proporciona un constructor que recibe un mensaje descriptivo.
namespace SistemaGabinos.Domain.Exceptions;

public abstract class DomainException(string message) : Exception(message)
{
}
```

- [ ] **Step 2: Create `Exceptions/AlumnoYaRegistradoException.cs`**

```csharp
// AlumnoYaRegistradoException.cs
// Se lanza cuando se intenta registrar un alumno con una CURP que ya existe en el sistema.
namespace SistemaGabinos.Domain.Exceptions;

public class AlumnoYaRegistradoException(string curp)
    : DomainException($"Ya existe un alumno registrado con la CURP: {curp}")
{
}
```

- [ ] **Step 3: Create `Exceptions/AlumnoInactivoException.cs`**

```csharp
// AlumnoInactivoException.cs
// Se lanza cuando se intenta realizar una acción sobre un alumno en estado Inactivo.
namespace SistemaGabinos.Domain.Exceptions;

public class AlumnoInactivoException(string nombre)
    : DomainException($"El alumno {nombre} se encuentra inactivo y no puede realizar esta acción.")
{
}
```

- [ ] **Step 4: Create `Exceptions/TutorObligatorioException.cs`**

```csharp
// TutorObligatorioException.cs
// Se lanza cuando un alumno menor de 18 años no tiene los datos del tutor completos.
namespace SistemaGabinos.Domain.Exceptions;

public class TutorObligatorioException()
    : DomainException("El alumno es menor de edad. Los datos del tutor (NombreTutor, ParentescoTutor, TelefonoTutor) son obligatorios.")
{
}
```

- [ ] **Step 5: Create `Exceptions/PagoFueraDeFechaException.cs`**

```csharp
// PagoFueraDeFechaException.cs
// Se lanza cuando la fecha de un pago es inválida (ej. fecha futura no permitida).
namespace SistemaGabinos.Domain.Exceptions;

public class PagoFueraDeFechaException()
    : DomainException("La fecha del pago no es válida.")
{
}
```

- [ ] **Step 6: Verify project compiles**

```bash
dotnet build SistemaGabinos.Domain/SistemaGabinos.Domain.csproj
```
Expected: Build succeeded with no errors.

- [ ] **Step 7: Commit**

```bash
git add SistemaGabinos.Domain/Exceptions/ && git commit -m "feat(domain): agregar DomainException base y excepciones de dominio"
```

---

### Task 2: Create Enums

**Files:**
- Create: `SistemaGabinos.Domain/Enums/EstadoAlumno.cs`
- Create: `SistemaGabinos.Domain/Enums/EstadoInscripcion.cs`
- Create: `SistemaGabinos.Domain/Enums/ConceptoDeuda.cs`
- Create: `SistemaGabinos.Domain/Enums/ConceptoPago.cs`
- Create: `SistemaGabinos.Domain/Enums/MetodoPago.cs`

**Interfaces:**
- Produces: 5 enums consumed by entity properties

- [ ] **Step 1: Create `Enums/EstadoAlumno.cs`**

```csharp
// EstadoAlumno.cs
// Define el estado de un alumno en el sistema.
// - Activo: el alumno está registrado y puede realizar operaciones.
// - Inactivo: el alumno fue dado de baja.
namespace SistemaGabinos.Domain.Enums;

public enum EstadoAlumno
{
    Activo,
    Inactivo
}
```

- [ ] **Step 2: Create `Enums/EstadoInscripcion.cs`**

```csharp
// EstadoInscripcion.cs
// Define el estado de una inscripción.
// - Vigente: la inscripción está activa.
// - Vencida: la inscripción ha expirado.
// - Cancelada: la inscripción fue cancelada.
namespace SistemaGabinos.Domain.Enums;

public enum EstadoInscripcion
{
    Vigente,
    Vencida,
    Cancelada
}
```

- [ ] **Step 3: Create `Enums/ConceptoDeuda.cs`**

```csharp
// ConceptoDeuda.cs
// Define los conceptos por los que se puede generar una deuda (cuenta por cobrar).
// - Inscripcion: cargo inicial por inscripción.
// - Mensualidad: cargo mensual recurrente.
// - Libro: cargo por libro/nivel al pasar de nivel.
namespace SistemaGabinos.Domain.Enums;

public enum ConceptoDeuda
{
    Inscripcion,
    Mensualidad,
    Libro
}
```

- [ ] **Step 4: Create `Enums/ConceptoPago.cs`**

```csharp
// ConceptoPago.cs
// Define el concepto de un pago realizado.
// - Mensualidad: pago de la cuota mensual.
// - Libro: pago del libro/nivel.
namespace SistemaGabinos.Domain.Enums;

public enum ConceptoPago
{
    Mensualidad,
    Libro
}
```

- [ ] **Step 5: Create `Enums/MetodoPago.cs`**

```csharp
// MetodoPago.cs
// Define los métodos de pago aceptados.
// - Efectivo: pago en efectivo.
// - Transferencia: pago por transferencia bancaria.
// - Tarjeta: pago con tarjeta de crédito/débito.
namespace SistemaGabinos.Domain.Enums;

public enum MetodoPago
{
    Efectivo,
    Transferencia,
    Tarjeta
}
```

- [ ] **Step 6: Verify project compiles**

```bash
dotnet build SistemaGabinos.Domain/SistemaGabinos.Domain.csproj
```
Expected: Build succeeded.

- [ ] **Step 7: Commit**

```bash
git add SistemaGabinos.Domain/Enums/ && git commit -m "feat(domain): add enums for student, enrollment, debt, payment, and payment method"
```

---

### Task 3: Create Curso, Inscripcion, Recibo entities

**Files:**
- Create: `SistemaGabinos.Domain/Entities/Curso.cs`
- Create: `SistemaGabinos.Domain/Entities/Inscripcion.cs`
- Create: `SistemaGabinos.Domain/Entities/Recibo.cs`

**Interfaces:**
- Produces: `Curso`, `Inscripcion`, `Recibo` classes
- Consumes: `EstadoInscripcion` enum, `ConceptoDeuda` enum

- [ ] **Step 1: Create `Entities/Curso.cs`**

```csharp
// Curso.cs
// Representa un nivel o libro en la escuela (Book 1, Book 2, etc.).
// No hay grados escolares tradicionales — los alumnos avanzan por libros/niveles.
// PrecioLibro es el costo del libro que se cobra al pasar este nivel.
namespace SistemaGabinos.Domain.Entities;

public class Curso
{
    public int Id { get; private set; }
    public string Nombre { get; private set; }
    public decimal PrecioLibro { get; private set; }

    private Curso() { }

    public Curso(string nombre, decimal precioLibro)
    {
        Nombre = nombre;
        PrecioLibro = precioLibro;
    }
}
```

- [ ] **Step 2: Create `Entities/Inscripcion.cs`**

```csharp
// Inscripcion.cs
// Representa la inscripción de un alumno a un curso (libro/nivel) específico.
// Este flujo es solo para alumnos nuevos que entran por primera vez al sistema.
// Los alumnos que ya están registrados y avanzan de nivel no usan este flujo.
using SistemaGabinos.Domain.Enums;

namespace SistemaGabinos.Domain.Entities;

public class Inscripcion
{
    public int Id { get; private set; }
    public int AlumnoId { get; private set; }
    public int CursoId { get; private set; }
    public DateTime FechaInscripcion { get; private set; }
    public EstadoInscripcion Estado { get; private set; }

    private Inscripcion() { }

    public Inscripcion(int alumnoId, int cursoId)
    {
        AlumnoId = alumnoId;
        CursoId = cursoId;
        FechaInscripcion = DateTime.UtcNow;
        Estado = EstadoInscripcion.Vigente;
    }

    public void Activar()
    {
        Estado = EstadoInscripcion.Vigente;
    }

    public void Cancelar()
    {
        Estado = EstadoInscripcion.Cancelada;
    }
}
```

- [ ] **Step 3: Create `Entities/Recibo.cs`**

```csharp
// Recibo.cs
// Comprobante generado a partir de un Pago.
// Contiene los datos del recibo (folio, monto, detalle).
// La impresión del recibo (PDF, impresora) es responsabilidad de Infrastructure.
namespace SistemaGabinos.Domain.Entities;

public class Recibo
{
    public int Id { get; private set; }
    public int PagoId { get; private set; }
    public decimal Monto { get; private set; }
    public DateTime FechaEmision { get; private set; }
    public string Folio { get; private set; }
    public string Detalle { get; private set; }

    private Recibo() { }

    public Recibo(int pagoId, decimal monto, string folio, string detalle)
    {
        PagoId = pagoId;
        Monto = monto;
        FechaEmision = DateTime.UtcNow;
        Folio = folio;
        Detalle = detalle;
    }
}
```

- [ ] **Step 4: Build and verify**

```bash
dotnet build SistemaGabinos.Domain/SistemaGabinos.Domain.csproj
```
Expected: Build succeeded.

- [ ] **Step 5: Commit**

```bash
git add SistemaGabinos.Domain/Entities/Curso.cs SistemaGabinos.Domain/Entities/Inscripcion.cs SistemaGabinos.Domain/Entities/Recibo.cs && git commit -m "feat(domain): add Curso, Inscripcion, Recibo entities"
```

---

### Task 4: Create Alumno entity with tutor support and business rule validation

**Files:**
- Create: `SistemaGabinos.Domain/Entities/Alumno.cs`

**Interfaces:**
- Consumes: `EstadoAlumno` enum, `DomainException`, `TutorObligatorioException`
- Produces: `Alumno` entity with `ValidarReglasDeNegocio()`

- [ ] **Step 1: Create `Entities/Alumno.cs`**

```csharp
// Alumno.cs
// Entidad principal que representa un estudiante registrado en el sistema.
// Si el alumno es menor de 18 años, los datos del tutor (NombreTutor, ParentescoTutor, TelefonoTutor) son obligatorios.
// CURP es un identificador único mexicano.
using SistemaGabinos.Domain.Enums;
using SistemaGabinos.Domain.Exceptions;

namespace SistemaGabinos.Domain.Entities;

public class Alumno
{
    public int Id { get; private set; }
    public string NombreCompleto { get; private set; }
    public string CURP { get; private set; }
    public DateTime FechaNacimiento { get; private set; }
    public string Telefono { get; private set; }
    public string? NombreTutor { get; private set; }
    public string? ParentescoTutor { get; private set; }
    public string? TelefonoTutor { get; private set; }
    public DateTime FechaRegistro { get; private set; }
    public EstadoAlumno Estado { get; private set; }

    private Alumno() { }

    public Alumno(
        string nombreCompleto,
        string curp,
        DateTime fechaNacimiento,
        string telefono,
        string? nombreTutor,
        string? parentescoTutor,
        string? telefonoTutor)
    {
        NombreCompleto = nombreCompleto;
        CURP = curp;
        FechaNacimiento = fechaNacimiento;
        Telefono = telefono;
        NombreTutor = nombreTutor;
        ParentescoTutor = parentescoTutor;
        TelefonoTutor = telefonoTutor;
        FechaRegistro = DateTime.UtcNow;
        Estado = EstadoAlumno.Activo;
    }

    public void ValidarReglasDeNegocio()
    {
        var edad = DateTime.UtcNow.Year - FechaNacimiento.Year;
        if (FechaNacimiento.Date > DateTime.UtcNow.AddYears(-edad))
            edad--;

        if (edad < 18)
        {
            if (string.IsNullOrWhiteSpace(NombreTutor) ||
                string.IsNullOrWhiteSpace(ParentescoTutor) ||
                string.IsNullOrWhiteSpace(TelefonoTutor))
            {
                throw new TutorObligatorioException();
            }
        }
    }

    public void DarDeBaja()
    {
        Estado = EstadoAlumno.Inactivo;
    }
}
```

- [ ] **Step 2: Build**

```bash
dotnet build SistemaGabinos.Domain/SistemaGabinos.Domain.csproj
```

- [ ] **Step 3: Commit**

```bash
git add SistemaGabinos.Domain/Entities/Alumno.cs && git commit -m "feat(domain): add Alumno entity with tutor validation and business rules"
```

---

### Task 5: Create Deuda and Pago entities

**Files:**
- Create: `SistemaGabinos.Domain/Entities/Deuda.cs`
- Create: `SistemaGabinos.Domain/Entities/Pago.cs`

**Interfaces:**
- Consumes: `ConceptoDeuda`, `ConceptoPago`, `MetodoPago` enums
- Produces: `Deuda` (groups partial payments), `Pago` (individual payment against a debt)

- [ ] **Step 1: Create `Entities/Deuda.cs`**

```csharp
// Deuda.cs
// Representa una cuenta por cobrar (deuda) asociada a un alumno.
// Una deuda puede pagarse en múltiples parcialidades (varios Pago registros).
// Concepto indica si es por Inscripcion, Mensualidad o Libro.
using SistemaGabinos.Domain.Enums;

namespace SistemaGabinos.Domain.Entities;

public class Deuda
{
    public int Id { get; private set; }
    public int AlumnoId { get; private set; }
    public ConceptoDeuda Concepto { get; private set; }
    public decimal MontoTotal { get; private set; }
    public DateTime FechaCreacion { get; private set; }
    public bool EstaPagada { get; private set; }

    private Deuda() { }

    public Deuda(int alumnoId, ConceptoDeuda concepto, decimal montoTotal)
    {
        AlumnoId = alumnoId;
        Concepto = concepto;
        MontoTotal = montoTotal;
        FechaCreacion = DateTime.UtcNow;
        EstaPagada = false;
    }

    public void MarcarComoPagada()
    {
        EstaPagada = true;
    }
}
```

- [ ] **Step 2: Create `Entities/Pago.cs`**

```csharp
// Pago.cs
// Representa un pago realizado por un alumno.
// Puede ser un pago completo o parcial vinculado a una Deuda (DeudaId).
// Concepto indica si es Mensualidad o Libro.
// MetodoPago indica cómo se pagó (Efectivo, Transferencia, Tarjeta).
using SistemaGabinos.Domain.Enums;

namespace SistemaGabinos.Domain.Entities;

public class Pago
{
    public int Id { get; private set; }
    public int AlumnoId { get; private set; }
    public int? DeudaId { get; private set; }
    public decimal Monto { get; private set; }
    public DateTime Fecha { get; private set; }
    public ConceptoPago Concepto { get; private set; }
    public MetodoPago MetodoPago { get; private set; }
    public bool EstaCancelado { get; private set; }

    private Pago() { }

    public Pago(int alumnoId, int? deudaId, decimal monto, ConceptoPago concepto, MetodoPago metodoPago)
    {
        AlumnoId = alumnoId;
        DeudaId = deudaId;
        Monto = monto;
        Fecha = DateTime.UtcNow;
        Concepto = concepto;
        MetodoPago = metodoPago;
        EstaCancelado = false;
    }

    public Recibo GenerarRecibo(string folio, string detalle)
    {
        return new Recibo(Id, Monto, folio, detalle);
    }
}
```

- [ ] **Step 3: Build**

```bash
dotnet build SistemaGabinos.Domain/SistemaGabinos.Domain.csproj
```

- [ ] **Step 4: Commit**

```bash
git add SistemaGabinos.Domain/Entities/Deuda.cs SistemaGabinos.Domain/Entities/Pago.cs && git commit -m "feat(domain): add Deuda and Pago entities with partial payment support"
```

---

### Task 6: Create Repository Interfaces

**Files:**
- Create: `SistemaGabinos.Domain/Interfaces/IAlumnoRepository.cs`
- Create: `SistemaGabinos.Domain/Interfaces/ICursoRepository.cs`
- Create: `SistemaGabinos.Domain/Interfaces/IInscripcionRepository.cs`
- Create: `SistemaGabinos.Domain/Interfaces/IDeudaRepository.cs`
- Create: `SistemaGabinos.Domain/Interfaces/IPagoRepository.cs`

**Interfaces:**
- Consumes: `Alumno`, `Curso`, `Inscripcion`, `Deuda`, `Pago` entities
- Produces: 5 repository interfaces consumed by Application and Infrastructure layers

- [ ] **Step 1: Create `Interfaces/IAlumnoRepository.cs`**

```csharp
// IAlumnoRepository.cs
// Contrato para la persistencia de alumnos.
// ObtenerPorCURP es usado en el flujo de inscripción para validar duplicados.
using SistemaGabinos.Domain.Entities;

namespace SistemaGabinos.Domain.Interfaces;

public interface IAlumnoRepository
{
    Alumno? ObtenerPorId(int id);
    Alumno? ObtenerPorCURP(string curp);
    List<Alumno> ObtenerTodos();
    void Guardar(Alumno alumno);
    void Eliminar(int id);
}
```

- [ ] **Step 2: Create `Interfaces/ICursoRepository.cs`**

```csharp
// ICursoRepository.cs
// Contrato para la persistencia de cursos (libros/niveles).
using SistemaGabinos.Domain.Entities;

namespace SistemaGabinos.Domain.Interfaces;

public interface ICursoRepository
{
    Curso? ObtenerPorId(int id);
    List<Curso> ObtenerTodos();
    void Guardar(Curso curso);
}
```

- [ ] **Step 3: Create `Interfaces/IInscripcionRepository.cs`**

```csharp
// IInscripcionRepository.cs
// Contrato para la persistencia de inscripciones.
// ObtenerPorAlumno permite consultar el historial de inscripciones de un alumno.
using SistemaGabinos.Domain.Entities;

namespace SistemaGabinos.Domain.Interfaces;

public interface IInscripcionRepository
{
    Inscripcion? ObtenerPorId(int id);
    List<Inscripcion> ObtenerPorAlumno(int alumnoId);
    void Guardar(Inscripcion inscripcion);
}
```

- [ ] **Step 4: Create `Interfaces/IDeudaRepository.cs`**

```csharp
// IDeudaRepository.cs
// Contrato para la persistencia de deudas (cuentas por cobrar).
// ObtenerPorAlumno permite consultar todas las deudas de un alumno.
using SistemaGabinos.Domain.Entities;

namespace SistemaGabinos.Domain.Interfaces;

public interface IDeudaRepository
{
    Deuda? ObtenerPorId(int id);
    List<Deuda> ObtenerPorAlumno(int alumnoId);
    void Guardar(Deuda deuda);
}
```

- [ ] **Step 5: Create `Interfaces/IPagoRepository.cs`**

```csharp
// IPagoRepository.cs
// Contrato para la persistencia de pagos.
// ObtenerPorAlumno consulta todos los pagos de un alumno.
// ObtenerPorDeuda consulta los pagos parciales asociados a una deuda.
using SistemaGabinos.Domain.Entities;

namespace SistemaGabinos.Domain.Interfaces;

public interface IPagoRepository
{
    Pago? ObtenerPorId(int id);
    List<Pago> ObtenerPorAlumno(int alumnoId);
    List<Pago> ObtenerPorDeuda(int deudaId);
    void Guardar(Pago pago);
}
```

- [ ] **Step 6: Build**

```bash
dotnet build SistemaGabinos.Domain/SistemaGabinos.Domain.csproj
```

- [ ] **Step 7: Commit**

```bash
git add SistemaGabinos.Domain/Interfaces/ && git commit -m "feat(domain): add repository interfaces for Alumno, Curso, Inscripcion, Deuda, Pago"
```

---

### Task 7: Final build and verify

- [ ] **Step 1: Full solution build**

```bash
dotnet build
```
Expected: Build succeeded with 0 warnings.

- [ ] **Step 2: Verify file structure**

```bash
ls -R SistemaGabinos.Domain/
```
Expected output matches the spec's file tree.

- [ ] **Step 3: Commit if any fixes were needed**

```bash
git add -A && git commit -m "chore(domain): final adjustments after full solution build"
```