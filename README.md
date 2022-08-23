# Assembly Definition Generator
Experimental automatic Assembly Definition generator for Unity Editor

> [Unity Documentation - Assembly definitions](https://docs.unity3d.com/Manual/ScriptCompilationAssemblyDefinitionFiles.html)

---

## Features

* Automatic Assembly Definition References based on assemblies referenced by the existing scripts
* Assembly Definitions created under `Editor` folders will be set to `Editor` platform only
* Automatic UdonSharp Assembly Definition generation for folders containing UdonSharpBehaviours

---

## How to use Assembly Definition Generator

1. Right-click your Project window and navigate to `Create` > `Assembly Definition (Automatic)`

![image](https://user-images.githubusercontent.com/26690821/186280472-2af54c0b-d8fe-4a4c-a180-ca360be2dd9e.png)

2. Type in the name for the Assembly Definition in the file save dialog

![image](https://user-images.githubusercontent.com/26690821/186281277-41bb8bef-5ef1-41ff-946b-13c352f47c69.png)

3. You should now have an automatically configured Assembly Definition in your folder

![image](https://user-images.githubusercontent.com/26690821/186282247-aa33472a-3a58-4897-b8bd-dacf261a33ce.png)

---

## Installation via Unity Package Manager (git):
1. Navigate to your toolbar: `Window` > `Package Manager` > `[+]` > `Add package from git URL...` and type in: `https://github.com/Varneon/AssemblyDefinitionGenerator.git?path=/Packages/com.varneon.assembly-definition-generator`

## Installation via [VRChat Creator Companion](https://vcc.docs.vrchat.com/):
1. Download the the repository's .zip [here](https://github.com/Varneon/AssemblyDefinitionGenerator/archive/refs/heads/main.zip)
2. Unpack the .zip somewhere
3. In VRChat Creator Companion, navigate to `Settings` > `User Packages` > `Add`
4. Navigate to the unpacked folder, `com.varneon.assembly-definition-generator` and click `Select Folder`
5. `Assembly Definition Generator` should now be visible under `Local User Packages` in the project view in VRChat Creator Companion

## Installation via Unitypackage:
1. Download latest Udon Array Extensions from [here](https://github.com/Varneon/AssemblyDefinitionGenerator/releases/latest)
2. Import the downloaded .unitypackage into your Unity project
