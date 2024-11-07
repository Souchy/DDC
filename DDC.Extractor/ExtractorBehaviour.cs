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
using Il2CppInterop.Runtime.Runtime;
using Il2CppInterop.Runtime;
using Il2CppSystem;
using Il2CppSystem.IO;
using Metadata.Enums;
using UnityEngine;
using static ers;
using File = System.IO.File;
using FileStream = System.IO.FileStream;
using Path = System.IO.Path;
using Newtonsoft.Json;

namespace DDC.Extractor;

public class ExtractorBehaviour : MonoBehaviour
{
    public static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        IncludeFields = false,
        WriteIndented = true,
        NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
        IgnoreReadOnlyProperties = false,
    };

    void Start() => StartCoroutine(StartCoroutine().WrapToIl2Cpp());

    static IEnumerator StartCoroutine()
    {
        yield return Wait(1);
        // missing targetMask dans effectinstance (enemis,alliés pour zones)
        // EffectInstance.zoneDescr converting
        // enum value converting
        // remove properties that dont have fields from .cs
        // remove static fields from .cs
        Extractor.Logger.LogInfo("Start extracting data...");

        if (false)
        {

            Prototyping.TestItemsEffects();
            //Prototyping.TestSpells();
            //Prototyping.TestWeapons();
        }
        else
        if (true)
        {
            yield return WaitForCompletion(ExtractRoots.ExtractAll());

            yield return WaitForCompletion(ExtractLocale("i18n/de.i18n.json", "Dofus_Data/StreamingAssets/Content/I18n/de.bin"));
            yield return WaitForCompletion(ExtractLocale("i18n/en.i18n.json", "Dofus_Data/StreamingAssets/Content/I18n/en.bin"));
            yield return WaitForCompletion(ExtractLocale("i18n/es.i18n.json", "Dofus_Data/StreamingAssets/Content/I18n/es.bin"));
            yield return WaitForCompletion(ExtractLocale("i18n/fr.i18n.json", "Dofus_Data/StreamingAssets/Content/I18n/fr.bin"));
            yield return WaitForCompletion(ExtractLocale("i18n/pt.i18n.json", "Dofus_Data/StreamingAssets/Content/I18n/pt.bin"));
        }

        Extractor.Logger.LogInfo("DDC_data extraction complete.");
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
        var folder = path[0..path.LastIndexOf("/")];
        System.IO.Directory.CreateDirectory(folder);

        await using FileStream stream = File.OpenWrite(path);
        await System.Text.Json.JsonSerializer.SerializeAsync(stream, localizationTable, JsonSerializerOptions);
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
