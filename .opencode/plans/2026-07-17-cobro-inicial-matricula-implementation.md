# Cobro Inicial en Nueva Matrícula — Implementation Plan

> **For agentic workers:** Use superpowers:subagent-driven-development or superpowers:executing-plans to implement task-by-task. Steps use checkbox (`- [ ]`) syntax.

**Goal:** Agregar sección de cobro inicial con checklist de conceptos al formulario NuevaMatricula (solo XAML + contratos de binding).

**Architecture:** Segunda Card "Cobro Inicial" debajo de la Card de datos del alumno en `NuevaMatricula.xaml`. El ViewModel expone propiedades para alimentar los controles. Sin cambios en Domain/Application/Infrastructure.

**Tech Stack:** WPF .NET 10, CommunityToolkit.Mvvm, Poppins, DESIGN.md tokens.

## Global Constraints

- Sin cambios en Domain, Application, Infrastructure
- Seguir tokens visuales de `SistemaGabinos/Views/DESIGN.md`
- No implementar lógica de negocio — solo XAML y contratos
- `ConceptoCobroItem` debe heredar de `ObservableObject` para notificaciones

---

### Task 1: Crear ConceptoCobroItem e InvertBoolConverter

**Files:**
- Create: `SistemaGabinos/ViewModels/ConceptoCobroItem.cs`
- Create: `SistemaGabinos/Converters/InvertBoolConverter.cs`

**Interfaces:**
- Produces: `ConceptoCobroItem` class with `Seleccionado`, `Monto`, `Concepto`, `MontoEsEditable`, `NombreVisual` properties; `InvertBoolConverter` for XAML binding

- [ ] **Step 1: Crear ConceptoCobroItem.cs**

```csharp
using CommunityToolkit.Mvvm.ComponentModel;

namespace SistemaGabinos.ViewModels;

public partial class ConceptoCobroItem : ObservableObject
{
    public string Concepto { get; }
    public bool MontoEsEditable { get; }
    public string NombreVisual { get; }

    [ObservableProperty]
    private bool _seleccionado;

    [ObservableProperty]
    private decimal _monto;

    public ConceptoCobroItem(string concepto, decimal monto, bool montoEsEditable, string nombreVisual)
    {
        Concepto = concepto;
        _monto = monto;
        MontoEsEditable = montoEsEditable;
        NombreVisual = nombreVisual;
    }
}
```

- [ ] **Step 2: Crear InvertBoolConverter.cs**

```csharp
using System.Globalization;
using System.Windows.Data;

namespace SistemaGabinos.Converters;

public class InvertBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool b ? !b : false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool b ? !b : false;
    }
}
```

---

### Task 2: Registrar converter en App.xaml

**Files:**
- Modify: `SistemaGabinos/App.xaml`

- [ ] **Step 1: Agregar namespace de converters**

En la etiqueta `<Application` de App.xaml, agregar:
```xml
xmlns:converters="clr-namespace:SistemaGabinos.Converters"
```

- [ ] **Step 2: Agregar converter a recursos**

Dentro de `<Application.Resources>` (crear el bloque si no existe):
```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary Source="Resources/Themes/Colors.xaml" />
            <ResourceDictionary Source="Resources/Themes/Typography.xaml" />
            <ResourceDictionary Source="Resources/Themes/Styles.xaml" />
        </ResourceDictionary.MergedDictionaries>
        <converters:InvertBoolConverter x:Key="InvertBoolConverter" />
    </ResourceDictionary>
</Application.Resources>
```

---

### Task 3: Agregar contratos de cobro al ViewModel

**Files:**
- Modify: `SistemaGabinos/ViewModels/NuevaMatriculaViewModel.cs`

**Interfaces:**
- Consumes: `ConceptoCobroItem`, `MetodoPago` enum, `Curso`
- Produces: `ConceptosCobro`, `Total`, `MetodoPagoSeleccionado`, `MetodosPago`, `CobroVisible`, `TicketVisible`, `MensajeExitoVisible`, `MensajeError`, `MensajeErrorVisible`, `GenerarTicketCommand`

- [ ] **Step 1: Agregar nuevos imports al inicio**

```csharp
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SistemaGabinos.Application.DTOs;
using SistemaGabinos.Application.Interfaces;
using SistemaGabinos.Domain.Entities;
using SistemaGabinos.Domain.Enums;
using SistemaGabinos.Domain.Interfaces;
using System.Windows;
```

- [ ] **Step 2: Agregar nuevas propiedades y comandos**

```csharp
[ObservableProperty]
private ObservableCollection<ConceptoCobroItem> _conceptosCobro = new();

[ObservableProperty]
private decimal _total;

[ObservableProperty]
private MetodoPago? _metodoPagoSeleccionado;

[ObservableProperty]
private ObservableCollection<MetodoPago> _metodosPago = new(Enum.GetValues<MetodoPago>());

[ObservableProperty]
private Visibility _cobroVisible = Visibility.Collapsed;

[ObservableProperty]
private Visibility _ticketVisible = Visibility.Collapsed;

[ObservableProperty]
private Visibility _mensajeExitoVisible = Visibility.Collapsed;

[ObservableProperty]
private string? _mensajeError;

[ObservableProperty]
private Visibility _mensajeErrorVisible = Visibility.Collapsed;

[RelayCommand]
private void GenerarTicket()
{
    // TODO: implementar generación de ticket (responsabilidad futura)
}
```

- [ ] **Step 3: Agregar OnCursoSeleccionadoChanged y RecalcularTotal**

```csharp
partial void OnCursoSeleccionadoChanged(Curso? value)
{
    if (value is not null)
    {
        CobroVisible = Visibility.Visible;
        ConceptosCobro.Clear();

        ConceptosCobro.Add(new ConceptoCobroItem(
            "Inscripcion", 0, true, "Inscripción"));

        ConceptosCobro.Add(new ConceptoCobroItem(
            "Libro", value.PrecioLibro, false, $"Libro ({value.Nombre})"));

        SuscribirConceptos();
        RecalcularTotal();
    }
    else
    {
        CobroVisible = Visibility.Collapsed;
        ConceptosCobro.Clear();
        Total = 0;
    }
}

private void RecalcularTotal()
{
    Total = ConceptosCobro.Where(c => c.Seleccionado).Sum(c => c.Monto);
}

private void SuscribirConceptos()
{
    foreach (var item in ConceptosCobro)
    {
        item.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName is nameof(ConceptoCobroItem.Seleccionado) or nameof(ConceptoCobroItem.Monto))
                RecalcularTotal();
        };
    }
}
```

- [ ] **Step 4: Modificar Registrar() para manejar estados de cobro**

```csharp
[RelayCommand]
private void Registrar()
{
    if (CursoSeleccionado is null)
    {
        MensajeError = "Seleccione un curso.";
        MensajeErrorVisible = Visibility.Visible;
        MensajeExitoVisible = Visibility.Collapsed;
        return;
    }

    if (MetodoPagoSeleccionado is null)
    {
        MensajeError = "Seleccione un método de pago.";
        MensajeErrorVisible = Visibility.Visible;
        MensajeExitoVisible = Visibility.Collapsed;
        return;
    }

    if (!ConceptosCobro.Any(c => c.Seleccionado))
    {
        MensajeError = "Seleccione al menos un concepto de cobro.";
        MensajeErrorVisible = Visibility.Visible;
        MensajeExitoVisible = Visibility.Collapsed;
        return;
    }

    try
    {
        var request = new RegistrarAlumnoRequest(
            NombreCompleto, Curp, FechaNacimiento, Telefono,
            NombreTutor, ParentescoTutor, TelefonoTutor,
            CursoSeleccionado.Id, Monto);

        var response = _useCase.Ejecutar(request);
        Mensaje = response.Mensaje;
        MensajeExitoVisible = Visibility.Visible;
        MensajeErrorVisible = Visibility.Collapsed;
        TicketVisible = Visibility.Visible;
    }
    catch (Exception ex)
    {
        MensajeError = ex.Message;
        MensajeErrorVisible = Visibility.Visible;
        MensajeExitoVisible = Visibility.Collapsed;
        TicketVisible = Visibility.Collapsed;
    }
}
```

---

### Task 4: XAML — Agregar sección "Cobro Inicial" a NuevaMatricula.xaml

**Files:**
- Modify: `SistemaGabinos/Views/NuevaMatricula.xaml`

**Interfaces:**
- Consumes: todas las propiedades y comandos de Task 1-3
- Binding a `ConceptosCobro`, `Total`, `MetodoPagoSeleccionado`, `MetodosPago`, `CobroVisible`, `TicketVisible`, `Mensaje`, `MensajeExitoVisible`, `MensajeError`, `MensajeErrorVisible`, `RegistrarCommand`, `GenerarTicketCommand`

- [ ] **Step 1: Agregar Page.Resources**

Después de la línea `mc:Ignorable="d"` (línea 8 actual), antes del cierre `>` de `<Page`, agregar:
```xml
xmlns:converters="clr-namespace:SistemaGabinos.Converters"
```

Y después de la línea `Background="{StaticResource BrushSurfaceAlt}"`:
```xml
<Page.Resources>
    <BooleanToVisibilityConverter x:Key="BoolToVisConverter" />
</Page.Resources>
```

- [ ] **Step 2: Agregar Card "Cobro Inicial" después de la card de datos**

Insertar después del `</Border>` que cierra la card de datos del alumno (línea 84), antes del botón de registro (línea 87):

```xml
<!-- Cobro Inicial Card -->
<Border Style="{StaticResource CardStyle}" Padding="24"
        Margin="0,20,0,0"
        Visibility="{Binding CobroVisible}">
    <StackPanel>
        <TextBlock Text="Cobro Inicial"
                   Style="{StaticResource HeadlineMedium}"
                   Margin="0,0,0,16" />

        <!-- Checklist de conceptos -->
        <Border BorderBrush="{StaticResource BrushBorder}"
                BorderThickness="1"
                CornerRadius="4"
                Padding="0">
            <ListBox ItemsSource="{Binding ConceptosCobro}"
                     BorderThickness="0"
                     Background="Transparent"
                     ScrollViewer.VerticalScrollBarVisibility="Disabled">
                <ListBox.ItemTemplate>
                    <DataTemplate>
                        <Border Padding="12,8"
                                BorderBrush="{StaticResource BrushBorder}"
                                BorderThickness="0,0,0,1">
                            <Grid>
                                <Grid.ColumnDefinitions>
                                    <ColumnDefinition Width="Auto" />
                                    <ColumnDefinition Width="*" />
                                    <ColumnDefinition Width="100" />
                                </Grid.ColumnDefinitions>

                                <CheckBox Grid.Column="0"
                                          IsChecked="{Binding Seleccionado}"
                                          VerticalAlignment="Center"
                                          Margin="0,0,8,0" />

                                <TextBlock Grid.Column="1"
                                           Text="{Binding NombreVisual}"
                                           Style="{StaticResource BodyMedium}"
                                           VerticalAlignment="Center" />

                                <TextBox Grid.Column="2"
                                         Text="{Binding Monto, StringFormat=N2}"
                                         Style="{StaticResource InputStyle}"
                                         IsReadOnly="{Binding MontoEsEditable, Converter={StaticResource InvertBoolConverter}}"
                                         TextAlignment="Right"
                                         Padding="8,4"
                                         FontSize="14" />
                            </Grid>
                        </Border>
                    </DataTemplate>
                </ListBox.ItemTemplate>
            </ListBox>
        </Border>

        <!-- Total -->
        <Grid Margin="0,12,0,0">
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="*" />
                <ColumnDefinition Width="120" />
            </Grid.ColumnDefinitions>
            <TextBlock Grid.Column="0"
                       Text="Total"
                       Style="{StaticResource TitleLarge}"
                       VerticalAlignment="Center" />
            <TextBlock Grid.Column="1"
                       Text="{Binding Total, StringFormat={}${0:N2}}"
                       Style="{StaticResource TitleLarge}"
                       Foreground="{StaticResource BrushPrimary}"
                       TextAlignment="Right"
                       VerticalAlignment="Center" />
        </Grid>

        <!-- Método de Pago -->
        <Grid Margin="0,12,0,0">
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="120" />
                <ColumnDefinition Width="*" />
            </Grid.ColumnDefinitions>
            <TextBlock Grid.Column="0"
                       Text="Método de Pago"
                       Style="{StaticResource LabelMedium}"
                       VerticalAlignment="Center" />
            <ComboBox Grid.Column="1"
                      ItemsSource="{Binding MetodosPago}"
                      SelectedItem="{Binding MetodoPagoSeleccionado}"
                      FontFamily="{StaticResource FontPoppins}"
                      FontSize="14"
                      Padding="12,8" />
        </Grid>
    </StackPanel>
</Border>
```

- [ ] **Step 3: Reemplazar botón y mensajes existentes**

Reemplazar desde la línea 87 hasta el final del archivo con:

```xml
<!-- Submit Button -->
<Button Content="Registrar y Cobrar"
        Style="{StaticResource ButtonPrimary}"
        Command="{Binding RegistrarCommand}"
        Margin="0,20,0,0"
        HorizontalAlignment="Left"
        Width="200" />

<!-- Success Message -->
<StackPanel Margin="0,12,0,0"
            Visibility="{Binding MensajeExitoVisible}">
    <TextBlock Text="{Binding Mensaje}"
               Foreground="{StaticResource BrushPrimary}"
               Style="{StaticResource BodyMedium}"
               TextWrapping="Wrap" />
    <Button Content="Generar Ticket"
            Style="{StaticResource ButtonAccent}"
            Command="{Binding GenerarTicketCommand}"
            Margin="0,8,0,0"
            HorizontalAlignment="Left"
            Width="180"
            Visibility="{Binding TicketVisible}" />
</StackPanel>

<!-- Error Message -->
<TextBlock Text="{Binding MensajeError}"
           Foreground="{StaticResource BrushError}"
           Margin="0,12,0,0"
           Style="{StaticResource BodyMedium}"
           TextWrapping="Wrap"
           Visibility="{Binding MensajeErrorVisible}" />
```
