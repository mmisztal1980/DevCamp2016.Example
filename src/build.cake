/**
 *  File: build.cake
 *  Description : DevCamp2016 Solution Build System
 *  Author: @mmisztal1980
 */

#addin "Cake.XdtTransform"
#addin "Cake.PowerShell"

/* Project Parameters */
var project = new {
    WebAppName = "Example.Api" // The web project that will be deployed to Azure
};
/* Constants */
const string buildDir = @".\Build";
const string solutionFile = @".\Solution.sln";

/* Build Parameters */
var target = Argument("target", "UnitTests");               // Default Target: UnitTests
var configuration = Argument("configuration", "Debug");     // Default Configuration: Debug

var unitTestsPath = string.Format("**/bin/{0}/*.UnitTests.dll", configuration);
var integrationTestsPath = string.Format("**/bin/{0}/*.IntegrationTests.dll", configuration);

/**
 * \brief: Cleans the build directory
 */
Task("Clean").Does(() => {
    CleanDirectory(buildDir);
}); // Clean

/**
 * \brief Runs the .NET build using the current configuration
 */
Task("Build")
    .IsDependentOn("Clean")
    .Does(() => {
     DotNetBuild(solutionFile, c => c.Configuration = configuration);        
}); // Build

/**
 * \brief Executes the UnitTests on all *.UnitTests.dll(s)
 */
Task("UnitTests")
    .IsDependentOn("Build")
    .Does(() => {
    XUnit2(unitTestsPath);
}); // UnitTests

/**
 * \brief: Copies build artifacts to the buildDir
 * \warn: Execute AFTER Build and UnitTests have been run
 */
Task("CopyArtifacts")
    .IsDependentOn("UnitTests")
    .Does(() => {
       var artifacts = new Dictionary<string, string>() {
           { @".\" + project.WebAppName + @"\bin\*.dll", buildDir + @"\" + project.WebAppName + @"\bin" },
       };
       
       foreach(var kvp in artifacts) {
           CreateDirectory(kvp.Value);
           CopyFiles(kvp.Key, kvp.Value);
           Information(kvp.Key + " ==> " + kvp.Value);
       }
}); // CopyArtifacts

/**
 * \brief: Applies Xdt transforms to the .config files
 */
Task("Configure")
    .IsDependentOn("CopyArtifacts")
    .Does(() => {
    
    var configFiles = new Dictionary<string, string>() {
        { @".\" + project.WebAppName + @"\Web.config", buildDir + @"\" + project.WebAppName + @"\Web.config" },
    };
    
    foreach(var kvp in configFiles) {
        XdtTransformConfig(
            kvp.Key, 
            File(kvp.Key.Replace(".config", string.Format(".{0}.config", configuration))), 
            kvp.Value);
        Information(kvp.Key + " ==" + configuration + "==> " + kvp.Value);
    }        
}); // Configure

/**
 * \brief: Deploys the application to target FTP site using PowerShell
 */
Task("Deploy")
    .IsDependentOn("Configure")
    .Does(() => {
    
    StartPowershellFile("./scripts/FtpUpload.ps1", args =>
    {
         args.Append("username", Environment.GetEnvironmentVariable("FTP_USERNAME"))
            .Append("password",  Environment.GetEnvironmentVariable("FTP_PASSWORD"))
            .Append("hostname",  Environment.GetEnvironmentVariable("FTP_HOSTNAME"))
            .Append("source", @"Build\Example.Api\*");
    });          
});

/**
 * \brief: Executes the IntegrationTests on all *.IntegrationTests.dll(s)
 * \warn: Execute AFTER Deploy has been run
 */
Task("IntegrationTests")
    .Does(() => {
    XUnit2(integrationTestsPath);
}); // IntegrationTests

RunTarget(target);