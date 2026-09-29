# Awtop

<p align="center">
  <img src="awtop-hero.png" alt="Un león sobre una roca junto a una ventana fijada" width="100%">
</p>

<p align="center"><strong>Elige una ventana. Mantenla arriba.</strong></p>

Awtop es una utilidad pequeña para Windows que coloca una ventana elegida por encima de las demás. Es útil para seguir un vídeo, una llamada, un panel de referencias o Discord mientras trabajas en otra aplicación.

## Qué hace

- Muestra las ventanas abiertas en una lista.
- Fija la ventana que elijas en primer plano.
- Mantiene su tamaño y posición.
- Permite retirar la prioridad con un clic.

## Uso

1. Descarga o compila `Awtop.exe`.
2. Abre Awtop y pulsa **Actualizar**.
3. Elige la ventana que quieres mantener visible.
4. Pulsa **Fijar seleccionada**.
5. Cuando termines, pulsa **Quitar prioridad**.

## Compilar

Con .NET SDK 8 para Windows:

```powershell
dotnet build .\Awtop.csproj -c Release
```

Con el compilador de .NET Framework incluido en Windows:

```powershell
& "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\csc.exe" /target:winexe /win32icon:Awtop.ico /out:Awtop.exe /r:System.Windows.Forms.dll /r:System.Drawing.dll Awtop.cs
```

## Archivos visuales

- `Awtop.ico`: icono de Windows.
- `Awtop-icon.png`: versión en PNG del icono.
- `awtop-hero.png`: portada original del repositorio.

Para reconstruir el icono desde el PNG, usa `Build-Icon.ps1` con `-SourcePng` y `-DestinationIco`.
