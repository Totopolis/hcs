using Application.Shared.Abstractions;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Reflection;

namespace Infrastructure.Shared.SystemInfo;

/*
Add to Xxx.csproj file this code:
 	<ItemGroup>
		<AssemblyAttribute Include="Infrastructure.Shared.SystemInfo.ReleaseDate">
			<_Parameter1>$([System.DateTime]::UtcNow.ToString("O"))</_Parameter1>
		</AssemblyAttribute>
	</ItemGroup>
*/
internal sealed class SystemInfo : ISystemInfo
{
    private readonly IHostEnvironment _environment;

    public SystemInfo(IHostEnvironment hostEnvironment)
    {
        _environment = hostEnvironment;
        StartDateTime = DateTime.UtcNow;

        var assembly = Assembly.GetEntryAssembly()!;
        // string assemblyVersion = assembly.GetName().Version!.ToString();
        string fileVersion = FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion!;
        // string productVersion = FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion!;

        Version = fileVersion;
    }

    /// <summary>
    /// Source code build time.
    /// </summary>
    public DateTime BuildDateTime => ReleaseDateAttribute.GetReleaseDate(
        Assembly.GetExecutingAssembly());

    /// <summary>
    /// When system started.
    /// </summary>
    public DateTime StartDateTime { get; init; }

    public TimeSpan Uptime => DateTime.UtcNow - StartDateTime;

    public bool IsDevelopment => _environment.IsDevelopment();

    public string ApplicationName => _environment.ApplicationName;

    public string EnvironmentName => _environment.EnvironmentName;

    public string Version { get; init; }
}
