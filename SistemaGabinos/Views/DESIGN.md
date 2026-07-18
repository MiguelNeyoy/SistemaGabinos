# Sistema Gabinos — Design System

> Basado en el diseño "Dashboard Gabino's" de Google Stitch.
> Versión para WPF (.NET 10, Windows Desktop).

---

## Paleta de Colores (2 colores)

| Token | Hex | Uso |
|-------|-----|-----|
| `ColorPrimary` | `#FFCE0C` | Amarillo — Botones principales, acento activo, barra de navegación activa, resaltados |
| `ColorSecondary` | `#FF6224` | Naranja — Acciones secundarias, notificaciones, badges, progreso |

### Colores neutros de soporte (no cuentan como color principal)

| Token | Hex | Uso |
|-------|-----|-----|
| `ColorSurface` | `#FFFFFF` | Blanco — Fondo de cards, inputs, contenido |
| `ColorSurfaceAlt` | `#F8F6F0` | Blanco cálido — Fondo general de la app, sidebar, contenedores secundarios |
| `ColorBorder` | `#E1E3E3` | Gris borde — Bordes de cards, inputs, separadores |
| `ColorText` | `#191C1C` | Casi negro — Texto principal |
| `ColorTextMuted` | `#4E4632` | Marrón grisáceo — Texto secundario, labels |
| `ColorError` | `#BA1A1A` | Rojo — Errores, validaciones |

---

## Tipografía

| Token | Font Family | Tamaño | Peso | Line Height |
|-------|-------------|--------|------|-------------|
| `DisplayLarge` | Poppins | 48px | Bold (700) | 56px |
| `HeadlineLarge` | Poppins | 32px | Semibold (600) | 40px |
| `HeadlineMedium` | Poppins | 24px | Semibold (600) | 32px |
| `TitleLarge` | Poppins | 20px | Semibold (600) | 28px |
| `BodyLarge` | Poppins | 16px | Regular (400) | 24px |
| `BodyMedium` | Poppins | 14px | Regular (400) | 20px |
| `LabelMedium` | Poppins | 12px | Medium (500) | 16px |

> Poppins se distribuye como recurso incrustado (.ttf) en la carpeta `Fonts/` del proyecto WPF.
> Se incluyen 4 pesos: Regular (400), Medium (500), SemiBold (600), Bold (700).
> En XAML se referencia con: `pack://application:,,,/Fonts/#Poppins`

---

## Layout

- **Sidebar:** 260px de ancho fijo, fondo `#F8F6F0`
- **Margen contenedor principal:** 24px
- **Grid baseline:** 4px
- **Gutter entre elementos:** 16px
- **Ancho máximo de contenedor:** 1440px
- **Densidad:** 8px entre items en listas/tablas, 16-24px en módulos de lectura

---

## Componentes

### Sidebar
- Fondo: `#F8F6F0`, mismo tono cálido que el fondo general
- Items de navegación: Texto oscuro `#191C1C`
- Item activo: Barra vertical amarilla (`#FFCE0C`) de 3px en borde izquierdo
- Hover: Leve oscurecimiento del fondo

### Botones
| Tipo | Fondo | Texto | Borde | Radius |
|------|-------|-------|-------|--------|
| **Primary** | `#FFCE0C` | `#191C1C` | Ninguno | 4px |
| **Secondary** | Transparente | `#191C1C` | 1px `#191C1C` | 4px |
| **Accent** | `#FF6224` | Blanco | Ninguno | 4px |

### Inputs
- Fondo blanco, borde 1px `#E1E3E3`, radius 4px
- Focus: borde 2px `#FFCE0C`
- Label: sobre el campo, `LabelMedium`, color `#4E4632` al 70%

### Cards
- Fondo blanco, borde 1px `#E1E3E3`, radius 8px
- Sin sombra
- Header con separador horizontal de 1px

### Tablas
- Header: `BodyMedium` semibold, color `#4E4632`
- Filas: `BodyMedium` regular
- Alternancia de color en filas: `#F8F9F9` / blanco

### Progress Bars
- Track: `#F0F0F0`
- Fill: `#FF6224` (naranja) / cambia a `#FFCE0C` al alcanzar meta

---

## Shapes (Border Radius)

| Elemento | Radius |
|----------|--------|
| Botones, inputs | 4px |
| Cards | 8px |
| Badges, indicadores | 9999px (pill) |

---

## Elevación

- Sin sombras. La jerarquía visual se logra con colores y bordes.
- **Surface 0:** Fondo blanco
- **Surface 1:** Cards con borde de 1px
- **Sidebar:** Fondo cálido se distingue del contenido por el borde gris

---

## Íconos

Usar **Segoe MDL2 Assets** (nativo de Windows) o texto unicode.
