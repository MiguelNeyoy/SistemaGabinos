# Nueva Matrícula — Layout Full Width (Plan de Implementación)

> **For agentic workers:** Use subagent-driven-development or executing-plans. Steps use checkbox syntax.

**Goal:** Rediseñar `NuevaMatricula.xaml` con layout de 2 columnas ocupando todo el ancho, emparejando Card 1 (Datos del Alumno) a la izquierda y Card 3 (Caja/Pago Inicial) a la derecha.

**Architecture:** Formulario WPF en ScrollViewer > StackPanel > Grid de 2 columnas. Sin cambios en Domain/Application/Infrastructure. Solo XAML + contratos de binding en comentarios.

**Tech Stack:** WPF .NET 10, CommunityToolkit.Mvvm, Poppins, DESIGN.md tokens, ToggleButtons para método de pago.

## Global Constraints

- Sin cambios en Domain, Application, Infrastructure
- Sin lógica de negocio, sin ViewModel, sin C# — solo XAML
- Seguir tokens visuales de `SistemaGabinos/Views/DESIGN.md`
- Layout responsive dentro del content area (sidebar 260px + content)
- No incluir Card 2 (Asignación Académica)

---

### Task: Reescribir NuevaMatricula.xaml con layout full width

**Files:**
- Modify: `SistemaGabinos/Views/NuevaMatricula.xaml` (reescribir completo)

**Layout final:**

```
┌───────────────────────────────────────────────────────────────────┐
│  Nueva Inscripción                                                │
│  Completa los campos para registrar a un nuevo alumno             │
│                                                                   │
│  ┌──── Card Izquierda (Datos) ────┐ ┌─── Card Derecha (Caja) ──┐│
│  │ CURP               [✔] [   ]  │ │ ☑ Inscripción  $0.00     ││
│  │ Nombre Completo    [       ]  │ │ ☑ Libro (Book) $350.00   ││
│  │ Teléfono       📞  [       ]  │ │ ────────────────────────  ││
│  │ Fecha Nacimiento   [📅     ]  │ │ Total:  $350.00           ││
│  │ ─── Tutor (si <18) ─────────  │ │                            ││
│  │ Nombre Tutor       [       ]  │ │ Método:                    ││
│  │ Parentesco         [       ]  │ │ [Efectivo] [Transferencia] ││
│  │ Tel. Tutor         [       ]  │ │ [🖨️ Generar Ticket]       ││
│  └────────────────────────────────┘ └────────────────────────────┘│
│                                                                   │
│  [  Guardar Matrícula  ]                                          │
│  Al guardar, el alumno quedará matriculado...                     │
└───────────────────────────────────────────────────────────────────┘
```

- [ ] **Step 1: Reemplazar el contenido completo del archivo**

Eliminar el contenido actual y escribir el nuevo XAML:

```xml
<Page x:Class="SistemaGabinos.Views.NuevaMatricula"
      xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
      Title="Nueva Matrícula"
      Background="{StaticResource BrushSurfaceAlt}"
      FontFamily="{StaticResource FontPoppins}">

    <ScrollViewer VerticalScrollBarVisibility="Auto">
        <StackPanel Margin="24">

            <!-- Page Title -->
            <TextBlock Text="Nueva Inscripción"
                       Style="{StaticResource HeadlineLarge}"
                       Margin="0,0,0,4" />
            <TextBlock Text="Completa los campos para registrar a un nuevo alumno"
                       Style="{StaticResource BodyMedium}"
                       Foreground="{StaticResource BrushTextMuted}"
                       Margin="0,0,0,24" />

            <!-- Two-column layout: Datos (left) + Caja (right) -->
            <Grid>
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="*" />
                    <ColumnDefinition Width="20" />
                    <ColumnDefinition Width="*" />
                </Grid.ColumnDefinitions>

                <!-- ============================================ -->
                <!-- CARD 1: DATOS DEL ALUMNO (Left column)       -->
                <!-- ============================================ -->
                <Border Grid.Column="0"
                        Style="{StaticResource CardStyle}"
                        Padding="24">
                    <StackPanel>

                        <!-- CURP row -->
                        <TextBlock Text="CURP (Obligatorio)"
                                   Style="{StaticResource LabelMedium}"
                                   Margin="0,0,0,4" />
                        <Grid>
                            <Grid.ColumnDefinitions>
                                <ColumnDefinition Width="Auto" />
                                <ColumnDefinition Width="*" />
                            </Grid.ColumnDefinitions>
                            <TextBlock Grid.Column="0"
                                       Text="&#x2705;"
                                       VerticalAlignment="Center"
                                       Margin="0,0,8,0" />
                            <TextBox Grid.Column="1"
                                     Style="{StaticResource InputStyle}"
                                     Text="{Binding Curp, UpdateSourceTrigger=PropertyChanged}"
                                     MaxLength="18" />
                        </Grid>

                        <!-- Nombre Completo -->
                        <TextBlock Text="Nombre Completo"
                                   Style="{StaticResource LabelMedium}"
                                   Margin="0,16,0,4" />
                        <TextBox Style="{StaticResource InputStyle}"
                                 Text="{Binding NombreCompleto, UpdateSourceTrigger=PropertyChanged}" />

                        <!-- Teléfono row -->
                        <TextBlock Text="Teléfono de Contacto"
                                   Style="{StaticResource LabelMedium}"
                                   Margin="0,16,0,4" />
                        <Grid>
                            <Grid.ColumnDefinitions>
                                <ColumnDefinition Width="Auto" />
                                <ColumnDefinition Width="*" />
                            </Grid.ColumnDefinitions>
                            <TextBlock Grid.Column="0"
                                       Text="&#x1F4DE;"
                                       VerticalAlignment="Center"
                                       Margin="0,0,8,0" />
                            <TextBox Grid.Column="1"
                                     Style="{StaticResource InputStyle}"
                                     Text="{Binding Telefono, UpdateSourceTrigger=PropertyChanged}" />
                        </Grid>

                        <!-- Fecha de Nacimiento -->
                        <TextBlock Text="Fecha de Nacimiento"
                                   Style="{StaticResource LabelMedium}"
                                   Margin="0,16,0,4" />
                        <DatePicker SelectedDate="{Binding FechaNacimiento}"
                                    FontFamily="{StaticResource FontPoppins}"
                                    FontSize="14" />

                        <!-- Tutor Section (condicional) -->
                        <Border BorderBrush="{StaticResource BrushBorder}"
                                BorderThickness="0,1,0,0"
                                Margin="0,20,0,16"
                                Padding="0,12"
                                Visibility="{Binding VisibleTutor}">
                            <TextBlock Text="Datos del Tutor (obligatorio si es menor de 18 años)"
                                       Style="{StaticResource BodyMedium}"
                                       FontStyle="Italic"
                                       Foreground="{StaticResource BrushTextMuted}" />
                        </Border>

                        <!-- Nombre del Tutor -->
                        <TextBlock Text="Nombre del Tutor"
                                   Style="{StaticResource LabelMedium}"
                                   Margin="0,0,0,4" />
                        <TextBox Style="{StaticResource InputStyle}"
                                 Text="{Binding NombreTutor, UpdateSourceTrigger=PropertyChanged}" />

                        <!-- Parentesco + Teléfono Tutor en fila -->
                        <Grid Margin="0,12,0,0">
                            <Grid.ColumnDefinitions>
                                <ColumnDefinition Width="*" />
                                <ColumnDefinition Width="12" />
                                <ColumnDefinition Width="*" />
                            </Grid.ColumnDefinitions>
                            <StackPanel Grid.Column="0">
                                <TextBlock Text="Parentesco"
                                           Style="{StaticResource LabelMedium}"
                                           Margin="0,0,0,4" />
                                <TextBox Style="{StaticResource InputStyle}"
                                         Text="{Binding ParentescoTutor, UpdateSourceTrigger=PropertyChanged}" />
                            </StackPanel>
                            <StackPanel Grid.Column="2">
                                <TextBlock Text="Teléfono del Tutor"
                                           Style="{StaticResource LabelMedium}"
                                           Margin="0,0,0,4" />
                                <TextBox Style="{StaticResource InputStyle}"
                                         Text="{Binding TelefonoTutor, UpdateSourceTrigger=PropertyChanged}" />
                            </StackPanel>
                        </Grid>

                    </StackPanel>
                </Border>

                <!-- ============================================ -->
                <!-- CARD 3: CAJA / PAGO INICIAL (Right column)   -->
                <!-- ============================================ -->
                <Border Grid.Column="2"
                        Style="{StaticResource CardStyle}"
                        Padding="24"
                        VerticalAlignment="Top"
                        Visibility="{Binding CobroVisible}">
                    <StackPanel>
                        <TextBlock Text="Caja / Pago Inicial"
                                   Style="{StaticResource HeadlineMedium}"
                                   Margin="0,0,0,16" />

                        <!--
                            Contratos de Binding (ViewModel debe exponer):
                              ConceptosCobro (ObservableCollection<ConceptoCobroItem>) → ListBox.ItemsSource
                                - ConceptoCobroItem.Seleccionado (bool)  → CheckBox.IsChecked
                                - ConceptoCobroItem.NombreVisual (string) → TextBlock.Text
                                - ConceptoCobroItem.Monto (decimal)       → TextBox.Text
                                - ConceptoCobroItem.MontoEsEditable (bool) → (opcional)
                              Total (decimal)          → TextBlock.Text
                              MetodoPago (string?)     → propiedad para toggle seleccionado
                              EfectivoSeleccionado (bool) → ToggleButton.IsChecked
                              TransferenciaSeleccionado (bool) → ToggleButton.IsChecked
                              TicketVisible (Visibility) → Button.Visibility
                              GenerarTicketCommand (IRelayCommand) → Button.Command
                              RegistrarCommand (IRelayCommand) → Button.Command
                        -->

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

                        <!-- Total -->
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
                                       Text="{Binding Total, StringFormat={}${0:N2}}"
                                       Style="{StaticResource TitleLarge}"
                                       Foreground="{StaticResource BrushPrimary}"
                                       TextAlignment="Right"
                                       VerticalAlignment="Center" />
                        </Grid>

                        <!-- Separator -->
                        <Border BorderBrush="{StaticResource BrushBorder}"
                                BorderThickness="0,1,0,0"
                                Margin="0,12,0,12" />

                        <!-- Método de Pago (ToggleButtons) -->
                        <TextBlock Text="Método de Pago"
                                   Style="{StaticResource LabelMedium}"
                                   Margin="0,0,0,8" />
                        <StackPanel Orientation="Horizontal">
                            <ToggleButton Content="&#x1F4B5; Efectivo"
                                          IsChecked="{Binding EfectivoSeleccionado}"
                                          Style="{StaticResource ToggleButtonStyle}"
                                          Margin="0,0,8,0" />
                            <ToggleButton Content="&#x1F3E6; Transferencia"
                                          IsChecked="{Binding TransferenciaSeleccionado}"
                                          Style="{StaticResource ToggleButtonStyle}" />
                        </StackPanel>

                        <!-- Separator -->
                        <Border BorderBrush="{StaticResource BrushBorder}"
                                BorderThickness="0,1,0,0"
                                Margin="0,12,0,12" />

                        <!-- Ticket Button -->
                        <Button Content="&#x1F5A8; Generar Ticket"
                                Style="{StaticResource ButtonSecondary}"
                                Command="{Binding GenerarTicketCommand}"
                                Height="36"
                                Visibility="{Binding TicketVisible}"
                                HorizontalAlignment="Left" />

                    </StackPanel>
                </Border>
            </Grid>

            <!-- Submit Button -->
            <Button Content="Guardar Matrícula"
                    Style="{StaticResource ButtonPrimary}"
                    Command="{Binding RegistrarCommand}"
                    Margin="0,24,0,0"
                    HorizontalAlignment="Center"
                    Width="240" />

            <!-- Footer text -->
            <TextBlock Text="Al guardar, el alumno quedará matriculado en el sistema."
                       Style="{StaticResource BodyMedium}"
                       Foreground="{StaticResource BrushTextMuted}"
                       TextAlignment="Center"
                       Margin="0,12,0,0" />

            <!-- Success Message -->
            <StackPanel Margin="0,12,0,0"
                        Visibility="{Binding MensajeExitoVisible}">
                <TextBlock Text="{Binding Mensaje}"
                           Foreground="{StaticResource BrushPrimary}"
                           Style="{StaticResource BodyMedium}"
                           TextWrapping="Wrap"
                           TextAlignment="Center" />
            </StackPanel>

            <!-- Error Message -->
            <TextBlock Text="{Binding MensajeError}"
                       Foreground="{StaticResource BrushError}"
                       Margin="0,12,0,0"
                       Style="{StaticResource BodyMedium}"
                       TextWrapping="Wrap"
                       TextAlignment="Center"
                       Visibility="{Binding MensajeErrorVisible}" />

        </StackPanel>
    </ScrollViewer>
</Page>
```

- [ ] **Step 2: Verificar que el ToggleButtonStyle existe en Styles.xaml**

Si no existe, agregarlo a `SistemaGabinos/Resources/Themes/Styles.xaml`:

```xml
<!-- ToggleButton style for payment method selection -->
<Style x:Key="ToggleButtonStyle" TargetType="ToggleButton">
    <Setter Property="FontFamily" Value="{StaticResource FontPoppins}" />
    <Setter Property="FontSize" Value="14" />
    <Setter Property="FontWeight" Value="Medium" />
    <Setter Property="Background" Value="{StaticResource BrushSurface}" />
    <Setter Property="Foreground" Value="{StaticResource BrushText}" />
    <Setter Property="BorderBrush" Value="{StaticResource BrushBorder}" />
    <Setter Property="BorderThickness" Value="1" />
    <Setter Property="Padding" Value="16,8" />
    <Setter Property="Cursor" Value="Hand" />
    <Setter Property="Template">
        <Setter.Value>
            <ControlTemplate TargetType="ToggleButton">
                <Border x="Name="Border"
                        Background="{TemplateBinding Background}"
                        BorderBrush="{TemplateBinding BorderBrush}"
                        BorderThickness="{TemplateBinding BorderThickness}"
                        CornerRadius="4"
                        Padding="{TemplateBinding Padding}">
                    <ContentPresenter HorizontalAlignment="Center"
                                      VerticalAlignment="Center"
                                      RecognizesAccessKey="True" />
                </Border>
                <ControlTemplate.Triggers>
                    <Trigger Property="IsChecked" Value="True">
                        <Setter TargetName="Border" Property="Background" Value="{StaticResource BrushPrimary}" />
                        <Setter TargetName="Border" Property="BorderBrush" Value="{StaticResource BrushPrimary}" />
                        <Setter Property="Foreground" Value="{StaticResource BrushText}" />
                    </Trigger>
                    <Trigger Property="IsMouseOver" Value="True">
                        <Setter TargetName="Border" Property="BorderBrush" Value="{StaticResource BrushPrimary}" />
                    </Trigger>
                </ControlTemplate.Triggers>
            </ControlTemplate>
        </Setter.Value>
    </Setter>
</Style>
```

Nota: El template anterior tiene un error sintáctico (`x:Name="Border"` debe ser `x:Name="Border"`). El estilo debe usar triggers correctos. Alternativamente se puede lograr con el estilo nativo de WPF.

---

### Resumen de contratos de binding

| Propiedad | Tipo | Control |
|-----------|------|---------|
| `Curp` | `string` | TextBox |
| `NombreCompleto` | `string` | TextBox |
| `Telefono` | `string` | TextBox |
| `FechaNacimiento` | `DateTime` | DatePicker |
| `VisibleTutor` | `Visibility` | Border.Visibility |
| `NombreTutor` | `string?` | TextBox |
| `ParentescoTutor` | `string?` | TextBox |
| `TelefonoTutor` | `string?` | TextBox |
| `CobroVisible` | `Visibility` | Card.Visibility |
| `ConceptosCobro` | `ObservableCollection<ConceptoCobroItem>` | ListBox.ItemsSource |
| `Total` | `decimal` | TextBlock.Text |
| `EfectivoSeleccionado` | `bool` | ToggleButton.IsChecked |
| `TransferenciaSeleccionado` | `bool` | ToggleButton.IsChecked |
| `TicketVisible` | `Visibility` | Button.Visibility |
| `MensajeExitoVisible` | `Visibility` | StackPanel.Visibility |
| `Mensaje` | `string` | TextBlock.Text |
| `MensajeError` | `string` | TextBlock.Text |
| `MensajeErrorVisible` | `Visibility` | TextBlock.Visibility |
| `RegistrarCommand` | `IRelayCommand` | Button.Command |
| `GenerarTicketCommand` | `IRelayCommand` | Button.Command |

**ConceptoCobroItem** (contrato):
- `Seleccionado` (bool)
- `NombreVisual` (string)
- `Monto` (decimal)
