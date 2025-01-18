using UELib;
using UELib.Core;
using System.CommandLine;

var console = System.Console.Out;
// Redirect stdout console logging from UELib.
Console.SetOut(System.Console.Error);

var fileOption = new Option<FileInfo>(
    aliases: ["--file", "-f"],
    description: "The UE3 package file to read.")
{
    IsRequired = true
};

var rootCommand = new RootCommand(
    "UE3 package tool performs various package manipulation and analysis actions.");

var findSubLevelsCmd = new Command("find-sublevels", "Find the sublevels of a level package.");

rootCommand.Add(findSubLevelsCmd);
findSubLevelsCmd.SetHandler(FindSubLevels, fileOption);
findSubLevelsCmd.AddOption(fileOption);

return await rootCommand.InvokeAsync(args);

async Task FindSubLevels(FileInfo file)
{
    var pkg = UnrealLoader.LoadPackage(file.FullName, FileAccess.ReadWrite);
    pkg.InitializePackage();
    pkg.InitializeExportObjects();

    var theWorld = pkg.FindObjectByGroup("TheWorld");
    if (theWorld == null)
    {
        throw new InvalidDataException("Object 'TheWorld' not found in package. Invalid UE3 package?");
    }

    theWorld.BeginDeserializing();
    // Console.WriteLine(theWorld);

    foreach (var obj in pkg.Objects)
    {
        if (obj.Class.Name == "LevelStreamingAlwaysLoaded")
        {
            obj.BeginDeserializing();
            // Console.WriteLine("---");
            // Console.WriteLine($"{obj}, {obj.Name}, {obj.GetFriendlyType()}, {obj.Class.Name}");
            foreach (var prop in obj.Default.Properties)
            {
                // Console.WriteLine($"{prop.Name} {prop.Value}");
                if (prop.Name == "PackageName")
                {
                    console.WriteLine(prop.Value);
                }
            }
        }
    }
}