# Configuración de la Interfaz Gráfica (GUI)

Este documento explica cómo configurar y usar la interfaz gráfica del proyecto Shin Megami Tensei.

## Prerequisitos

La implementación de la GUI está lista, pero requiere los siguientes pasos de configuración:

### 1. Instalar paquetes NuGet de Avalonia

En el proyecto `Shin-Megami-Tensei-View`, instala los siguientes paquetes desde NuGet:
- `Avalonia`
- `Avalonia.Desktop`
- `Avalonia.Themes.Fluent`

### 2. Agregar la librería Shin-Megami-Tensei-GUI.dll

1. Coloca la carpeta `GuiLib` (proporcionada por el profesor) dentro del proyecto `Shin-Megami-Tensei-View`

2. Agrega la referencia a la DLL en ambos proyectos (`View` y `Controller`):
   - Click derecho en `Dependencies`
   - `Add From...`
   - Navega hasta `GuiLib/Shin-Megami-Tensei-GUI.dll`
   - Repite para ambos proyectos

### 3. Verificar la instalación

Agrega este using en cualquier archivo:
```csharp
using Shin_Megami_Tensei_GUI;
```

Si no hay errores de compilación, la instalación fue exitosa.

## Estructura del Código

### Clases Adapter

Se crearon las siguientes clases adapter en `Shin-Megami-Tensei-View/Gui/Adapters/`:

- **UnitAdapter.cs**: Convierte `Unit` del modelo a `IUnit` de la GUI
- **PlayerAdapter.cs**: Convierte `Team` del modelo a `IPlayer` de la GUI
- **StateAdapter.cs**: Convierte `GameState` del modelo a `IState` de la GUI

### Interfaces y Vistas

- **IShinMegamiTenseiView.cs**: Interfaz común para todas las vistas (consola y GUI)
- **ShinMegamiTenseiGuiView.cs**: Implementación de la vista GUI
- **ShinMegamiTenseiConsoleView.cs**: Wrapper para mantener compatibilidad con vista de consola

## Uso

### Modo Consola (Actual)

El programa actual funciona en modo consola para testing:

```csharp
var view = View.BuildManualTestingView(test);
var game = new Game(view, teamsFolder);
game.Play();
```

### Modo GUI (Pendiente de configuración)

Una vez configurada la librería GUI, se puede usar así en `Program.cs`:

```csharp
using Shin_Megami_Tensei_GUI;
using Shin_Megami_Tensei_View.Gui;

bool useGui = true;

if (useGui)
{
    // Crear vista GUI
    var teamController = /* crear TeamController */;
    var guiView = new ShinMegamiTenseiGuiView(teamController);

    // Crear juego con vista GUI
    var game = new Game(guiView, "");

    // Iniciar GUI y ejecutar juego
    guiView.Start(game.Play);
}
else
{
    // Modo consola para testing (actual)
    string testFolder = SelectTestFolder();
    string test = SelectTest(testFolder);
    string teamsFolder = testFolder.Replace("-Tests", "");

    var view = View.BuildManualTestingView(test);
    var game = new Game(view, teamsFolder);
    game.Play();
}
```

## Implementación Pendiente

Para completar la integración de la GUI, se necesita:

1. **ConvertTeamInfoToTeam**: Implementar la conversión de `ITeamInfo` (de la GUI) a `Team` (del modelo)
   - Ubicación: `ShinMegamiTenseiGuiView.cs`
   - Requiere acceso a los repositorios de unidades y habilidades

2. **Mapeo de Selección**: Implementar la lógica para convertir elementos clickeados en índices
   - `GetSelectedTarget()`: Mapear unit clickeado a índice en el board
   - `GetSelectedMonster()`: Mapear unit en reserva a índice

3. **Constructor Alternativo en Game**: Agregar constructor que acepte `IShinMegamiTenseiView`
   ```csharp
   public Game(IShinMegamiTenseiView view)
   {
       // Implementar
   }
   ```

4. **Integración con GameController**: Modificar `GameController` para usar la interfaz `IShinMegamiTenseiView`

## Testing

Los tests actuales NO se verán afectados porque:
- El constructor original de `Game(View, string)` se mantiene intacto
- La vista de consola sigue funcionando como antes
- La GUI es una adición, no un reemplazo

## Notas

- La arquitectura sigue el patrón Adapter para convertir entre el modelo y las interfaces de la GUI
- Se mantiene la separación de responsabilidades (MVC)
- El código es extensible para agregar más tipos de vistas en el futuro
