using System.Diagnostics;

namespace DDC.CLI;

internal class Program
{

    private static DirectoryInfo dofusFolder;
    private static DirectoryInfo bepinFolder;
    private static DirectoryInfo ddcFolder;
    private static DirectoryInfo assetFolder;
    private static string assetStudioPath;

    static async Task Main(string[] args)
    {
        dofusFolder = new DirectoryInfo(args[0]);
        bepinFolder = new DirectoryInfo(args[1]);
        ddcFolder = new DirectoryInfo(args[2]);
        assetFolder = new DirectoryInfo(Path.Join(dofusFolder.FullName, "Dofus_Data/StreamingAssets/Content/Picto"));
        assetStudioPath = args[3];

        if (args.Length == 5 && args[4] == "true")
        {
            SetupBepIn();
            CreateBepInConfigFolder();
            await RunGame("Chainloader startup complete");
            SetupInterop();
        }

        // Extract types
        CleanPlugins();
        await BuildModelExtractor();

        // Extract assets & data
        var bundles = assetFolder.GetFiles("*.bundle", SearchOption.AllDirectories);
        var tasks = bundles.Select(b => ExtractAssetBundle(b.Directory!.Name, b.Name))
            .Append(BuildDataExtractor())
            .ToArray();
        Task.WaitAll(tasks);
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
            //RedirectStandardOutput = true,
        };
        process.StartInfo = startInfo;
        //process.StandardOutput
        process.Start();
        //process.WaitForExit();
        await process.WaitForExitAsync();
        //while (!process.StandardOutput.EndOfStream)
        //{
        //    string line = process.StandardOutput.ReadLine();
        //    Console.WriteLine(line);
        //    Debug.WriteLine(line);
        //}
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
        CleanPlugins();
    }

    static async Task BuildDataExtractor()
    {
        await BuildProject("DDC.Extractor");
        await CopyPlugins("DDC.Extractor");

        var configPath = Path.Combine(dofusFolder.FullName, "BepInEx", "config", "DDC.Extractor.cfg");
        var extractedFolder = Path.Combine(dofusFolder.FullName, "extracted/data");
        File.WriteAllText(configPath,
            @$"
            [General]
            OutputDirectory = {extractedFolder}
            ");

        await RunGame("DDC_data extraction complete.");
        CleanPlugins();
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
        //CopyDirectory(pluginsFolder, new DirectoryInfo(Path.Combine(ddcFolder.FullName, projectName, "bin/Release/net6.0")));
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
        var cmd = $"dotnet build {csproj} --configuration Release --no-restore";
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
        bundleName = bundleName.Replace("_.bundle", "");
        bundleName = bundleName.Replace(".bundle", "");
        var output = Path.Combine(dofusFolder.FullName, "extracted/assets", bundleFolder, bundleName);
        Directory.CreateDirectory(output);
        Debug.WriteLine($"Extracting: {bundleFolder}/{bundleName} to: {output}");
        Console.WriteLine($"Extracting: {bundleFolder}/{bundleName} to: {output}");
        //var input = "C:/Users/Blank/AppData/Local/Ankama/Dofus-beta/Dofus_Data/StreamingAssets/Content/Picto/Spells/spellstate_.bundle";
        //var output = "C:/Users//Blank/AppData//Local/Ankama/Dofus-beta/Dofus_Data/StreamingAssets/Content/Picto/Spells/spellstate/";
        //var assetStudioPath = "./AssetStudio.CLI.exe";
        var cmd = $"{assetStudioPath} \"{input}\" \"{output}\" --silent --types Sprite --game Normal --unity_version 2022.3.42f1 --logger_flags Error";
        await Run(cmd);
        var sprites = new DirectoryInfo(Path.Combine(output, "Sprite"));
        foreach (var file in sprites.GetFiles())
        {
            try
            {
                file.MoveTo(Path.Combine(output, file.Name));
            }
            catch (Exception)
            {
            }
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
                //file.DeleteSafe();
            }
        }
    }

}