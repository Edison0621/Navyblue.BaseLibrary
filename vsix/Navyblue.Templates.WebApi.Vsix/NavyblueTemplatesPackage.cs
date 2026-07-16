using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace Navyblue.Templates.WebApi.Vsix;

/// <summary>
///     VS package that registers the Navyblue Web API <c>dotnet new</c> template on load.
/// </summary>
[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
[InstalledProductRegistration("Navyblue ASP.NET Core Web API Templates", "Navyblue layered DDD/CQRS Web API project template (navyblue-webapi).", "3.2.0")]
[Guid(PackageGuidString)]
[ProvideAutoLoad(UIContextGuids80.NoSolution, PackageAutoLoadFlags.BackgroundLoad)]
[ProvideAutoLoad(UIContextGuids80.SolutionExists, PackageAutoLoadFlags.BackgroundLoad)]
public sealed class NavyblueTemplatesPackage : AsyncPackage
{
    public const string PackageGuidString = "a1b2c3d4-e5f6-7890-abcd-ef1234567890";
    private const string TemplatePackageId = "Navyblue.Templates.WebApi";

    protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
    {
        await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

        try
        {
            string nupkgPath = ResolveEmbeddedNupkgPath();
            if (string.IsNullOrEmpty(nupkgPath) || !File.Exists(nupkgPath))
            {
                await this.LogAsync($"Navyblue template nupkg not found near extension. Expected under Packages\\.");
                return;
            }

            await this.LogAsync($"Installing Navyblue Web API template from: {nupkgPath}");
            int exitCode = await RunDotnetAsync($"new install \"{nupkgPath}\" --force", cancellationToken).ConfigureAwait(true);
            if (exitCode == 0)
                await this.LogAsync($"Installed {TemplatePackageId}. Create a project via File > New > Project, search 'Navyblue', or: dotnet new navyblue-webapi");
            else
                await this.LogAsync($"dotnet new install exited with code {exitCode}. You can install manually: dotnet new install \"{nupkgPath}\"");
        }
        catch (Exception ex)
        {
            await this.LogAsync($"Failed to install Navyblue template: {ex.Message}");
        }
    }

    private static string ResolveEmbeddedNupkgPath()
    {
        string assemblyDir = Path.GetDirectoryName(typeof(NavyblueTemplatesPackage).Assembly.Location) ?? string.Empty;
        string candidate = Path.Combine(assemblyDir, "Packages", "Navyblue.Templates.WebApi.nupkg");
        if (File.Exists(candidate))
            return candidate;

        // Extension install folder layout
        string parent = Directory.GetParent(assemblyDir)?.FullName ?? assemblyDir;
        candidate = Path.Combine(parent, "Packages", "Navyblue.Templates.WebApi.nupkg");
        return File.Exists(candidate) ? candidate : candidate;
    }

    private static Task<int> RunDotnetAsync(string arguments, CancellationToken cancellationToken)
    {
        return Task.Run(() =>
        {
            var psi = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            if (process is null)
                return -1;

            process.WaitForExit();
            cancellationToken.ThrowIfCancellationRequested();
            return process.ExitCode;
        }, cancellationToken);
    }

    private async Task LogAsync(string message)
    {
        await this.JoinableTaskFactory.SwitchToMainThreadAsync();
        try
        {
            var pane = await this.GetOutputPaneAsync();
            pane?.OutputStringThreadSafe($"[Navyblue Templates] {message}{Environment.NewLine}");
        }
        catch
        {
            // ignore logging failures
        }
    }

    private async Task<IVsOutputWindowPane> GetOutputPaneAsync()
    {
        await this.JoinableTaskFactory.SwitchToMainThreadAsync();
        if (await this.GetServiceAsync(typeof(SVsOutputWindow)) is not IVsOutputWindow outputWindow)
            return null;

        var paneGuid = new Guid("B8C8C0E0-1111-2222-3333-444455556666");
        outputWindow.GetPane(ref paneGuid, out IVsOutputWindowPane pane);
        if (pane is null)
        {
            outputWindow.CreatePane(ref paneGuid, "Navyblue Templates", 1, 1);
            outputWindow.GetPane(ref paneGuid, out pane);
        }

        return pane;
    }
}
