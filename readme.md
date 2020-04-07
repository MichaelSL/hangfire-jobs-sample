# Hangfire Worker Service Demo Project

This is a demo for using Hangfire server to run schedules jobs.

## How to run it?

Now project is setup to consume `HangfireWorkerService.Shared` and `HangfireWorkerService.SampleJobs` as NuGet packages.
You will need a NuGet server to publish tha packages (in case you don't want to use NuGet - read [this](#run-without-nuget-packages) section).

### Nuke build

This project uses [Nuke build](https://nuke.build/) to assemble the project.

To push packages to your NuGet server you need to run
- on Windows:
```
PS> ./build.ps1 -target PushAllNugets -buildVersion "Your build version here" -NuGetServerUrl "Your NuGet server url" -NuGetApiKey "Your NuGet server API key"
```
- on Linux:
```
$> ./build.sh -target PushAllNugets -buildVersion "Your build version here" -NuGetServerUrl "Your NuGet server url" -NuGetApiKey "Your NuGet server API key"
```

### Run without NuGet packages

- Remove `HangfireWorkerService.Shared` and `HangfireWorkerService.SampleJobs` packages from `HangfireWorkerService` project.
- Remove `HangfireWorkerService.Shared` from `HangfireWorkerService.SampleJobs` projects.
- Add `HangfireWorkerService.Shared` to `HangfireWorkerService.SampleJobs` project dependencies.
- Add `HangfireWorkerService.Shared` and `HangfireWorkerService.SampleJobs` projects to `HangfireWorkerService` project dependencies.

### Run

Now you can restore packages and run a service from Visual Studio

## Jobs

Jobs are distributed in a NuGet packages. Jobs should implement `IConfigurableJob` interface.

You can configure job peoperties to utilize different Hangfire job types.

- `JobType` determines how your job will be executed (i.e. one time or recurring)
- `Interval` - interval for recurring jobs. Hangfire will start job again regardless if previous run finished or still in progress.
- `Delay` - delay for schedule jobs.
- `JobId` - string identifier for recurring jobs.
- `SectionName` - *appsettings.json* file section with job config.