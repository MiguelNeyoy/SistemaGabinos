# Diseño: Cobro Inicial Integrado en Nueva Matrícula

> Integrar el cobro inicial dentro del formulario de Nueva Matrícula,
> usando checklist de conceptos con montos automáticos.

---

## Motivación

Actualmente `NuevaMatricula` solo registra al alumno y crea una deuda,
pero no cobra en el momento. El usuario necesita cobrar la inscripción
y el libro al registrar al alumno, en un solo flujo.

## Alcance

**Solo UI** — XAML del formulario y contratos de binding (interfaz pública del ViewModel).
Sin cambios en Domain, Application, Infrastructure.

## Layout de la sección "Cobro Inicial"

Se agrega una segunda Card debajo de la card de datos del alumno.

```
┌─────────────────────────────────────────┐
│  Cobro Inicial                (card)    │
│─────────────────────────────────────────│
│  ┌─── ListBox (CheckBox items) ───────┐ │
│  │ ☑ Inscripción          $ 350.00   │ │  ← Monto editable
│  │ ☑ Libro (Book 1)       $ 350.00   │ │  ← Monto fijo del curso
│  └────────────────────────────────────┘ │
│─────────────────────────────────────────│
│  Total:                     $ 700.00    │
│  Método de Pago:      [Efectivo    ▼]   │
│─────────────────────────────────────────│
│  [  Registrar y Cobrar  ]               │  ← ButtonPrimary
│─────────────────────────────────────────│
│  ✓ Alumno registrado + pago exitoso    │  ← Mensaje éxito (verde)
│  [  Generar Ticket  ]                  │  ← ButtonAccent
└─────────────────────────────────────────┘
```

## Contratos de Binding (ViewModel)

### Propiedades existentes que se mantienen

| Propiedad | Tipo | Binding |
|-----------|------|---------|
| `Curp` | `string` | TextBox.Text |
| `NombreCompleto` | `string` | TextBox.Text |
| `FechaNacimiento` | `DateTime` | DatePicker.SelectedDate |
| `Telefono` | `string` | TextBox.Text |
| `CursoSeleccionado` | `Curso?` | ComboBox.SelectedItem |
| `NombreTutor` | `string?` | TextBox.Text |
| `ParentescoTutor` | `string?` | TextBox.Text |
| `TelefonoTutor` | `string?` | TextBox.Text |
| `VisibleTutor` | `Visibility` | Border.Visibility |
| `Cursos` | `ObservableCollection<Curso>` | ComboBox.ItemsSource |
| `Mensaje` | `string` | TextBlock.Text |

### Nuevas propiedades de cobro

| Propiedad | Tipo | Binding | Notas |
|-----------|------|---------|-------|
| `ConceptosCobro` | `ObservableCollection<ConceptoCobroItem>` | ListBox.ItemsSource | Items chequeables |
| `Total` | `decimal` | TextBlock.Text | Suma automática |
| `MetodoPagoSeleccionado` | `MetodoPago?` | ComboBox.SelectedItem | |
| `MetodosPago` | `ObservableCollection<MetodoPago>` | ComboBox.ItemsSource | Lista de enums |
| `CobroVisible` | `Visibility` | Card.Visibility | Oculta hasta que se seleccione un curso |
| `TicketVisible` | `Visibility` | Button.Visibility | Visible tras registro exitoso |
| `MensajeExitoVisible` | `Visibility` | TextBlock.Visibility | Visible tras registro exitoso |
| `RegistrarCommand` | `IRelayCommand` | Button.Command | Modificado: ahora también procesa pagos |
| `GenerarTicketCommand` | `IRelayCommand` | Button.Command | |

### Clase `ConceptoCobroItem` (modelo para los items del checklist)

```csharp
public class ConceptoCobroItem : ObservableObject
{
    public string Concepto { get; }           // "Inscripción", "Libro"
    public decimal MontoBase { get; }         // Monto original
    public bool MontoEsEditable { get; }      // Solo editable para Inscripción
    public string CursoNombre { get; }        // Para mostrar "Libro (Book 1)"

    [ObservableProperty]
    private bool _seleccionado;               // → notifica cambio, recalcula Total

    [ObservableProperty]
    private decimal _monto;                   // Monto actual (puede editarse)

    public event Action? SeleccionChanged;    // Para que el ViewModel recalcule Total
}
```

## Comportamientos esperados

1. **Al seleccionar un curso** → `CobroVisible = Visible`, se crean los items:
   - `Inscripción` con Monto editable (default $0 o sugerencia)
   - `Libro (Book N)` con Monto = `Curso.PrecioLibro`, no editable
2. **Al marcar/desmarcar un checkbox** → se recalcula `Total`
3. **Al editar el monto de Inscripción** → se recalcula `Total`
4. **Al hacer clic en "Registrar y Cobrar"** → registra alumno + crea pagos por cada concepto marcado
5. **Si éxito** → `Mensaje` verde, `TicketVisible = Visible`, botón "Generar Ticket" aparece
6. **El botón "Generar Ticket"** → imprime/exporta recibo (responsabilidad futura)

## Archivos a modificar

| Archivo | Cambio |
|---------|--------|
| `SistemaGabinos/Views/NuevaMatricula.xaml` | Agregar sección Cobro Inicial con ListBox, Total, Método, botones |
| `SistemaGabinos/ViewModels/NuevaMatriculaViewModel.cs` | Agregar propiedades y comandos de cobro (contratos) |

Sin cambios en otras capas.

## Diseño visual

Aplica los tokens del `DESIGN.md`:
- Card: `BrushSurface`, borde 1px `BrushBorder`, radius 8px
- Label: `LabelMedium`, color `BrushTextMuted` 70%
- Inputs: `InputStyle`
- Botón principal: `ButtonPrimary`
- Botón ticket: `ButtonAccent`
- Mensaje error: `BrushError`, éxito: `BrushPrimary`
