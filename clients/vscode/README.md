# Injection.VsCode

Yoko Injection support for Visual Studio Code.

To use this extension you have to install:

- [Visual Studio Code](https://code.visualstudio.com/)
- [.NET Core 2.1](https://dotnet.microsoft.com/download/dotnet-core/2.1)

This extension is in very early development stage so there are many bugs
and unimplemented features. If you are interested in this project, please file a bug on [Github](https://github.com/uoinfusion/InjectionScript/issues) or on [Discord](https://discord.gg/Ng3RDke).

## Features

- Editing Yoko Injection scripts in an amazing text editor on Windows, Linux or MacOS.
- Code completion

![Code completion example](https://raw.githubusercontent.com/uoinfusion/InjectionScript/master/clients/vscode/images/code-completion.gif)

- Source code analysis reports usage of unknown variable, subrutine or keyword.

![Code error and warning example](https://raw.githubusercontent.com/uoinfusion/InjectionScript/master/clients/vscode/images/code-analysis.gif)

- Go to Definition (F12)

![Go to Definition](https://raw.githubusercontent.com/uoinfusion/InjectionScript/master/clients/vscode/images/goto-definition.gif)

## Known Issues

Implementation is very slow inefficient and incomplete.

## Release Notes

### 0.0.16
- Added `safe call` syntax.
- Added `UO.MoveOn`, `UO.MoveOff`, `UO.Track`, `UO.CancelTarget`, `UO.ConColor`, `UO.WaitTargetType`, `UO.BandageSelf`, `UO.ContainerOff`, `UO.FunRunning`.
- Fixed overloads `UO.Click`, `UO.DeleteJournal`, `UO.AddObject`.

### 0.0.15

- `break` keyword.
- Added/fixed `UO.MoveItem`, `UO.Print`, `UO.LastTile`, `UO.Count`, `UO.Dead`, `UO.GetName`, `UO.DeleteObject`, `UO.UseSkill`, `UO.WaitMenu`, `UO.Launch`, `UO.SkillVal`.
- Fixed literal parsing (problems with `"` inside `'` or `'` inside `"`).
- Fixed escaped characters - `\` doesn't denotes escaped characters anymore.

### 0.0.8

- Many bug fixes.
- More supported Injection API.
- More syntax highlighting colors.

### 0.0.6

- F12 navigates to symbol definition (subrutine, variable, label).

### 0.0.1

- Initial release.
