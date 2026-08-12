# 🚀 Guía de Actualizaciones con Velopack — SistemaGabinos

Este documento explica cómo funciona el sistema de actualizaciones automáticas basado en **Velopack** para **SistemaGabinos**, cómo compilar y empaquetar versiones para desarrolladores y cómo lo experimenta el usuario final.

---

## 🏗️ Arquitectura de Actualizaciones

- **Tecnología**: [Velopack](https://velopack.io/) (Motor moderno de empacado y auto-actualización).
- **Origen de Releases**: Repositorio de GitHub (`https://github.com/MiguelNeyoy/SistemaGabinos`).
- **Instalación**: Velopack gestiona la creación de accesos directos en Escritorio/Menú Inicio y desinstalador sin requerir privilegios de Administrador.

---

## 👨‍💻 Guía para Desarrolladores (Cómo publicar una versión)

### Requisitos Previos

Instalar la herramienta de CLI de Velopack (`vpk`) de forma global (solo se hace una vez):

```bash
dotnet tool install -g vpk
```

---

### Paso a Paso para Empaquetar y Publicar v1.0.0 (o superior)

#### 1. Actualizar la versión en el proyecto
Asegúrate de cambiar la versión del ensamblado en `SistemaGabinos.csproj` o en tus etiquetas de Release.

#### 2. Compilar el proyecto en modo Release
Ejecuta el siguiente comando para generar los binarios optimizados:

```bash
dotnet publish d:\Proyectos\SistemaGabinos\SistemaGabinos\SistemaGabinos.csproj -c Release -o d:\Proyectos\SistemaGabinos\dist
```

#### 3. Crear el paquete instalador de Velopack (`vpk pack`)
Usa la herramienta `vpk` para empaquetar la carpeta `dist` en un instalador ejecutable:

```bash
vpk pack -u SistemaGabinos -v 1.0.0 -p d:\Proyectos\SistemaGabinos\dist -e SistemaGabinos.exe
```

Esto generará en la carpeta actual:
- `SistemaGabinos-Setup.exe` (Instalador inicial para usuarios).
- `SistemaGabinos-1.0.0-full.nupkg` (Paquete completo de versión).
- `releases.win.json` (Índice de versiones de Velopack).

#### 4. Subir a GitHub Releases
1. Ve a tu repositorio en GitHub: `https://github.com/MiguelNeyoy/SistemaGabinos`.
2. Crea una nueva Release (etiqueta Ej: `v1.0.0`).
3. Adjunta los archivos generados: `SistemaGabinos-Setup.exe`, `SistemaGabinos-1.0.0-full.nupkg` y `releases.win.json`.
4. Publica la Release.

---

## 👤 Guía para Usuarios / Secretarias (Cómo actualizar)

1. En la aplicación, ve al módulo de **Configuración**.
2. Presiona el botón **"Buscar Actualizaciones"**.
3. Si existe una nueva versión publicada en GitHub:
   - Aparecerá el mensaje `¡Nueva versión v1.1.0 disponible!`.
   - Se activará el botón **"Descargar y Aplicar Actualización"**.
4. Haz clic en **"Descargar y Aplicar Actualización"**:
   - El sistema descargará los paquetes en segundo plano mostrando el progreso.
   - Una vez finalizada la descarga, el programa se **reiniciará automáticamente** ya actualizado con la nueva versión.

---

## 📂 Estructura del Código del Proyecto

- `SistemaGabinos.Infrastructure/Updates/`
  - [`IUpdateService.cs`](file:///d:/Proyectos/SistemaGabinos/SistemaGabinos.Infrastructure/Updates/IUpdateService.cs): Contrato del servicio de actualizaciones.
  - [`UpdateService.cs`](file:///d:/Proyectos/SistemaGabinos/SistemaGabinos.Infrastructure/Updates/UpdateService.cs): Implementación con `Velopack.UpdateManager` y `GithubSource`.
- `SistemaGabinos/Program.cs`: Punto de entrada nativo que ejecuta `VelopackApp.Build().Run()` antes del arranque de la ventana de WPF.
- `SistemaGabinos/ViewModels/ConfiguracionViewModel.cs`: Comandos y estado visual para la pantalla de Configuración.
