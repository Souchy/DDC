using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Xml.Linq;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using Core.DataCenter;
using Core.DataCenter.Metadata.Alliance;
using Core.DataCenter.Metadata.Appearance;
using Core.DataCenter.Metadata.Breed;
using Core.DataCenter.Metadata.Effect;
using Core.DataCenter.Metadata.Effect.Instance;
using Core.DataCenter.Metadata.Idol;
using Core.DataCenter.Metadata.Item;
using Core.DataCenter.Metadata.Monster;
using Core.DataCenter.Metadata.Quest;
using Core.DataCenter.Metadata.Quest.TreasureHunt;
using Core.DataCenter.Metadata.Social;
using Core.DataCenter.Metadata.Spell;
using Core.DataCenter.Metadata.Stat;
using Core.DataCenter.Metadata.World;
using Core.DataCenter.Types;
using Core.Engine.Messages;
using Core.Localization;
using Il2CppSystem.IO;
using Metadata.Enums;
using UnityEngine;
using static ers;
using File = System.IO.File;
using FileStream = System.IO.FileStream;
using Path = System.IO.Path;

namespace DDC.Extractor;

public class ExtractorBehaviour : MonoBehaviour
{
    public static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        IncludeFields = false,
        WriteIndented = true,
        NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
    };

    void Start() => StartCoroutine(StartCoroutine().WrapToIl2Cpp());

    static IEnumerator StartCoroutine()
    {
        yield return Wait(1);

        Extractor.Logger.LogInfo("Start extracting data...");
        if (true)
        {
            //yield return WaitForCompletion(ExtractRoots.ExtractRoot(DataCenterModule.idolsRoot));
            //yield return WaitForCompletion(ExtractRoots.ExtractRoot(DataCenterModule.idolsPresetIconsRoot));
            //yield return WaitForCompletion(ExtractRoots.ExtractRoot(DataCenterModule.socialTagsTypesRoot));
            //yield return WaitForCompletion(ExtractRoots.ExtractRoot(DataCenterModule.skinPositionsRoot));
            ExtractRoots.rootTypes.Add(typeof(Idols));
            ExtractRoots.rootTypes.Add(typeof(IdolsPresetIcons));
            ExtractRoots.rootTypes.Add(typeof(SocialTagsTypes));
            ExtractRoots.rootTypes.Add(typeof(SkinPositions));
            //DataCenterModule.LoadData();
            var roots = ExtractRoots.FindRoots();
            Extractor.Logger.LogMessage("Root types (" + ExtractRoots.rootTypes.Count + "): " + string.Join(", ", ExtractRoots.rootTypes.Select(t => t.Name)));
            yield return WaitForCompletion(ExtractRoots.GetAllRoots(roots));
        }
        else
        {
            yield return WaitForCompletion(ExtractModelTypes.GetAllModels());
        }
        yield return WaitForCompletion(ExtractLocale("ddc/json/i18n/de.i18n.json", "Dofus_Data/StreamingAssets/Content/I18n/de.bin"));
        yield return WaitForCompletion(ExtractLocale("ddc/json/i18n/en.i18n.json", "Dofus_Data/StreamingAssets/Content/I18n/en.bin"));
        yield return WaitForCompletion(ExtractLocale("ddc/json/i18n/es.i18n.json", "Dofus_Data/StreamingAssets/Content/I18n/es.bin"));
        yield return WaitForCompletion(ExtractLocale("ddc/json/i18n/fr.i18n.json", "Dofus_Data/StreamingAssets/Content/I18n/fr.bin"));
        yield return WaitForCompletion(ExtractLocale("ddc/json/i18n/pt.i18n.json", "Dofus_Data/StreamingAssets/Content/I18n/pt.bin"));

        Extractor.Logger.LogInfo("DDC data extraction complete.");

        Application.Quit(0);
    }

    static async Task ExtractLocale(string filename, string binFile)
    {
        Extractor.Logger.LogInfo($"Extracting locale from {binFile}...");
        LocalizationTable table = LocalizationTable.ReadFrom(binFile);

        Dictionary<int, string> entries = new();
        foreach (Il2CppSystem.Collections.Generic.KeyValuePair<int, uint> entry in table.m_header.m_integerKeyedOffsets)
        {
            if (!table.TryLookup(entry.Key, out string output))
            {
                continue;
            }

            entries[entry.Key] = output;
        }

        Models.I18N.LocalizationTable localizationTable = new() { LanguageCode = table.m_header.languageCode, Entries = entries };

        string path = Path.Join(Extractor.OutputDirectory, filename);
        await using FileStream stream = File.OpenWrite(path);
        await JsonSerializer.SerializeAsync(stream, localizationTable, JsonSerializerOptions);
        stream.Flush();

        Extractor.Logger.LogInfo($"Extracted locale {table.m_header.languageCode} to {path}.");
    }

    static IEnumerator Wait(float seconds)
    {
        float startTime = Time.time;
        while (Time.time - startTime < seconds)
        {
            yield return null;
        }
    }

    static IEnumerator WaitForCompletion(Task task)
    {
        while (!task.IsCompleted)
        {
            yield return null;
        }
    }
}
