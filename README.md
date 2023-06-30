# Beat Saber Markup Language
An easy way to set up your UI in Beat Saber without manually creating new objects and parenting them.

More info on the [docs](https://monkeymanboy.github.io/BSML-Docs/).

## For developers

### Contributing to Beat Saber Markup Language
In order to build this project, please create the file `BeatSaberMarkupLanguage.csproj.user` and add your Beat Saber directory path to it in the project directory.
This file should not be uploaded to GitHub and is filtered out by the .gitignore.

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <!-- Set "YOUR OWN" Beat Saber folder here to resolve most of the dependency paths! -->
    <BeatSaberDir>E:\Program Files (x86)\Steam\steamapps\common\Beat Saber</BeatSaberDir>
  </PropertyGroup>
</Project>
```

If you plan on adding any new dependencies which are located in the Beat Saber directory, it would be nice if you edited the paths to use `$(BeatSaberDir)` in `BeatSaberMarkupLanguage.csproj`

```xml
...
<Reference Include="IPA.Loader">
  <HintPath>$(BeatSaberDir)\Beat Saber_Data\Managed\IPA.Loader.dll</HintPath>
</Reference>
...
```

### GitHub Packages
This project currently uses [a fork](github.com/nicoco007/BepInEx.AssemblyPublicizer) of [BepInEx.AssemblyPublicizer](github.com/BepInEx/BepInEx.AssemblyPublicizer). You will need to add `https://nuget.pkg.github.com/nicoco007/index.json` as a package source for NuGet to download it. Take a look at GitHub's [Working with the NuGet Registry](https://docs.github.com/en/packages/working-with-a-github-packages-registry/working-with-the-nuget-registry#authenticating-to-github-packages) for details.
