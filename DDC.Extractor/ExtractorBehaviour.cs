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
        // missing targetMask dans effectinstance (enemis,alliés pour zones)
        // EffectInstance.zoneDescr converting
        // enum value converting
        // remove properties that dont have fields from .cs
        // remove static fields from .cs
        Extractor.Logger.LogInfo("Start extracting data...");

        if (false)
        {
            yield return WaitForCompletion(ExtractModelTypes.GetAllModels());
            Extractor.Logger.LogInfo("DDC model generation complete.");
        }
        else
        {
            var wep = Weapons.GetItemById(25219);
            Extractor.Logger.LogWarning("wep: " + wep + ", " + wep.GetType() + ", " + wep.GetType().BaseType);

            //DataCenterModule.LoadData();

            //var weapons = DataCenterModule.GetDataRoot<MetadataRoot<Weapons>>();
            //Extractor.Logger.LogWarning("weapons: " + weapons);
            var baguette = DataCenterModule.itemsRoot.GetObjectById(25219);
            Extractor.Logger.LogWarning("baguette: " + baguette.GetType() + ", " + baguette.GetType().BaseType);
            //baguette.possibleEffects
            //var weapons = DataCenterModule.itemsRoot.GetObjects()._items.Where(i => i is Weapons).ToList();
            //Extractor.Logger.LogWarning("weapons: " + weapons.Count);
            var vitality = baguette.possibleEffects._items.First(e => e.effectId == ActionId.CharacterBoostVitality);
            Extractor.Logger.LogWarning("baguette effect: " + vitality + ", " + vitality.GetType());

            //var weap = (Weapons) baguette;
            //Extractor.Logger.LogWarning("weap: " + weap);
            //baguette = DataCenterModule.s_itemsRootCached.GetObjectById(25219);
            //Extractor.Logger.LogWarning("baguette: " + baguette.GetType() + ", " + baguette.GetType().BaseType);
            //var weapons = DataCenterModule.s_itemsRootCached.GetObjects()._items.Where(i => i is Weapons).ToList();
            //Extractor.Logger.LogWarning("weapons: " + weapons.Count);


            //Extractor.Logger.LogMessage("Root types (" + ExtractRoots.rootTypes.Count + "): " + string.Join(", ", ExtractRoots.rootTypes.Select(t => t.Name)));
            //var spellLevel = DataCenterModule.spellLevelsRoot.GetObjectById(41053); //12984);
            //var eff = spellLevel.effects[0];
            //Extractor.Logger.LogInfo($"SpellLevel.EffectInstance: {eff.spellId}, e {eff.effectId}, {eff.effectUid}, c {eff.category}, z {eff.zoneDescr}={eff.zoneDescr.param1}x{eff.zoneDescr.shape}, // {eff.zoneSize}, {eff.zoneShape}");
            //var spellLevel2 = (Generated.Core.DataCenter.Metadata.Spell.SpellLevels) ExtractRoots.ConvertType(spellLevel.GetType(), spellLevel);
            //var eff2 = spellLevel2.effects[0];
            //Extractor.Logger.LogInfo($"SpellLevel2.EffectInstance: {eff2.spellId}, e {eff2.effectId}, {eff2.effectUid}, c {eff2.category}, z {eff2.zoneDescr}={eff2.zoneDescr.param1}x{eff2.zoneDescr.shape}, // {eff2.zoneSize}, {eff2.zoneShape}");
            //var eff3 = (Generated.Core.DataCenter.Metadata.Effect.EffectInstance) ExtractRoots.ConvertType(eff.GetType(), eff);
            //Extractor.Logger.LogInfo($"EffectInstance2: {eff3.spellId}, e {eff3.effectId}, {eff3.effectUid}, c {eff3.category}, z {eff3.zoneDescr}={eff3.zoneDescr.param1}x{eff3.zoneDescr.shape}, // {eff3.zoneSize}, {eff3.zoneShape}");

            ExtractRoots.rootTypes.Add(typeof(Idols));
            ExtractRoots.rootTypes.Add(typeof(IdolsPresetIcons));
            ExtractRoots.rootTypes.Add(typeof(SocialTagsTypes));
            ExtractRoots.rootTypes.Add(typeof(SkinPositions));
            //var roots = ExtractRoots.FindRoots();
            //yield return WaitForCompletion(ExtractRoots.GetAllRoots(roots));
            //yield return WaitForCompletion(ExtractLocale("ddc/json/i18n/de.i18n.json", "Dofus_Data/StreamingAssets/Content/I18n/de.bin"));
            //yield return WaitForCompletion(ExtractLocale("ddc/json/i18n/en.i18n.json", "Dofus_Data/StreamingAssets/Content/I18n/en.bin"));
            //yield return WaitForCompletion(ExtractLocale("ddc/json/i18n/es.i18n.json", "Dofus_Data/StreamingAssets/Content/I18n/es.bin"));
            //yield return WaitForCompletion(ExtractLocale("ddc/json/i18n/fr.i18n.json", "Dofus_Data/StreamingAssets/Content/I18n/fr.bin"));
            //yield return WaitForCompletion(ExtractLocale("ddc/json/i18n/pt.i18n.json", "Dofus_Data/StreamingAssets/Content/I18n/pt.bin"));
            //Extractor.Logger.LogInfo("DDC data extraction complete.");
        }

        //Application.Quit(0);
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
