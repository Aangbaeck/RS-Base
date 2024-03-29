![RS-Base](Etc/LogoRSBase.png) 

> An awesome boilerplate template for building beautiful MVVM-WPF applications.
> It comes preconfigured with MVVM Community Toolkit, Serilog, Localization, MaterialDesignForXAML and a nice architectural structure.
> Every release has a Project template for Visual Studio to easily get started.

[![License](http://img.shields.io/:license-mit-blue.svg?style=flat-square)](http://badges.mit-license.org)

Dark Theme                 |  Light Theme
:-------------------------:|:-------------------------:
[![Screenshot1](Etc/screenshot1.png)]()  |  [![Screenshot1](Etc/screenshot2.png)]()


---
### Clone
- Clone this repo to your local machine using `https://github.com/Aangbaeck/RS-Base.git`

## Setup

### Developer Setup
You need to have .Net 6.0 or Framework 4.8 & Visual Studio 2022 installed

When creating project templates - use the whole project "RS-Base Clean""

### User Setup
* Download latest release.
* Close Visual Studio.
* Copy "RS-Base Vx.zip" and put in "%USERPROFILE%\Documents\Visual Studio 2022\Templates\ProjectTemplates".

When creating a new Project in Visual Studiop this template will show up.
Use the window RSView instead of Window in WPF to get nice styling

You need to have .Net 6.0 or Framework 4.8 & Visual Studio 2022 installed
If the references gives you any troubles: Run the command "Update-Package -Reinstall" (without "") in the Package-Manager console

The NuGet-package RS-StandardComponents is published on NuGet [Nuget Gallery](https://www.nuget.org/packages/RS-StandardComponents)

Tips:
* Use [LogExpert](https://github.com/zarunbal/LogExpert) to read the logfiles. LogExpert can tail the logfiles very efficient.
* Use [ResX Manager](https://github.com/dotnet/ResXResourceManager) for handling the Resource files. It's a very convenient tool. It's both availible as a standalone app and a extension to Visual Studio.
* There is a compiled version of [MaterialDesignInXAML](https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit/releases) to be used as a cheat set to see what types of controls are availible

---

## Featuring
- [Windows Community Toolkit](https://github.com/CommunityToolkit/WindowsCommunityToolkit)
- [MaterialDesignInXamlToolkit](https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit)
- [MaterialDesignExtensions](https://github.com/spiegelp/MaterialDesignExtensions)
- [Serilog](https://github.com/serilog/serilog)
- [Localization Infralution](https://www.codeproject.com/Articles/35159/WPF-Localization-Using-RESX-Files)


---

## Contributing
#### Step 1
- **Option 1**
    - 🍴 Fork this repo!
- **Option 2**
    - 👯 Clone this repo to your local machine using `https://github.com/Aangbaeck/RS-Base.git`
#### Step 2
- **HACK AWAY!** 🔨🔨🔨

#### Step 3
- ✅ Check that everything works
- 🔃 Create a new pull request.

---
## Roadmap
* Extend the example app more with a lot more comments.
* Add automatic template creation when building project.
* Become HACKERMAN!
---

## License
[![License](http://img.shields.io/:license-mit-blue.svg?style=flat-square)](http://badges.mit-license.org) 
- **[MIT license](http://opensource.org/licenses/mit-license.php)**
