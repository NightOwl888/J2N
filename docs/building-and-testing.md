# Building and Testing

## Command Line

### Prerequisites

- [PowerShell](https://docs.microsoft.com/en-us/powershell/scripting/install/installing-powershell) 6.0 or higher (see [this question](http://stackoverflow.com/questions/1825585/determine-installed-powershell-version) to check your PowerShell version)
- [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- [.NET 9 Runtime (Or SDK)](https://dotnet.microsoft.com/en-us/download/dotnet/9.0) (Required only if testing)
- [.NET 8 Runtime (Or SDK)](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) (Required only if testing)
- [.NET 6 Runtime (Or SDK)](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) (Required only if testing)

### Execution

> **NOTE:** If the project is open in Visual Studio, its background restore may interfere with these commands. It is recommended to close all instances of Visual Studio that have this project open before executing.

To build the source, clone or download and unzip the repository. From the repository or distribution root, execute the **build** command from a command prompt and include the desired options from the build options table below:

##### Windows

```console
> build [options]
```

##### Linux or macOS

```console
./build [options]
```

> **NOTE:** The `build` file will need to be given permission to run using the command `chmod u+x build` before the first execution.

#### Build Options

The following options are case-insensitive. Each option has both a short form indicated by a single `-` and a long form indicated by `--`. The options that require a value must be followed by a space and then the value, similar to running the [dotnet CLI](https://docs.microsoft.com/en-us/dotnet/core/tools/).

<table>
    <tr>
        <th>Short</th>
        <th>Long</th>
        <th>Description</th>
        <th>Example</th>
    </tr>
    <tr>
        <td>&#8209;config</td>
        <td>&#8209;&#8209;configuration</td>
        <td>The build configuration ("Release" or "Debug").</td>
        <td>build&nbsp;&#8209;&#8209;configuration Debug</td>
    </tr>
    <tr>
        <td>&#8209;t</td>
        <td>&#8209;&#8209;test</td>
        <td>Runs the tests after building. This option does not require a value.</td>
        <td>build&nbsp;&#8209;t</td>
    </tr>
</table>

For example the following command creates a Release build with NuGet package with a version generated using the nbgv tool and will also run the tests for every target framework.

##### Windows

```console
> build --configuration Release --test
```

##### Linux or macOS

```console
./build --configuration Release --test
```

NuGet packages are output by the build to the `/_artifacts/NuGetPackages/` directory. Test results (if applicable) are output to the `/_artifacts/TestResults/` directory.

You can setup Visual Studio to read the NuGet packages like any NuGet feed by following these steps:

1. In Visual Studio, right-click the solution in Solution Explorer, and choose "Manage NuGet Packages for Solution"
2. Click the gear icon next to the Package sources dropdown.
3. Click the `+` icon (for add)
4. Give the source a name such as `spatial4n Local Packages`
5. Click the `...` button next to the Source field, and choose the `/src/_artifacts/NuGetPackages` folder on your local system.
6. Click Ok

Then all you need to do is choose the `spatial4n Local Packages` feed from the dropdown (in the NuGet Package Manager) and you can search for, install, and update the NuGet packages just as you can with any Internet-based feed.

## Visual Studio

### Prerequisites

- Visual Studio 2022 or higher
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/visual-studio-sdks)
- [.NET 9 Runtime (Or SDK)](https://dotnet.microsoft.com/en-us/download/dotnet/9.0) (Required only if testing)
- [.NET 8 Runtime (Or SDK)](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) (Required only if testing)
- [.NET 6 Runtime (Or SDK)](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) (Required only if testing)

> **NOTE:** Visual Studio 2022 is only supported for building and testing .NET Core and .NET Standard 2.1 due to false errors caused by lack of LSP language support for C# 14.0. It is recommended to use Visual Studio 2026, which fully supports our build. If using Visual Studio 2022, we recommend running `dotnet build` on the command line prior to submitting PRs to ensure builds succeed on .NET Framework and .NET Standard 2.0.

### Execution

1. Open `J2N.sln` in Visual Studio.
2. Build a project or the entire solution, and wait for Visual Studio to discover the tests.
3. Run or debug the tests in Test Explorer, optionally using the desired filters.

> **TIP:** When running tests in Visual Studio, [set the default processor architecture to either 32 or 64 bit](https://stackoverflow.com/a/45946727) depending on your preference.
