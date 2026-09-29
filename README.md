# Fijar ventana

Aplicación sencilla para Windows que permite elegir una ventana abierta y mantenerla siempre en primer plano.

## Uso

1. Abre `DiscordFloater-v2.exe`.
2. Pulsa **Actualizar** y elige una ventana de la lista.
3. Pulsa **Fijar seleccionada**.
4. Para revertirlo, pulsa **Quitar prioridad**.

El icono está en `DiscordFloater.ico`, generado a partir de `DiscordFloater-icon.png`.

## Compilar

Con .NET SDK 8 en Windows:

```powershell
dotnet build .\DiscordFloater.csproj -c Release
```

Con el compilador de .NET Framework incluido en Windows:

```powershell
& "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\csc.exe" /target:winexe /win32icon:DiscordFloater.ico /out:DiscordFloater-v2.exe /r:System.Windows.Forms.dll /r:System.Drawing.dll DiscordFloater.Legacy.cs
```

Para reconstruir el `.ico` desde el PNG, ejecuta `Build-Icon.ps1` con `-SourcePng` y `-DestinationIco`.
