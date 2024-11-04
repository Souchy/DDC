using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace DDC.CLI;

internal class Options
{

    [Option('o', "output", Required = false, HelpText = "Path to extracted output folder")]
    public string? OutputPath { get; set; }

    [Option('i', "input", Required = true, HelpText = "Path to Dofus folder")]
    public string DofusFolderPath { get; set; }

    [Option('w', "workspace", Required = true, HelpText = "Path to DDC workspace folder")]
    public string DDCFolderPath { get; set; }

    [Option('b', "bepin", Required = false, HelpText = "Path to Bepin download folder")]
    public string? BepinFolderPath { get; set; }

    [Option('a', "assetstudio", Required = false, HelpText = "Path to AssetStudio download folder")]
    public string? AssetStudioPath { get; set; }

    [Option('d', "debuglocaldata", Required = false, HelpText = "Debug local data (deactivates bepin setup, assets and model extractiong)")]
    public bool? DebugLocalData { get; set; }

    //    dofusFolder = new DirectoryInfo(args[0]);
    //    bepinFolder = new DirectoryInfo(args[1]);
    //    ddcFolder = new DirectoryInfo(args[2]);
    //    assetFolder = new DirectoryInfo(Path.Join(dofusFolder.FullName, "Dofus_Data/StreamingAssets/Content/Picto"));
    //     assetStudioPath = args[3];

    //    if (args.Length == 5 && args[4] == "true")
    //    {
    //              SetupBepIn();
    //              CreateBepInConfigFolder();
    //              await RunGame("Chainloader startup complete");
    //              SetupInterop();
    //     }

}
