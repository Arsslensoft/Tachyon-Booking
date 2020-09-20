#tool "nuget:?package=xunit.runner.console"

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

Task("dotnet-restore")
        .Does(() => 
        {
                DotNetCoreRestore("./Tachyon.Booking.sln");
        });

Task("dotnet-build")
        .IsDependentOn("dotnet-restore")	
        .Does(() => 
        {
                DotNetCoreBuild("./Tachyon.Booking.sln", new DotNetCoreBuildSettings 
                {
                        Configuration = configuration,
                        MSBuildSettings = new DotNetCoreMSBuildSettings
                        {
                                TreatAllWarningsAs = MSBuildTreatAllWarningsAs.Error
                        }
                });
        });

Task("run-xunit-tests")	
        .IsDependentOn("dotnet-build")
        .Does(() => 
        {
                var settings = new DotNetCoreTestSettings
        {
                        Configuration = configuration
        };
		
                DotNetCoreTest("./Tachyon.Booking.Tests/Tachyon.Booking.Tests.csproj", settings);
        });	

Task("Default")
        .IsDependentOn("run-xunit-tests");

RunTarget(target);