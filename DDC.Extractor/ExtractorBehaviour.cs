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
using Core.DataCenter.Metadata.Breed;
using Core.DataCenter.Metadata.Effect;
using Core.DataCenter.Metadata.Effect.Instance;
using Core.DataCenter.Metadata.Item;
using Core.DataCenter.Metadata.Monster;
using Core.DataCenter.Metadata.Quest.TreasureHunt;
using Core.DataCenter.Metadata.Spell;
using Core.DataCenter.Metadata.Stat;
using Core.DataCenter.Metadata.World;
using Core.DataCenter.Types;
using Core.Engine.Messages;
using Core.Localization;
using DDC.Extractor.Converters;
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
        //JsonSerializerOptions.Converters.Add(null);
        yield return Wait(1);

        Extractor.Logger.LogInfo("Start extracting data...");

        //var items15 = DataCenterModule.itemsRoot.QuerySmallerThanLevel(15);
        //string path = Path.Join(Extractor.OutputDirectory, "ddc/items-15-b.json");
        ////using FileStream stream = File.OpenWrite(path);
        ////var converter = new ItemConverter();
        ////JsonSerializer.Serialize(stream, items._items.Where(i => i != null).Select(converter.Convert), JsonSerializerOptions);
        ////stream.Flush();
        ////var itemsJson = items?._items?.Select(i => i?.name + " (" + i?.level + ")")?.ToList() ?? new List<string>();
        ////var str = string.Join(", ", itemsJson);
        ////Extractor.Logger.LogInfo($"Item names: " + str);
        //var json = Json.serialize(items15);
        //yield return File.WriteAllTextAsync(path, json);

        //yield return WaitForCompletion(ExtractRaws("breeds.json", DataCenterModule.GetDataRoot<BreedsRoot>()));
        //yield return WaitForCompletion(ExtractRaws("items.json", DataCenterModule.GetDataRoot<ItemsRoot>()));
        //yield return WaitForCompletion(ExtractRaws("spells.json", DataCenterModule.GetDataRoot<SpellsRoot>()));
        //yield return WaitForCompletion(ExtractRaws("spell-levels.json", DataCenterModule.GetDataRoot<SpellLevelsRoot>()));
        //yield return WaitForCompletion(ExtractRaws("effects.json", DataCenterModule.GetDataRoot<EffectsRoot>()));
        //yield return WaitForCompletion(ExtractRaws("monsters.json", DataCenterModule.GetDataRoot<MonstersRoot>()));
        //yield return WaitForCompletion(ExtractRaws("evolutive-effects.json", DataCenterModule.GetDataRoot<EvolutiveEffectsRoot>()));



        //DataCenterModule.effectsRoot
        //DataCenterModule.evolutiveEffectsRoot
        //DataCenterModule.itemSetsRoot
        //DataCenterModule.legendaryPowersCategoriesRoot
        //DataCenterModule.spellBombsRoot
        //DataCenterModule.spellConversionsRoot
        //DataCenterModule.spellPairsRoot
        //DataCenterModule.spellStatesRoot
        //DataCenterModule.spellTypesRoot
        //DataCenterModule.spellVariantsRoot
        //DataCenterModule.characteristicsRoot
        //DataCenterModule.characteristicCategoriesRoot
        //DataCenterModule.monstersRoot
        //DataCenterModule.monsterMiniBossRoot
        //DataCenterModule.monsterRacesRoot
        //DataCenterModule.monsterSuperRacesRoot
        //DataCenterModule.mountsRoot
        //DataCenterModule.mountFamilyRoot
        //DataCenterModule.dungeonsRoot
        //DataCenterModule.fightScenariosRoot
        //DataCenterModule.forgettableSpellsRoot
        //DataCenterModule.featureDescriptionsRoot
        //DataCenterModule.evolutiveItemTypesRoot
        //DataCenterModule.breedsRoot
        //DataCenterModule.breedRolesRoot
        //DataCenterModule.achievementsRoot

        //var scenarios = DataCenterModule.fightScenariosRoot.GetObjects()._items.Where(s => s != null).Select(s => s.name).ToList();
        //Extractor.Logger.LogInfo($"Scenarios: " + string.Join(", ", scenarios));

        if (true)
        {
            yield return WaitForCompletion(ExtractRoots.GetAllRoots());
            //yield return WaitForCompletion(ExtractRoots.ExtractRoot(DataCenterModule.breedsRoot));
            //yield return WaitForCompletion(ExtractRoots.ExtractRoot(DataCenterModule.breedRolesRoot));
            //yield return WaitForCompletion(ExtractRoots.ExtractRoot(DataCenterModule.customModeBreedSpellsRoot));
            //yield return WaitForCompletion(ExtractRoots.ExtractRoot(DataCenterModule.dungeonsRoot));
            //yield return WaitForCompletion(ExtractRoots.ExtractRoot(DataCenterModule.characteristicsRoot));
            //yield return WaitForCompletion(ExtractRoots.ExtractRoot(DataCenterModule.characteristicCategoriesRoot));
            //yield return WaitForCompletion(ExtractRoots.ExtractRoot(DataCenterModule.legendaryPowersCategoriesRoot));
            //yield return WaitForCompletion(ExtractRoots.ExtractRoot(DataCenterModule.legendaryTreasureHuntsRoot));
            //yield return WaitForCompletion(ExtractRoots.ExtractRoot(DataCenterModule.itemSuperTypesRoot));
            //yield return WaitForCompletion(ExtractRoots.ExtractRoot(DataCenterModule.itemTypesRoot));
            //yield return WaitForCompletion(ExtractRoots.ExtractRoot(DataCenterModule.itemSetsRoot));
            //yield return WaitForCompletion(ExtractRoots.ExtractRoot(DataCenterModule.itemsRoot));
            //yield return WaitForCompletion(ExtractRoots.ExtractRoot(DataCenterModule.evolutiveItemTypesRoot));
            //yield return WaitForCompletion(ExtractRoots.ExtractRoot(DataCenterModule.effectsRoot));
            //yield return WaitForCompletion(ExtractRoots.ExtractRoot(DataCenterModule.evolutiveEffectsRoot));
            //yield return WaitForCompletion(ExtractRoots.ExtractRoot(DataCenterModule.spellsRoot));
            //yield return WaitForCompletion(ExtractRoots.ExtractRoot(DataCenterModule.spellLevelsRoot));
            //yield return WaitForCompletion(ExtractRoots.ExtractRoot(DataCenterModule.spellPairsRoot));
            //yield return WaitForCompletion(ExtractRoots.ExtractRoot(DataCenterModule.spellVariantsRoot));
            //yield return WaitForCompletion(ExtractRoots.ExtractRoot(DataCenterModule.spellBombsRoot));
            //yield return WaitForCompletion(ExtractRoots.ExtractRoot(DataCenterModule.spellConversionsRoot));
            //yield return WaitForCompletion(ExtractRoots.ExtractRoot(DataCenterModule.spellTypesRoot));
            //yield return WaitForCompletion(ExtractRoots.ExtractRoot(DataCenterModule.monstersRoot));
            //yield return WaitForCompletion(ExtractRoots.ExtractRoot(DataCenterModule.monsterMiniBossRoot));
            //yield return WaitForCompletion(ExtractRoots.ExtractRoot(DataCenterModule.monsterRacesRoot));
            //yield return WaitForCompletion(ExtractRoots.ExtractRoot(DataCenterModule.monsterSuperRacesRoot));
            //yield return WaitForCompletion(ExtractRoots.ExtractRoot(DataCenterModule.featureDescriptionsRoot));
            //yield return WaitForCompletion(ExtractRoots.ExtractRoot(DataCenterModule.optionalFeaturesRoot));
            //yield return WaitForCompletion(ExtractRoots.ExtractRoot(DataCenterModule.achievementCategoriesRoot));
            //yield return WaitForCompletion(ExtractRoots.ExtractRoot(DataCenterModule.achievementsRoot));
        }
        else
        {
            yield return WaitForCompletion(ExtractModelTypes.getAllModels());
        }

        //yield return WaitForCompletion(ExtractData("ddc/item-super-types.json", DataCenterModule.itemSuperTypesRoot, new ItemSuperTypeConverter()));
        //yield return WaitForCompletion(ExtractData("ddc/item-types.json", DataCenterModule.itemTypesRoot, new ItemTypeConverter()));
        //yield return WaitForCompletion(ExtractData("ddc/items.json", DataCenterModule.itemsRoot, new ItemConverter()));
        //yield return WaitForCompletion(ExtractData("ddc/monsters.json", DataCenterModule.itemsRoot, new Monst()));


        //yield return WaitForCompletion(ExtractData("ddc/point-of-interest.json", DataCenterModule.GetDataRoot<PointOfInterestRoot>(), new PointOfInterestConverter()));
        //yield return WaitForCompletion(ExtractData("ddc/map-positions.json", DataCenterModule.GetDataRoot<MapPositionsRoot>(), new MapPositionsConverter()));
        //yield return WaitForCompletion(ExtractLocale("ddc/de.i18n.json", "Dofus_Data/StreamingAssets/Content/I18n/de.bin"));
        //yield return WaitForCompletion(ExtractLocale("ddc/en.i18n.json", "Dofus_Data/StreamingAssets/Content/I18n/en.bin"));
        //yield return WaitForCompletion(ExtractLocale("ddc/es.i18n.json", "Dofus_Data/StreamingAssets/Content/I18n/es.bin"));
        yield return WaitForCompletion(ExtractLocale("ddc/fr.i18n.json", "Dofus_Data/StreamingAssets/Content/I18n/fr.bin"));
        //yield return WaitForCompletion(ExtractLocale("ddc/pt.i18n.json", "Dofus_Data/StreamingAssets/Content/I18n/pt.bin"));

        Extractor.Logger.LogInfo("DDC data extraction complete.");

        //Application.Quit(0);
    }

    static async Task ExtractRaw<TData>(string filename, MetadataRoot<TData> root)
    {
        string dataTypeName = typeof(TData).Name;
        string path = Path.Join(Extractor.OutputDirectory, filename);

        Extractor.Logger.LogInfo($"Extracting data of type {dataTypeName}...");

        Il2CppSystem.Collections.Generic.List<TData> data = root.GetObjects();
        //TSerializedData[] arr = data._items.Take(data.Count).Where(i => i != null).Select(converter.Convert).ToArray();

        await using FileStream stream = File.OpenWrite(path);
        //await JsonSerializer.SerializeAsync(stream, data, JsonSerializerOptions);

        StringBuilder sb = new();
        foreach (var item in data)
        {
            var props = item.GetType().GetProperties();
            for (int i = 0; i < props.Length; i++)
            {
                var prop = props[i];
                if (prop.PropertyType.Name.EndsWith("Ptr"))
                    continue;
                else
                {
                    sb.Append($"\"{prop.Name}\": {prop.GetValue(item)}");
                    if (i != props.Length - 1)
                        sb.Append(',');
                }
            }
        }
        stream.Flush();

        Extractor.Logger.LogInfo($"Extracted data of type {dataTypeName} to {path}.");
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

    static async Task ExtractData<TData, TSerializedData>(string filename, MetadataRoot<TData> root, IConverter<TData, TSerializedData> converter)
    {
        string dataTypeName = typeof(TData).Name;
        string path = Path.Join(Extractor.OutputDirectory, filename);

        Extractor.Logger.LogInfo($"Extracting data of type {dataTypeName}...");

        Il2CppSystem.Collections.Generic.List<TData> data = root.GetObjects();
        TSerializedData[] arr = data._items.Take(data.Count).Where(i => i != null).Select(converter.Convert).ToArray();

        await using FileStream stream = File.OpenWrite(path);
        await JsonSerializer.SerializeAsync(stream, arr, JsonSerializerOptions);
        stream.Flush();

        Extractor.Logger.LogInfo($"Extracted data of type {dataTypeName} to {path}.");
    }

    static async Task ExtractRaws<TData>(string filename, MetadataRoot<TData> root, int count = 0)
    {
        string dataTypeName = typeof(TData).Name;
        string path = Path.Join(Extractor.OutputDirectory, filename);

        Extractor.Logger.LogInfo($"Extracting data of type {dataTypeName}...");

        Il2CppSystem.Collections.Generic.List<TData> data = root.GetObjects();
        if (count == 0) count = data.Count;
        TData[] arr = data._items.Take(count).ToArray();

        await using FileStream stream = File.OpenWrite(path);
        await JsonSerializer.SerializeAsync(stream, arr, JsonSerializerOptions);
        stream.Flush();

        Extractor.Logger.LogInfo($"Extracted data of type {dataTypeName} to {path}.");
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
