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

    [Option('d', "debuglocaldata", Required = false, HelpText = "Debug local data (removes bepin setup, assets and model extractiong)")]
    public bool? DebugLocalData { get; set; }

    [Option('v', "branch", Required = true, HelpText = "Dofus branch is [beta, dofus3, main], where beta and dofus3 are current versions and main is 2.73")]
    public string Branch { get; set; }

}
