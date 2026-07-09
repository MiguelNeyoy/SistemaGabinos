# Domain Layer Design — Sistema Gabino's

**Date:** 2026-07-08
**Status:** Approved
**Project:** Sistema Gabino's (MVP)
**Architecture:** Clean Architecture (Enfoque A — Repositories in Domain)

---

## 1. Purpose

Define the domain layer (`SistemaGabinos.Domain`) containing enterprise business rules, entities, enums, repository interfaces, and domain exceptions. This layer has zero dependencies on external frameworks or infrastructure.

---

## 2. Entities

### 2.1 Alumno

Represents a student enrolled in the school. If the student is a minor, tutor data is required.

| Property | Type | Notes |
|----------|------|-------|
| Id | int | Primary key |
| NombreCompleto | string | Full name |
| CURP | string | Mexican CURP (unique identifier) |
| FechaNacimiento | DateTime | Date of birth |
| Telefono | string | Contact phone |
| NombreTutor | string? | Tutor's full name (required if minor) |
| ParentescoTutor | string? | Relationship to student (required if minor) |
| TelefonoTutor | string? | Tutor's phone (required if minor) |
| FechaRegistro | DateTime | When registered in the system |
| Estado | EstadoAlumno | Activo / Inactivo |

Behavior:
- `ValidarReglasDeNegocio()` — validates CURP structure, calculates age from FechaNacimiento, and if under 18 enforces that all three tutor fields are non-empty. Throws `TutorObligatorioException` if violated.
- `DarDeBaja()` — sets Estado to Inactivo (soft delete)

**Excluded methods (moved to Application layer):**
- ❌ `ObtenerExpediente()`
- ❌ `ObtenerHistorialPagos()`
- ❌ `ObtenerInscripciones()`

### 2.2 Curso

Represents a level/book in the school (Book 1, Book 2, etc.). There are no traditional school grades — students progress through books. The book cost is charged only when the student passes a level (new students also pay for the book at enrollment).

| Property | Type | Notes |
|----------|------|-------|
| Id | int | Primary key |
| Nombre | string | e.g. "Book 1", "Book 2" |
| PrecioLibro | decimal | Cost of the book when passing this level |

### 2.3 Inscripcion

Represents a student's enrollment in a specific course (book/level). This flow is designed exclusively for **new students** entering the system for the first time. Existing students advancing levels or paying monthly fees do not go through this flow — they are managed directly from the Payment Interface (F9 shortcut).

| Property | Type | Notes |
|----------|------|-------|
| Id | int | Primary key |
| AlumnoId | int | FK to Alumno |
| CursoId | int | FK to Curso |
| FechaInscripcion | DateTime | Enrollment date |
| Estado | EstadoInscripcion | Vigente / Vencida / Cancelada |

Behavior:
- `Activar()` — sets Estado to Vigente
- `Cancelar()` — sets Estado to Cancelada

### 2.4 Deuda

Represents a charge/account receivable that can be paid in multiple installments. Created at enrollment to group partial payments toward the total owed (mensualidad + libro for new students, or libro alone for level advancement).

| Property | Type | Notes |
|----------|------|-------|
| Id | int | Primary key |
| AlumnoId | int | FK to Alumno |
| Concepto | ConceptoDeuda | Inscripcion / Mensualidad / Libro |
| MontoTotal | decimal | Full amount owed |
| FechaCreacion | DateTime | When the debt was created |
| EstaPagada | bool | True when payments cover the total |

### 2.5 Pago

Represents a payment made by a student. A single Deuda can be paid through multiple Pago records. For new students, the initial charge includes mensualidad + libro.

| Property | Type | Notes |
|----------|------|-------|
| Id | int | Primary key |
| AlumnoId | int | FK to Alumno |
| DeudaId | int? | FK to Deuda (nullable — standalone payments possible) |
| Monto | decimal | Payment amount (can be partial toward a Deuda) |
| Fecha | DateTime | Payment date |
| Concepto | ConceptoPago | Mensualidad / Libro |
| MetodoPago | MetodoPago | Efectivo / Transferencia / Tarjeta |
| EstaCancelado | bool | Whether the payment was cancelled/refunded |

### 2.6 Recibo

Receipt generated from a Payment. The receipt holds the data; printing logic (PDF generation, printer communication) belongs to Infrastructure.

| Property | Type | Notes |
|----------|------|-------|
| Id | int | Primary key |
| PagoId | int | FK to Pago |
| Monto | decimal | Same as Pago.Monto (snapshot) |
| FechaEmision | DateTime | When receipt was generated |
| Folio | string | Unique receipt number/folio |
| Detalle | string | Description of what was paid |

---

## 3. Enums

| Enum | Values | Description |
|------|--------|-------------|
| `EstadoAlumno` | Activo, Inactivo | Student status |
| `EstadoInscripcion` | Vigente, Vencida, Cancelada | Enrollment status |
| `ConceptoDeuda` | Inscripcion, Mensualidad, Libro | What the debt is for |
| `ConceptoPago` | Mensualidad, Libro | What the payment is for |
| `MetodoPago` | Efectivo, Transferencia, Tarjeta | How the payment was made |

---

## 4. Repository Interfaces

Contracts without infrastructure coupling. Implementations live in Infrastructure layer.

### IAlumnoRepository
- `ObtenerPorId(int id) : Alumno?`
- `ObtenerPorCURP(string curp) : Alumno?`
- `ObtenerTodos() : List<Alumno>`
- `Guardar(Alumno alumno)`
- `Eliminar(int id)`

### ICursoRepository
- `ObtenerPorId(int id) : Curso?`
- `ObtenerTodos() : List<Curso>`
- `Guardar(Curso curso)`

### IInscripcionRepository
- `ObtenerPorId(int id) : Inscripcion?`
- `ObtenerPorAlumno(int alumnoId) : List<Inscripcion>`
- `Guardar(Inscripcion inscripcion)`

### IDeudaRepository
- `ObtenerPorId(int id) : Deuda?`
- `ObtenerPorAlumno(int alumnoId) : List<Deuda>`
- `Guardar(Deuda deuda)`

### IPagoRepository
- `ObtenerPorId(int id) : Pago?`
- `ObtenerPorAlumno(int alumnoId) : List<Pago>`
- `ObtenerPorDeuda(int deudaId) : List<Pago>`
- `Guardar(Pago pago)`

---

## 5. Domain Exceptions

- `AlumnoYaRegistradoException` — thrown when trying to register a student with an existing CURP
- `AlumnoInactivoException` — thrown when trying to perform actions on an inactive student
- `PagoFueraDeFechaException` — thrown when payment date is invalid
- `TutorObligatorioException` — thrown when a minor student lacks tutor data

---

## 6. Relationships (UML Summary)

```
Alumno  1 ── * Inscripcion   (a student has many enrollments)
Curso   1 ── * Inscripcion   (a course has many enrollments)
Alumno  1 ── * Deuda         (a student has many debts)
Deuda   1 ── * Pago          (a debt can be paid in multiple payments)
Pago    1 ── 0..1 Recibo     (a payment generates zero or one receipt)
```

---

## 7. Architectural Rules

- **Zero external dependencies** in Domain project — only `System.*` and `net10.0` runtime
- **All entities use `class` with private setters** for immutability where appropriate
- **Repository interfaces return nullable types** for single-entity queries (`Alumno?`)
- **Exceptions inherit from `DomainException`** base class
- **No infrastructure, no framework, no EF Core reference** in Domain

---

## 8. Files to Create

All files go under `SistemaGabinos.Domain/`:

```
SistemaGabinos.Domain/
├── Entities/
│   ├── Alumno.cs
│   ├── Curso.cs
│   ├── Inscripcion.cs
│   ├── Deuda.cs
│   ├── Pago.cs
│   └── Recibo.cs
├── Enums/
│   ├── EstadoAlumno.cs
│   ├── EstadoInscripcion.cs
│   ├── ConceptoDeuda.cs
│   ├── ConceptoPago.cs
│   └── MetodoPago.cs
├── Interfaces/
│   ├── IAlumnoRepository.cs
│   ├── ICursoRepository.cs
│   ├── IInscripcionRepository.cs
│   ├── IDeudaRepository.cs
│   └── IPagoRepository.cs
├── Exceptions/
│   ├── DomainException.cs
│   ├── AlumnoYaRegistradoException.cs
│   ├── AlumnoInactivoException.cs
│   ├── PagoFueraDeFechaException.cs
│   └── TutorObligatorioException.cs
└── SistemaGabinos.Domain.csproj
```

---

## 9. Flujo de Inscripción (Application Orchestration)

This section documents the orchestration logic that will live in the Application layer. It is included here as a reference for architectural consistency.

### Paso 1: Validación de Existencia (Anti-Duplicados)
- Tomar CURP ingresada
- `_alumnoRepository.ObtenerPorCURP(curp)`
- Si existe → lanzar `AlumnoYaRegistradoException`

### Paso 2: Instanciación y Validación del Alumno
- Construir `Alumno` con datos personales y de tutor
- Ejecutar `alumno.ValidarReglasDeNegocio()`

### Paso 3: Persistencia de Identidad
- `_alumnoRepository.Guardar(alumno)` → obtener Id generado

### Paso 4: Alta de Inscripción Académica
- Instanciar `Inscripcion` vinculando `AlumnoId` + `CursoId` (libro/nivel inicial)
- `_inscripcionRepository.Guardar(inscripcion)`

### Paso 5: Registro del Cobro Inicial y Emisión de Recibo
- Crear `Deuda` con monto total (mensualidad + libro)
- Registrar `Pago(s)` parciales contra la deuda (según lo que pague el alumno)
- Generar `Recibo` por el monto pagado
- Enviar a Infrastructure para PDF + impresión