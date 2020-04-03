using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.NuGet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Compile);

    private const string SharedProjectName = "HangfireWorkerService.Shared";
    private const string SampleJobsProjectName = "HangfireWorkerService.SampleJobs";

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;
    [Parameter("Build Version to use")]
    readonly string BuildVersion = "0.1.0";
    [Parameter("NuGet server API key")]
    readonly string NuGetApiKey;
    [Parameter("NuGet server URL")]
    readonly string NuGetServerUrl;

    [Solution] readonly Solution Solution;
    //[GitRepository] readonly GitRepository GitRepository;

    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            EnsureCleanDirectory(ArtifactsDirectory);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore());
        });

    Target PackSharedNuGet => _ => _
        .DependsOn(Compile)
        .Requires(() => BuildVersion)
        .Executes(() =>
        {
            DotNetPack(c => c
                .SetProject(Solution.GetProject(SharedProjectName).Path)
                .SetVersion(BuildVersion)
                .SetOutputDirectory(ArtifactsDirectory / "nuget"));
        });

    Target PackSampleJobs => _ => _
        .DependsOn(Compile)
        .Requires(() => BuildVersion)
        .Executes(() =>
        {
            DotNetPack(c => c
                .SetProject(Solution.GetProject(SampleJobsProjectName).Path)
                .SetVersion(BuildVersion)
                .SetOutputDirectory(ArtifactsDirectory / "nuget"));
        });

    Target PushSharedNuGet => _ => _
        .DependsOn(PackSharedNuGet)
        .Requires(() => BuildVersion)
        .Requires(() => NuGetApiKey)
        .Requires(() => NuGetServerUrl)
        .Executes(() =>
        {
            NuGetTasks.NuGetPush(c => c
                .SetApiKey(NuGetApiKey)
                .SetSource(NuGetServerUrl)
                .SetTargetPath(ArtifactsDirectory / "nuget" / $"{SharedProjectName}.{BuildVersion}.nupkg"));
        });

    Target PushSampleJobsNuGet => _ => _
        .DependsOn(PackSampleJobs)
        .Requires(() => BuildVersion)
        .Requires(() => NuGetApiKey)
        .Requires(() => NuGetServerUrl)
        .Executes(() =>
        {
            NuGetTasks.NuGetPush(c => c
                .SetApiKey(NuGetApiKey)
                .SetSource(NuGetServerUrl)
                .SetTargetPath(ArtifactsDirectory / "nuget" / $"{SampleJobsProjectName}.{BuildVersion}.nupkg"));
        });

    Target PushAllNugets => _ => _
        .DependsOn(PushSharedNuGet, PushSampleJobsNuGet)
        .Executes(() => { });
}
