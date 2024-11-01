using System.Diagnostics;

namespace DDC.CLI;

internal class Program
{

    private static DirectoryInfo dofusFolder;
    private static DirectoryInfo bepinFolder;
    private static DirectoryInfo ddcFolder;

    static void Main(string[] args)
    {
        dofusFolder = new DirectoryInfo(args[0]);
        bepinFolder = new DirectoryInfo(args[1]);
        ddcFolder = new DirectoryInfo(args[2]);
        if(args.Length == 4 && args[3] == "true")
        {
            SetupBepIn();
            CreateBepInConfigFolder();
            RunGame("Chainloader startup complete");
            SetupInterop();
        }

        CleanPlugins();
        BuildModelExtractor();
        BuildExtractor();

    }

    static void Run(string cmd)
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
        process.WaitForExit();
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

    static void RunGame(string untilLog)
    {
        // \"dofus-beta-2.73.45.43\\Dofus.exe\"
        var dofusPath = Path.Combine(dofusFolder.FullName, "Dofus.exe");
        var scriptPath = Path.Combine(ddcFolder.FullName, "scripts/bepinex-run-until");
        var cmd = $"node \"{scriptPath}\" \"{dofusPath}\" \"{untilLog}\"";
        Run(cmd);
    }

    static void SetupInterop()
    {
        var dofusPath = Path.Combine(dofusFolder.FullName, "BepInEx", "interop");
        var ddcPath = Path.Combine(ddcFolder.FullName, "Interop");
        Directory.CreateDirectory(ddcPath);
        CopyDirectory(ddcPath, new DirectoryInfo(dofusPath));
    }

    static void BuildModelExtractor()
    {
        BuildProject("DDC.ModelExtractor");
        CopyPlugins("DDC.ModelExtractor");

        var configPath = Path.Combine(dofusFolder.FullName, "BepInEx", "config", "DDC.ModelExtractor.cfg");
        var generatedFolder = Path.Combine(ddcFolder.FullName, "DDC", "Generated");
        File.WriteAllText(configPath,
            @$"
            [General]
            OutputDirectory = {generatedFolder}
            ");

        RunGame("DDC_type model generation complete.");
        CleanPlugins();
    }

    static void BuildExtractor()
    {
        BuildProject("DDC.Extractor");
        CopyPlugins("DDC.Extractor");

        var configPath = Path.Combine(dofusFolder.FullName, "BepInEx", "config", "DDC.Extractor.cfg");
        var extractedFolder = Path.Combine(dofusFolder.FullName, "extracted-data");
        File.WriteAllText(configPath,
            @$"
            [General]
            OutputDirectory = {extractedFolder}
            ");

        RunGame("DDC_data extraction complete.");
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

    static void CopyPlugins(string projectName)
    {
        var pluginsFolder = Path.Combine(dofusFolder.FullName, "BepInEx", "plugins");
        var bin = Path.Combine(ddcFolder.FullName, projectName, "bin/Release/net6.0/DDC*.dll");
        Run($"copy {bin} {pluginsFolder}");
        //CopyDirectory(pluginsFolder, new DirectoryInfo(Path.Combine(ddcFolder.FullName, projectName, "bin/Release/net6.0")));
    }

    static void CreateBepInConfigFolder()
    {
        var configFolder = Path.Combine(dofusFolder.FullName, "BepInEx", "config");
        //Run($"md {configFolder} -Force");
        Directory.CreateDirectory(configFolder);
    }

    static void BuildProject(string projectName)
    {
        var csproj = Path.Combine(ddcFolder.FullName, projectName, projectName + ".csproj");
        var cmd = $"dotnet build {csproj} --configuration Release --no-restore";
        Run(cmd);
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

}

public static class Extensions
{
    public static void DeleteSafe(this FileInfo file)
    {
        try
        {
            file.Delete();
        }
        catch (UnauthorizedAccessException e)
        {
            file.DeleteSafe();
        }
    }

}