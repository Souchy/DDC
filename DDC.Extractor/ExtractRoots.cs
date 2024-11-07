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
    public static List<Type> rootTypes = [
        typeof(Idols), typeof(IdolsPresetIcons), typeof(SocialTagsTypes), typeof(SkinPositions),
    ];
    public const int FAST_TAKES = 5;
    public const bool debug = false;

    public static async Task ExtractAll()
    {
        // Roots are properties of DataCenterModule
        var roots = typeof(DataCenterModule).GetProperties().Where(p => p.Name.EndsWith("Root")) // && !p.Name.StartsWith("s_"));
        .Select(prop =>
        {
            try
            {
                if (dangerousTypes.Select(t => t.Name.ToLower() + "root").Contains(prop.Name.ToLower()))
                {
                    Extractor.Logger.LogMessage($"Ignoring root: " + prop.Name);
                    return null;
                }
                return prop.GetValue(typeof(DataCenterModule));
            }
            catch (Exception ex)
            {
                Extractor.Logger.LogWarning("Exception asdf (" + prop.Name + "): " + ex.Message);
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
            // Wait async tasks
            var tasks = roots.Select(ExtractRootStep1).ToArray();
            Task.WaitAll(tasks);
            Extractor.Logger.LogInfo($"Extracting ROOTs DONE =================");
        }
        catch (Exception ex)
        {
            Extractor.Logger.LogInfo("Exception asdf overall: " + ex.Message + " -> " + ex.StackTrace);
        }
    }

    private static async Task ExtractRootStep1(object v)
    {
        try
        {
            //Extractor.Logger.LogInfo($"Datacenter prop: " + v.GetType().Name);
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
            await ExtractRootStep2(items, items2, itemType, methAdd);
        }
        catch (Exception ex)
        {
            Extractor.Logger.LogWarning("Exception asdf prop (" + v.GetType().Name + "): " + ex.Message);
            //continue;
        }
    }

    private static async Task ExtractRootStep2(IEnumerable items, ICollection items2, Type itemType, MethodInfo methAdd)
    {
        if (dangerousTypes.Contains(itemType))
            return;
        try
        {
            var folder = itemType.Namespace.Replace(".", "/") + "/";
            string path = Path.Join(Extractor.OutputDirectory, folder);
            System.IO.Directory.CreateDirectory(path);
            Extractor.Logger.LogInfo($"Extracting ROOT of type {itemType.Name} to {path}.");

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

            await using FileStream stream = File.OpenWrite(path + "/" + itemType.Name + ".json");
            await JsonSerializer.SerializeAsync<object>(stream, items2, ExtractorBehaviour.JsonSerializerOptions);
            stream.Flush();

            Extractor.Logger.LogInfo($"Extracted ROOT of type {itemType.Name}. (" + count + ")");
        }
        catch (Exception ex)
        {
            Extractor.Logger.LogError($"Exception extract ROOT (" + itemType.FullName + "): " + ex.Message + " -> " + ex.StackTrace);
        }

    }

}
