using CommandLine;
using System.Diagnostics;

namespace DDC.CLI;

internal class Program
{

    private static DirectoryInfo outputFolder;
    private static DirectoryInfo dofusFolder;
    private static DirectoryInfo ddcFolder;

    private static DirectoryInfo bepinFolder;
    private static DirectoryInfo assetFolder;
    private static string assetStudioPath;

    static async Task Main(string[] args) => await Parser.Default.ParseArguments<Options>(args).WithParsedAsync(RunOptions);

    static async Task RunOptions(Options opts)
    {
        ddcFolder = new DirectoryInfo(opts.DDCFolderPath);
        dofusFolder = new DirectoryInfo(opts.DofusFolderPath);
        outputFolder = new DirectoryInfo(opts.OutputPath ?? Path.Combine(opts.DofusFolderPath, "extracted"));

        var tasks = Enumerable.Empty<Task>();

        if (!opts.DebugLocalData.HasValue || !opts.DebugLocalData.Value)
        {
            // Extract assets asynchronously
            if (opts.AssetStudioPath != null)
            {
                assetFolder = new DirectoryInfo(Path.Join(dofusFolder.FullName, "Dofus_Data/StreamingAssets/Content/Picto"));
                var bundles = assetFolder.GetFiles("*.bundle", SearchOption.AllDirectories);
                assetStudioPath = opts.AssetStudioPath;
                tasks = bundles.Select(b => ExtractAssetBundle(b.Directory!.Name, b.Name));
            }
            //Install Bepin
            if (opts.BepinFolderPath != null)
            {
                bepinFolder = new DirectoryInfo(opts.BepinFolderPath);
                SetupBepIn();
                CreateBepInConfigFolder();
                await RunGame("Chainloader startup complete");
                SetupInterop();
            }
            //Extract types
            await BuildModelExtractor();
        }

        // Extract data
        tasks = tasks.Append(BuildDataExtractor());
        Task.WaitAll(tasks.ToArray());
        CleanPlugins();
    }

    static async Task Run(string cmd)
    {
        Debug.WriteLine($"Execute: {cmd}");
        Process process = new();
        ProcessStartInfo startInfo = new()
        {
            WindowStyle = ProcessWindowStyle.Hidden,
            FileName = "powershell.exe",
            Arguments = cmd,
        };
        process.StartInfo = startInfo;
        process.Start();
        await process.WaitForExitAsync();
    }

    static void SetupBepIn()
    {
        CopyDirectory(dofusFolder.FullName, bepinFolder);
    }

    static async Task RunGame(string untilLog)
    {
        // \"dofus-beta-2.73.45.43/Dofus.exe\"
        var dofusPath = Path.Combine(dofusFolder.FullName, "Dofus.exe");
        var scriptPath = Path.Combine(ddcFolder.FullName, "scripts/bepinex-run-until");
        var cmd = $"node \"{scriptPath}\" \"{dofusPath}\" \"{untilLog}\"";
        await Run(cmd);
    }

    static void SetupInterop()
    {
        var dofusPath = Path.Combine(dofusFolder.FullName, "BepInEx", "interop");
        var ddcPath = Path.Combine(ddcFolder.FullName, "Interop");
        Directory.CreateDirectory(ddcPath);
        CopyDirectory(ddcPath, new DirectoryInfo(dofusPath));
    }

    static async Task BuildModelExtractor()
    {
        CleanPlugins();
        await BuildProject("DDC.ModelExtractor");
        await CopyPlugins("DDC.ModelExtractor");

        var configPath = Path.Combine(dofusFolder.FullName, "BepInEx", "config", "DDC.ModelExtractor.cfg");
        var generatedFolder = Path.Combine(ddcFolder.FullName, "DDC", "Generated");
        File.WriteAllText(configPath,
            @$"
            [General]
            OutputDirectory = {generatedFolder}
            ");

        await RunGame("DDC_type model generation complete.");
    }

    static async Task BuildDataExtractor()
    {
        CleanPlugins();
        await BuildProject("DDC.Extractor");
        await CopyPlugins("DDC.Extractor");

        var configPath = Path.Combine(dofusFolder.FullName, "BepInEx", "config", "DDC.Extractor.cfg");
        var extractedFolder = Path.Combine(outputFolder.FullName, "data");
        File.WriteAllText(configPath,
            @$"
            [General]
            OutputDirectory = {extractedFolder}
            ");

        await RunGame("DDC_data extraction complete.");
    }

    static void CleanPlugins()
    {
        //Thread.Sleep(1000);
        var pluginsFolder = Path.Combine(dofusFolder.FullName, "BepInEx", "plugins");
        var dir = new DirectoryInfo(pluginsFolder);
        foreach (var plugin in dir.EnumerateFiles())
        {
            plugin.DeleteSafe();
        }
    }

    static async Task CopyPlugins(string projectName)
    {
        var pluginsFolder = Path.Combine(dofusFolder.FullName, "BepInEx", "plugins");
        var bin = Path.Combine(ddcFolder.FullName, projectName, "bin/Release/net6.0/DDC*.dll");
        await Run($"copy {bin} {pluginsFolder}");
    }

    static void CreateBepInConfigFolder()
    {
        var configFolder = Path.Combine(dofusFolder.FullName, "BepInEx", "config");
        //Run($"md {configFolder} -Force");
        Directory.CreateDirectory(configFolder);
    }

    static async Task BuildProject(string projectName)
    {
        var csproj = Path.Combine(ddcFolder.FullName, projectName, projectName + ".csproj");
        var cmd = $"dotnet build {csproj} --configuration Release --no-restore --property WarningLevel=0";
        await Run(cmd);
    }

    static void CopyDirectory(string destDir, DirectoryInfo sourceDir)
    {
        Directory.CreateDirectory(destDir);
        foreach (var dir in sourceDir.GetDirectories())
        {
            var path = Path.Combine(destDir, dir.Name);
            if (Directory.Exists(path))
                return;
            CopyDirectory(path, dir);
        }
        foreach (var file in sourceDir.GetFiles())
        {
            var path = Path.Combine(destDir, file.Name);
            if (File.Exists(path))
                return;
            file.CopyTo(path);
        }
    }

    static async Task ExtractAssetBundle(string bundleFolder, string bundleName)
    {
        var input = Path.Combine(assetFolder.FullName, bundleFolder, bundleName);
        bundleName = bundleName.Replace("_.bundle", "").Replace(".bundle", "");
        var output = Path.Combine(outputFolder.FullName, "assets", bundleFolder, bundleName);
        Directory.CreateDirectory(output);
        //var input = "C:/Users/Blank/AppData/Local/Ankama/Dofus-beta/Dofus_Data/StreamingAssets/Content/Picto/Spells/spellstate_.bundle";
        //var output = "C:/Users//Blank/AppData//Local/Ankama/Dofus-beta/Dofus_Data/StreamingAssets/Content/Picto/Spells/spellstate/";
        //var assetStudioPath = "./AssetStudio.CLI.exe";

        Debug.WriteLine($"Extracting: {bundleFolder}/{bundleName} to: {output}");
        var cmd = $"{assetStudioPath} \"{input}\" \"{output}\" --silent --types Sprite --game Normal --unity_version 2022.3.42f1 --logger_flags Error";
        await Run(cmd);
        var sprites = new DirectoryInfo(Path.Combine(output, "Sprite"));
        foreach (var file in sprites.GetFiles())
        {
            file.MoveTo(Path.Combine(output, file.Name));
        }
        try
        {
            sprites.Delete();
        }
        catch (Exception)
        {
        }
    }

}

public static class Extensions
{
    public static void DeleteSafe(this FileInfo file)
    {
        while (true)
        {
            try
            {
                file.Delete();
                return;
            }
            catch (UnauthorizedAccessException)
            {
                Thread.Sleep(100);
            }
        }
    }

}