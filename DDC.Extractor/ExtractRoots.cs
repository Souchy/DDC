using Core.DataCenter;
using Core.DataCenter.Metadata.Alliance;
using Core.DataCenter.Metadata.Appearance;
using Core.DataCenter.Metadata.Bonus;
using Core.DataCenter.Metadata.Bonus.Criteria;
using Core.DataCenter.Metadata.Breed;
using Core.DataCenter.Metadata.Challenge;
using Core.DataCenter.Metadata.Effect;
using Core.DataCenter.Metadata.Effect.Instance;
using Core.DataCenter.Metadata.House;
using Core.DataCenter.Metadata.Idol;
using Core.DataCenter.Metadata.Item;
using Core.DataCenter.Metadata.Job;
using Core.DataCenter.Metadata.Monster;
using Core.DataCenter.Metadata.OptionalFeatures;
using Core.DataCenter.Metadata.Progression;
using Core.DataCenter.Metadata.Quest;
using Core.DataCenter.Metadata.Quest.Objective;
using Core.DataCenter.Metadata.Social;
using Core.DataCenter.Metadata.Spell;
using Core.DataCenter.Metadata.World;
using Il2CppInterop.Runtime.InteropTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using static Il2CppSystem.Net.Http.Headers.Parser;

namespace DDC.Extractor;
public class ExtractRoots
{
    public static List<Type> dangerousTypes = [
        typeof(Idols), typeof(IdolsPresetIcons), typeof(SocialTagsTypes), typeof(SkinPositions),
        //typeof(Houses), typeof(HintCategory), typeof(Hints)
    ];
    //public static List<string> dangerousProperties = ["name", "undiacriticalName", "unDiacriticalName", "entityName", "durationString", "description", "theoreticalDescription", "descriptionForTooltip", "theoreticalDescriptionForTooltip", "theoreticalShortDescriptionForTooltip"];
    public static List<Type> rootTypes = []; //[typeof(Monsters), typeof(Items), typeof(Spells), typeof(SpellLevels), typeof(ItemTypes), typeof(ItemSuperTypes), typeof(Effects)];
    public const int FAST_TAKES = 5;
    public const bool debug = false;

    public static List<(IEnumerable items, ICollection items2, Type itemType, MethodInfo methAdd)> FindRoots()
    {
        var roots = typeof(DataCenterModule).GetProperties().Where(p => p.Name.EndsWith("Root")).
            Select(prop =>
            {
                try
                {
                    //if (prop.Name != "itemsRoot") // && prop.Name != "spellsRoot" && prop.Name != nameof(DataCenterModule.spellLevelsRoot))
                    //    return null;

                    return prop.GetValue(typeof(DataCenterModule));
                }
                catch (Exception ex)
                {
                    Extractor.Logger.LogWarning("Exception GetAllRoots (" + prop.Name + "): " + ex.Message); // + " -> " + ex.StackTrace);
                    return null;
                }
            })
            .Where(r => r != null)
            .Select(v =>
            {
                var rootType = v.GetType();
                var rootTypeBase = rootType.BaseType;
                var meth0 = rootTypeBase.GetMethod("GetObjects");
                var objects = meth0.Invoke(v, []);
                //Extractor.Logger.LogInfo($"Transforming root objects: " + objects + ": " + objects?.GetType().FullName);
                var items = objects.GetType().GetProperty("_items").GetValue(objects) as IEnumerable;
                var size = objects.GetType().GetProperty("_size").GetValue(objects);
                //Extractor.Logger.LogInfo($"Transforming root items (" + size + "): " + items + ": " + items?.GetType().FullName);
                var itemType = items.GetType().GenericTypeArguments[0];
                //Extractor.Logger.LogInfo($"Transforming root list type:  {itemType.Name}"); // + items.Count);
                var genericType = Converter.GetCorrespondingType(items.GetType());
                //Extractor.Logger.LogInfo($"Transforming root new list type: " + genericType.FullName);
                var items2 = Activator.CreateInstance(genericType) as ICollection;
                var methAdd = genericType.GetMethod("Add");
                rootTypes.Add(itemType);
                return (items, items2, itemType, methAdd);
            })
            .ToList();
        return roots;
    }

    public static async Task GetAllRoots(List<(IEnumerable items, ICollection items2, Type itemType, MethodInfo methAdd)> roots)
    {
        try
        {
            Extractor.Logger.LogInfo($"Extracting ROOTs (" + roots.Count() + ") =================");

            string path = Path.Join(Extractor.OutputDirectory);
            if (Directory.Exists(path))
                Directory.Delete(path, true);

            foreach (var root in roots)
            {
                await ExtractRoot2(root.items, root.items2, root.itemType, root.methAdd);
            }
            Extractor.Logger.LogInfo($"Extracting ROOTs DONE =================");
        }
        catch (Exception ex)
        {
            Extractor.Logger.LogInfo("Exception GetAllRoots: " + ex.Message + " -> " + ex.StackTrace);
        }
    }


    public static async Task asdf()
    {
        // Roots are properties of DataCenterModule
        var roots = typeof(DataCenterModule).GetProperties().Where(p => p.Name.EndsWith("Root")) // && !p.Name.StartsWith("s_"));
        .Select(prop =>
        {
            try
            {
                //if (prop.Name != "itemsRoot") // && prop.Name != "spellsRoot" && prop.Name != nameof(DataCenterModule.spellLevelsRoot))
                //    return null;
                if (dangerousTypes.Select(t => t.Name.ToLower() + "root").Contains(prop.Name.ToLower()))
                {
                    Extractor.Logger.LogMessage($"Ignoring property: " + prop.Name);
                    return null;
                }
                return prop.GetValue(typeof(DataCenterModule));
            }
            catch (Exception ex)
            {
                Extractor.Logger.LogWarning("Exception asdf (" + prop.Name + "): " + ex.Message); // + " -> " + ex.StackTrace);
                return null;
            }
        })
        .Where(r => r != null);

        try
        {
            Extractor.Logger.LogInfo($"Extracting ROOTs (" + roots.Count() + ") =================");
            Extractor.Logger.LogInfo(string.Join(", ", roots.Select(p => p.GetType().Name)));
            string path = Path.Join(Extractor.OutputDirectory);
            if (Directory.Exists(path))
                Directory.Delete(path, true);
            await Task.Delay(5000);
            foreach (var v in roots)
            {
                //await Task.Delay(1000);
                try
                {
                    //Extractor.Logger.LogInfo($"Datacenter prop: " + prop.Name);
                    //Extractor.Logger.LogInfo($"Datacenter prop: " + v.GetType().Name);
                    //var v = prop.GetValue(typeof(DataCenterModule));
                    //if (v == null) continue;
                    var rootType = v.GetType();
                    var rootTypeBase = rootType.BaseType;
                    var meth0 = rootTypeBase.GetMethod("GetObjects");
                    var objects = meth0.Invoke(v, []);
                    //Extractor.Logger.LogInfo($"Transforming root objects: " + objects + ": " + objects?.GetType().FullName);
                    var objectsvalues = objects.GetType().GetProperty("_items").GetValue(objects);
                    var items = objectsvalues as IEnumerable;

                    var size = objects.GetType().GetProperty("_size").GetValue(objects);
                    //Extractor.Logger.LogInfo($"Transforming root items (" + size + "): " + items + ": " + items?.GetType().FullName);
                    var itemType = items.GetType().GenericTypeArguments[0];
                    if (dangerousTypes.Contains(itemType))
                        return;
                    //Extractor.Logger.LogInfo($"Transforming root list type:  {itemType.Name}"); // + items.Count);
                    var genericType = Converter.GetCorrespondingType(items.GetType());
                    //Extractor.Logger.LogInfo($"Transforming root new list type: " + genericType.FullName);
                    var items2 = Activator.CreateInstance(genericType) as ICollection;
                    var methAdd = genericType.GetMethod("Add");
                    rootTypes.Add(itemType);

                    //return (items, items2, itemType, methAdd);
                    await ExtractRoot2(items, items2, itemType, methAdd);
                }
                catch (Exception ex)
                {
                    Extractor.Logger.LogWarning("Exception asdf prop (" + v.GetType().Name + "): " + ex.Message); // + " -> " + ex.StackTrace);
                    continue;
                }
            }
            Extractor.Logger.LogInfo($"Extracting ROOTs DONE =================");
        }
        catch (Exception ex)
        {
            Extractor.Logger.LogInfo("Exception asdf overall: " + ex.Message + " -> " + ex.StackTrace);
        }
    }

    public static async Task ExtractRoot2(IEnumerable items, ICollection items2, Type itemType, MethodInfo methAdd) //object root0 = null)
    {
        if (dangerousTypes.Contains(itemType))
            return;
        try
        {
            var folder = itemType.Namespace.Replace(".", "/") + "/";
            string path = Path.Join(Extractor.OutputDirectory, folder);
            Extractor.Logger.LogInfo($"Extracting ROOT of type {itemType.Name} to {path}.");

            //var methAdd = items2.GetType().GetMethod("Add");
            int count = 0;
            foreach (var item in items)
            {
                if (item is null) continue;
                //if (count == FAST_TAKES) break;
                count++;
                var item2 = Converter.ConvertType(item, count.ToString());
                if (item2 != null)
                    methAdd.Invoke(items2, [item2]);
            }

            //if (itemType == typeof(Core.DataCenter.Metadata.Item.ItemSets))
            //{
            //    foreach (var i in items2)
            //    {
            //        var a = i as Generated.Core.DataCenter.Metadata.Item.ItemSets;
            //        var ef = a.effects[1].values[0] as Generated.Core.DataCenter.Metadata.Effect.Instance.EffectInstanceDice;
            //        Extractor.Logger.LogWarning("ItemSets effect: " + ef + ", " + ef.diceNum);
            //        var s1 = JsonSerializer.Serialize<object>(a);
            //        var s2 = JsonSerializer.Serialize<object>(a.effects);
            //        var s3 = JsonSerializer.Serialize<object>(a.effects[1].values);
            //        var s4 = JsonSerializer.Serialize<object>(a.effects[1].values[0]);
            //        Extractor.Logger.LogWarning("Pano: " + s1);
            //        Extractor.Logger.LogWarning("Effets: " + s2);
            //        Extractor.Logger.LogWarning("Effets[1].values: " + s3);
            //        Extractor.Logger.LogWarning("Effets[1].values[0]: " + s4);
            //        break;
            //    }
            //}
            System.IO.Directory.CreateDirectory(path);
            //Extractor.Logger.LogInfo($"Created Directory. " + path);

            await using FileStream stream = File.OpenWrite(path + "/" + itemType.Name + ".json");
            await JsonSerializer.SerializeAsync<object>(stream, items2, ExtractorBehaviour.JsonSerializerOptions);
            //await Utf8Json.JsonSerializer.SerializeAsync(stream, items2);
            stream.Flush();


            //if (itemType == typeof(ItemSets))
            //{
            //    foreach (var i in items)
            //    {
            //        Extractor.Logger.LogInfo("---- Pano bouftou: ");
            //        var json = Newtonsoft.Json.JsonConvert.SerializeObject((ItemSets) i, ExtractorBehaviour.NewtonsoftSettings);
            //        Extractor.Logger.LogWarning(json);
            //        Extractor.Logger.LogInfo("---- Pano bouftou ^^^");
            //        break;
            //    }
            //}
            //JsonSerializer
            ////var json = Newtonsoft.Json.JsonConvert.SerializeObject(items2, ExtractorBehaviour.NewtonsoftSettings);
            //using (var sw = new StreamWriter(path + "/" + itemType.Name + "-newton.json"))
            //using (var writer = new Newtonsoft.Json.JsonTextWriter(sw))
            //{
            //ExtractorBehaviour.NewtonsoftSerializer.Serialize(writer, items2);
            //    //serializer.Serialize(writer, product);
            //    // {"ExpiryDate":new Date(1230375600000),"Price":0}
            //}
            //var json = Newtonsoft.Json.JsonConvert.SerializeObject((Il2CppSystem.Object) items, ExtractorBehaviour.NewtonsoftSettings);
            // items2, ExtractorBehaviour.JsonSerializerOptions);
            //new Newtonsoft.Json.JsonWriter()
            //new Il2CppInterop
            //new Il2CppSystem.IO.TextWriter().;


            Extractor.Logger.LogInfo($"Extracted ROOT of type {itemType.Name}. (" + count + ")");
        }
        catch (Exception ex)
        {
            Extractor.Logger.LogError($"Exception extract ROOT (" + itemType.FullName + "): " + ex.Message + " -> " + ex.StackTrace);
        }

    }

}
