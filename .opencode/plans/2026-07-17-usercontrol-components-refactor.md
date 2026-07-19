# Refactor a Componentes UserControl — Plan de Implementación

**Goal:** Refactor `NuevaMatricula.xaml` de ~300 líneas a ~50 líneas creando componentes reutilizables (UserControls con DependencyProperties), similar a componentes de React con props.

**Architecture:** Cada componente es un par `.xaml` + `.xaml.cs` en carpeta `SistemaGabinos/Controls/`. Se registran con namespace `xmlns:controls="clr-namespace:SistemaGabinos.Controls"`.

**Tech Stack:** WPF .NET 10, DependencyProperties, binding con `RelativeSource`.

## Global Constraints

- Sin lógica de negocio — solo UI
- Componentes deben ser reutilizables (no acoplados a NuevaMatricula)
- Seguir tokens de DESIGN.md
- Binding via `DependencyProperty` + `{Binding ..., RelativeSource={RelativeSource AncestorType=UserControl}}`

---

### Task 1: Crear carpeta y namespace `Controls/`

**Files:**
- Create: `SistemaGabinos/Controls/` (carpeta)

- [ ] **Step 1: Crear carpeta**

```
New-Item -ItemType Directory -Path "SistemaGabinos/Controls"
```

---

### Task 2: Componente `FormField` (label + TextBox)

**Files:**
- Create: `SistemaGabinos/Controls/FormField.xaml`
- Create: `SistemaGabinos/Controls/FormField.xaml.cs`

**Props (DependencyProperties):**

| Prop | Tipo | Default | Binding target |
|------|------|---------|---------------|
| `Label` | `string` | `""` | TextBlock.Text |
| `Text` | `string` | `""` | TextBox.Text |
| `MaxLength` | `int` | `0` | TextBox.MaxLength |

**Uso:**
```xml
<controls:FormField Label="Nombre Completo" Text="{Binding NombreCompleto}" />
<controls:FormField Label="CURP" Text="{Binding Curp}" MaxLength="18" />
```

**FormField.xaml:**
```xml
<UserControl x:Class="SistemaGabinos.Controls.FormField"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <StackPanel>
        <TextBlock Text="{Binding Label, RelativeSource={RelativeSource AncestorType=UserControl}}"
                   Style="{StaticResource LabelMedium}"
                   Margin="0,0,0,4" />
        <TextBox Text="{Binding Text, RelativeSource={RelativeSource AncestorType=UserControl}, UpdateSourceTrigger=PropertyChanged}"
                 Style="{StaticResource InputStyle}"
                 MaxLength="{Binding MaxLength, RelativeSource={RelativeSource AncestorType=UserControl}}" />
    </StackPanel>
</UserControl>
```

**FormField.xaml.cs:**
```csharp
using System.Windows;
using System.Windows.Controls;

namespace SistemaGabinos.Controls;

public partial class FormField : UserControl
{
    public static readonly DependencyProperty LabelProperty =
        DependencyProperty.Register(nameof(Label), typeof(string), typeof(FormField), new PropertyMetadata(""));

    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(FormField), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly DependencyProperty MaxLengthProperty =
        DependencyProperty.Register(nameof(MaxLength), typeof(int), typeof(FormField), new PropertyMetadata(0));

    public int MaxLength
    {
        get => (int)GetValue(MaxLengthProperty);
        set => SetValue(MaxLengthProperty, value);
    }

    public FormField()
    {
        InitializeComponent();
    }
}
```

---

### Task 3: Componente `IconInput` (label + icon + TextBox)

**Files:**
- Create: `SistemaGabinos/Controls/IconInput.xaml`
- Create: `SistemaGabinos/Controls/IconInput.xaml.cs`

**Props:**

| Prop | Tipo | Default |
|------|------|---------|
| `Label` | `string` | `""` |
| `Text` | `string` | `""` |
| `Icon` | `string` | `""` |
| `MaxLength` | `int` | `0` |

**Uso:**
```xml
<controls:IconInput Label="CURP (Obligatorio)" Icon="✅" Text="{Binding Curp}" MaxLength="18" />
<controls:IconInput Label="Teléfono de Contacto" Icon="📞" Text="{Binding Telefono}" />
```

**IconInput.xaml:**
```xml
<UserControl x:Class="SistemaGabinos.Controls.IconInput"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <StackPanel>
        <TextBlock Text="{Binding Label, RelativeSource={RelativeSource AncestorType=UserControl}}"
                   Style="{StaticResource LabelMedium}"
                   Margin="0,0,0,4" />
        <Grid>
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="Auto" />
                <ColumnDefinition Width="*" />
            </Grid.ColumnDefinitions>
            <TextBlock Grid.Column="0"
                       Text="{Binding Icon, RelativeSource={RelativeSource AncestorType=UserControl}}"
                       VerticalAlignment="Center"
                       Margin="0,0,8,0" />
            <TextBox Grid.Column="1"
                     Text="{Binding Text, RelativeSource={RelativeSource AncestorType=UserControl}, UpdateSourceTrigger=PropertyChanged}"
                     Style="{StaticResource InputStyle}"
                     MaxLength="{Binding MaxLength, RelativeSource={RelativeSource AncestorType=UserControl}}" />
        </Grid>
    </StackPanel>
</UserControl>
```

**IconInput.xaml.cs:**
```csharp
using System.Windows;
using System.Windows.Controls;

namespace SistemaGabinos.Controls;

public partial class IconInput : UserControl
{
    public static readonly DependencyProperty LabelProperty =
        DependencyProperty.Register(nameof(Label), typeof(string), typeof(IconInput), new PropertyMetadata(""));
    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(IconInput), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(nameof(Icon), typeof(string), typeof(IconInput), new PropertyMetadata(""));
    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly DependencyProperty MaxLengthProperty =
        DependencyProperty.Register(nameof(MaxLength), typeof(int), typeof(IconInput), new PropertyMetadata(0));
    public int MaxLength
    {
        get => (int)GetValue(MaxLengthProperty);
        set => SetValue(MaxLengthProperty, value);
    }

    public IconInput()
    {
        InitializeComponent();
    }
}
```

---

### Task 4: Componente `CardSection` (Card con título + contenido slot)

**Files:**
- Create: `SistemaGabinos/Controls/CardSection.xaml`
- Create: `SistemaGabinos/Controls/CardSection.xaml.cs`

**Props:**

| Prop | Tipo | Default |
|------|------|---------|
| `Title` | `string` | `""` |

**Content:** usa `ContentPresenter` para el slot (todo lo que pongas dentro del tag).

**Uso:**
```xml
<controls:CardSection Title="Datos del Alumno">
    <StackPanel>
        <controls:IconInput ... />
        <controls:FormField ... />
    </StackPanel>
</controls:CardSection>
```

**CardSection.xaml:**
```xml
<UserControl x:Class="SistemaGabinos.Controls.CardSection"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Border Style="{StaticResource CardStyle}" Padding="24">
        <StackPanel>
            <TextBlock Text="{Binding Title, RelativeSource={RelativeSource AncestorType=UserControl}}"
                       Style="{StaticResource HeadlineMedium}"
                       Margin="0,0,0,16" />
            <ContentPresenter />
        </StackPanel>
    </Border>
</UserControl>
```

**CardSection.xaml.cs:**
```csharp
using System.Windows;
using System.Windows.Controls;

namespace SistemaGabinos.Controls;

public partial class CardSection : UserControl
{
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(CardSection), new PropertyMetadata(""));
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public CardSection()
    {
        InitializeComponent();
    }
}
```

---

### Task 5: Componente `ChecklistBox` (lista chequeable + total)

**Files:**
- Create: `SistemaGabinos/Controls/ChecklistBox.xaml`
- Create: `SistemaGabinos/Controls/ChecklistBox.xaml.cs`

**Props:**

| Prop | Tipo | Default |
|------|------|---------|
| `ItemsSource` | `IEnumerable` | `null` |
| `Total` | `decimal` | `0` |
| `TotalFormat` | `string` | `"${0:N2}"` |

**Uso:**
```xml
<controls:ChecklistBox ItemsSource="{Binding ConceptosCobro}"
                       Total="{Binding Total}" />
```

Requiere que `ConceptoCobroItem` tenga las propiedades `Seleccionado`, `NombreVisual`, `Monto`.

**ChecklistBox.xaml:**
```xml
<UserControl x:Class="SistemaGabinos.Controls.ChecklistBox"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <StackPanel>
        <Border BorderBrush="{StaticResource BrushBorder}"
                BorderThickness="1"
                CornerRadius="4"
                Padding="0">
            <ListBox ItemsSource="{Binding ItemsSource, RelativeSource={RelativeSource AncestorType=UserControl}}"
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
                                    <ColumnDefinition Width="80" />
                                </Grid.ColumnDefinitions>
                                <CheckBox Grid.Column="0"
                                          IsChecked="{Binding Seleccionado}"
                                          VerticalAlignment="Center"
                                          Margin="0,0,8,0" />
                                <TextBlock Grid.Column="1"
                                           Text="{Binding NombreVisual}"
                                           Style="{StaticResource BodyMedium}"
                                           VerticalAlignment="Center" />
                                <TextBlock Grid.Column="2"
                                           Text="{Binding Monto, StringFormat=N2}"
                                           Style="{StaticResource BodyMedium}"
                                           FontWeight="SemiBold"
                                           TextAlignment="Right"
                                           VerticalAlignment="Center" />
                            </Grid>
                        </Border>
                    </DataTemplate>
                </ListBox.ItemTemplate>
            </ListBox>
        </Border>
        <Grid Margin="0,12,0,0">
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="*" />
                <ColumnDefinition Width="80" />
            </Grid.ColumnDefinitions>
            <TextBlock Grid.Column="0"
                       Text="Total"
                       Style="{StaticResource TitleLarge}"
                       VerticalAlignment="Center" />
            <TextBlock Grid.Column="1"
                       Text="{Binding Total, RelativeSource={RelativeSource AncestorType=UserControl}, StringFormat={}${0:N2}}"
                       Style="{StaticResource TitleLarge}"
                       Foreground="{StaticResource BrushPrimary}"
                       TextAlignment="Right"
                       VerticalAlignment="Center" />
        </Grid>
    </StackPanel>
</UserControl>
```

**ChecklistBox.xaml.cs:**
```csharp
using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace SistemaGabinos.Controls;

public partial class ChecklistBox : UserControl
{
    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(ChecklistBox));
    public IEnumerable ItemsSource
    {
        get => (IEnumerable)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public static readonly DependencyProperty TotalProperty =
        DependencyProperty.Register(nameof(Total), typeof(decimal), typeof(ChecklistBox), new FrameworkPropertyMetadata(0m, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    public decimal Total
    {
        get => (decimal)GetValue(TotalProperty);
        set => SetValue(TotalProperty, value);
    }

    public ChecklistBox()
    {
        InitializeComponent();
    }
}
```

---

### Task 6: Componente `ToggleGroup` (dos botones toggle)

**Files:**
- Create: `SistemaGabinos/Controls/ToggleGroup.xaml`
- Create: `SistemaGabinos/Controls/ToggleGroup.xaml.cs`

**Props:**

| Prop | Tipo | Default |
|------|------|---------|
| `Label` | `string` | `""` |
| `Option1Text` | `string` | `""` |
| `Option1Checked` | `bool` | `false` |
| `Option2Text` | `string` | `""` |
| `Option2Checked` | `bool` | `false` |

**Uso:**
```xml
<controls:ToggleGroup Label="Método de Pago"
                      Option1Text="Efectivo"
                      Option2Text="Transferencia"
                      Option1Checked="{Binding EfectivoSeleccionado}"
                      Option2Checked="{Binding TransferenciaSeleccionado}" />
```

**ToggleGroup.xaml:**
```xml
<UserControl x:Class="SistemaGabinos.Controls.ToggleGroup"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <StackPanel>
        <TextBlock Text="{Binding Label, RelativeSource={RelativeSource AncestorType=UserControl}}"
                   Style="{StaticResource LabelMedium}"
                   Margin="0,0,0,8" />
        <StackPanel Orientation="Horizontal">
            <ToggleButton Content="{Binding Option1Text, RelativeSource={RelativeSource AncestorType=UserControl}}"
                          IsChecked="{Binding Option1Checked, RelativeSource={RelativeSource AncestorType=UserControl}}"
                          Style="{StaticResource ToggleButtonStyle}"
                          Margin="0,0,8,0" />
            <ToggleButton Content="{Binding Option2Text, RelativeSource={RelativeSource AncestorType=UserControl}}"
                          IsChecked="{Binding Option2Checked, RelativeSource={RelativeSource AncestorType=UserControl}}"
                          Style="{StaticResource ToggleButtonStyle}" />
        </StackPanel>
    </StackPanel>
</UserControl>
```

**ToggleGroup.xaml.cs:**
```csharp
using System.Windows;
using System.Windows.Controls;

namespace SistemaGabinos.Controls;

public partial class ToggleGroup : UserControl
{
    public static readonly DependencyProperty LabelProperty =
        DependencyProperty.Register(nameof(Label), typeof(string), typeof(ToggleGroup), new PropertyMetadata(""));
    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public static readonly DependencyProperty Option1TextProperty =
        DependencyProperty.Register(nameof(Option1Text), typeof(string), typeof(ToggleGroup), new PropertyMetadata(""));
    public string Option1Text
    {
        get => (string)GetValue(Option1TextProperty);
        set => SetValue(Option1TextProperty, value);
    }

    public static readonly DependencyProperty Option1CheckedProperty =
        DependencyProperty.Register(nameof(Option1Checked), typeof(bool), typeof(ToggleGroup), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    public bool Option1Checked
    {
        get => (bool)GetValue(Option1CheckedProperty);
        set => SetValue(Option1CheckedProperty, value);
    }

    public static readonly DependencyProperty Option2TextProperty =
        DependencyProperty.Register(nameof(Option2Text), typeof(string), typeof(ToggleGroup), new PropertyMetadata(""));
    public string Option2Text
    {
        get => (string)GetValue(Option2TextProperty);
        set => SetValue(Option2TextProperty, value);
    }

    public static readonly DependencyProperty Option2CheckedProperty =
        DependencyProperty.Register(nameof(Option2Checked), typeof(bool), typeof(ToggleGroup), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    public bool Option2Checked
    {
        get => (bool)GetValue(Option2CheckedProperty);
        set => SetValue(Option2CheckedProperty, value);
    }

    public ToggleGroup()
    {
        InitializeComponent();
    }
}
```

---

### Task 7: Refactorizar `NuevaMatricula.xaml` para usar componentes

**Files:**
- Modify: `SistemaGabinos/Views/NuevaMatricula.xaml` (reescribir usando controles)

**Resultado final:**
```xml
<Page x:Class="SistemaGabinos.Views.NuevaMatricula"
      xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
      xmlns:controls="clr-namespace:SistemaGabinos.Controls"
      Title="Nueva Matrícula"
      Background="{StaticResource BrushSurfaceAlt}"
      FontFamily="{StaticResource FontPoppins}">

    <ScrollViewer VerticalScrollBarVisibility="Auto">
        <StackPanel Margin="24">
            <TextBlock Text="Nueva Inscripción" Style="{StaticResource HeadlineLarge}" Margin="0,0,0,4" />
            <TextBlock Text="Completa los campos para registrar a un nuevo alumno"
                       Style="{StaticResource BodyMedium}"
                       Foreground="{StaticResource BrushTextMuted}" Margin="0,0,0,24" />

            <Grid>
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="*" />
                    <ColumnDefinition Width="24" />
                    <ColumnDefinition Width="*" />
                </Grid.ColumnDefinitions>

                <controls:CardSection Grid.Column="0" Title="Datos del Alumno">
                    <StackPanel>
                        <controls:IconInput Label="CURP (Obligatorio)" Icon="✅"
                                            Text="{Binding Curp}" MaxLength="18" />
                        <controls:FormField Label="Nombre Completo"
                                            Text="{Binding NombreCompleto}" Margin="0,16,0,0" />
                        <controls:IconInput Label="Teléfono de Contacto" Icon="&#x1F4DE;"
                                            Text="{Binding Telefono}" Margin="0,16,0,0" />
                        <controls:FormField Label="Fecha de Nacimiento" Type="DatePicker"
                                            Text="{Binding FechaNacimiento}" Margin="0,16,0,0" />

                        <Border BorderBrush="{StaticResource BrushBorder}"
                                BorderThickness="0,1,0,0" Margin="0,20,0,16" Padding="0,12"
                                Visibility="{Binding VisibleTutor}">
                            <TextBlock Text="Datos del Tutor (obligatorio si es menor de 18 años)"
                                       Style="{StaticResource BodyMedium}" FontStyle="Italic"
                                       Foreground="{StaticResource BrushTextMuted}" />
                        </Border>
                        <controls:FormField Label="Nombre del Tutor" Text="{Binding NombreTutor}" />
                        <Grid Margin="0,12,0,0">
                            <Grid.ColumnDefinitions>
                                <ColumnDefinition Width="*" />
                                <ColumnDefinition Width="12" />
                                <ColumnDefinition Width="*" />
                            </Grid.ColumnDefinitions>
                            <controls:FormField Grid.Column="0" Label="Parentesco" Text="{Binding ParentescoTutor}" />
                            <controls:FormField Grid.Column="2" Label="Teléfono del Tutor" Text="{Binding TelefonoTutor}" />
                        </Grid>
                    </StackPanel>
                </controls:CardSection>

                <controls:CardSection Grid.Column="2" Title="Caja / Pago Inicial"
                                      Visibility="{Binding CobroVisible}">
                    <StackPanel>
                        <controls:ChecklistBox ItemsSource="{Binding ConceptosCobro}" Total="{Binding Total}" />
                        <Border BorderBrush="{StaticResource BrushBorder}" BorderThickness="0,1,0,0"
                                Margin="0,12,0,12" />
                        <controls:ToggleGroup Label="Método de Pago"
                                              Option1Text="Efectivo" Option1Checked="{Binding EfectivoSeleccionado}"
                                              Option2Text="Transferencia" Option2Checked="{Binding TransferenciaSeleccionado}" />
                        <Border BorderBrush="{StaticResource BrushBorder}" BorderThickness="0,1,0,0"
                                Margin="0,12,0,12" />
                        <Button Content="Generar Ticket" Style="{StaticResource ButtonSecondary}"
                                Command="{Binding GenerarTicketCommand}" Height="36"
                                Visibility="{Binding TicketVisible}" HorizontalAlignment="Left" />
                    </StackPanel>
                </controls:CardSection>
            </Grid>

            <Button Content="Guardar Matrícula" Style="{StaticResource ButtonPrimary}"
                    Command="{Binding RegistrarCommand}" Margin="0,24,0,0"
                    HorizontalAlignment="Center" Width="240" />
            <TextBlock Text="Al guardar, el alumno quedará matriculado en el sistema."
                       Style="{StaticResource BodyMedium}" Foreground="{StaticResource BrushTextMuted}"
                       TextAlignment="Center" Margin="0,12,0,0" />
            <StackPanel Margin="0,12,0,0" Visibility="{Binding MensajeExitoVisible}">
                <TextBlock Text="{Binding Mensaje}" Foreground="{StaticResource BrushPrimary}"
                           Style="{StaticResource BodyMedium}" TextWrapping="Wrap" TextAlignment="Center" />
            </StackPanel>
            <TextBlock Text="{Binding MensajeError}" Foreground="{StaticResource BrushError}"
                       Margin="0,12,0,0" Style="{StaticResource BodyMedium}"
                       TextWrapping="Wrap" TextAlignment="Center"
                       Visibility="{Binding MensajeErrorVisible}" />
        </StackPanel>
    </ScrollViewer>
</Page>
```

Nota: `FormField` necesitaría una prop `Type` para soportar `DatePicker` como alternativa a `TextBox`. Esto se puede agregar como mejora en el componente.

---

### Resumen de archivos

| Archivo | Acción |
|---------|--------|
| `SistemaGabinos/Controls/FormField.xaml` | Crear |
| `SistemaGabinos/Controls/FormField.xaml.cs` | Crear |
| `SistemaGabinos/Controls/IconInput.xaml` | Crear |
| `SistemaGabinos/Controls/IconInput.xaml.cs` | Crear |
| `SistemaGabinos/Controls/CardSection.xaml` | Crear |
| `SistemaGabinos/Controls/CardSection.xaml.cs` | Crear |
| `SistemaGabinos/Controls/ChecklistBox.xaml` | Crear |
| `SistemaGabinos/Controls/ChecklistBox.xaml.cs` | Crear |
| `SistemaGabinos/Controls/ToggleGroup.xaml` | Crear |
| `SistemaGabinos/Controls/ToggleGroup.xaml.cs` | Crear |
| `SistemaGabinos/Views/NuevaMatricula.xaml` | Modificar (usar controles) |
| `SistemaGabinos/ViewModels/NuevaMatriculaViewModel.cs` | Sin cambios |

**Total:** ~300 líneas de XAML plano → ~50 líneas de XAML componentizado.
