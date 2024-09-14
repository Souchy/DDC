using Core.DataCenter;
using Core.DataCenter.Metadata.Alliance;
using Core.DataCenter.Metadata.Appearance;
using Core.DataCenter.Metadata.Breed;
using Core.DataCenter.Metadata.Challenge;
using Core.DataCenter.Metadata.Effect;
using Core.DataCenter.Metadata.Idol;
using Core.DataCenter.Metadata.Item;
using Core.DataCenter.Metadata.Monster;
using Core.DataCenter.Metadata.OptionalFeatures;
using Core.DataCenter.Metadata.Progression;
using Core.DataCenter.Metadata.Social;
using Core.DataCenter.Metadata.Spell;
using Core.DataCenter.Metadata.World;
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

namespace DDC.Extractor;
public class ExtractRoots
{
    public static List<Type> dangerousTypes = [typeof(Idols), typeof(IdolsPresetIcons), typeof(SocialTagsTypes), typeof(SkinPositions)];
    public static List<string> dangerousProperties = ["name", "undiacriticalName", "unDiacriticalName", "entityName", "durationString", "description", "theoreticalDescription", "descriptionForTooltip", "theoreticalDescriptionForTooltip", "theoreticalShortDescriptionForTooltip"];
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
                var genericType = GetCorrespondingType(items.GetType());
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
    public static async Task ExtractRoot2(IEnumerable items, ICollection items2, Type itemType, MethodInfo methAdd) //object root0 = null)
    {
        if (itemType == typeof(Idols))
            return;
        try
        {
            var folder = itemType.Namespace.Replace(".", "/") + "/";
            string path = Path.Join(Extractor.OutputDirectory, "ddc/json/" + folder);
            Extractor.Logger.LogInfo($"Extracting ROOT of type {itemType.Name} to {path}.");

            //var methAdd = items2.GetType().GetMethod("Add");
            int count = 0;
            foreach (var item in items)
            {
                if (item is null) continue;
                //if (count == FAST_TAKES) break;
                count++;
                var item2 = ConvertType(item.GetType(), item, count.ToString());
                if (item2 != null)
                    methAdd.Invoke(items2, [item2]);
            }
            System.IO.Directory.CreateDirectory(path);
            //Extractor.Logger.LogInfo($"Created Directory. " + path);
            await using FileStream stream = File.OpenWrite(path + "/" + itemType.Name + ".json");
            await JsonSerializer.SerializeAsync(stream, items2, ExtractorBehaviour.JsonSerializerOptions);
            stream.Flush();
            Extractor.Logger.LogInfo($"Extracted ROOT of type {itemType.Name}. (" + count + ")");
        }
        catch (Exception ex)
        {
            Extractor.Logger.LogError($"Exception extract ROOT (" + itemType.FullName + "): " + ex.Message + " -> " + ex.StackTrace);
        }

    }
    public static async Task ExtractRoot<T>(MetadataRoot<T> root0 = null)
    //static void ExtractRoot<T>(MetadataRoot<T> root0 = null)
    {
        if (typeof(T) == typeof(Idols))
            return;
        try
        {
            string name = typeof(T).Name;
            MetadataRoot<T> root = root0; // DataCenterModule.GetDataRoot<MetadataRoot<T>>()
            var items = root.GetObjects()._items.Where(i => i != null).ToList(); //.Take(FAST_TAKES).ToList();
            Extractor.Logger.LogInfo($"Extracting ROOT of type {name}. (" + items.Count + "/" + root.GetObjects().Count + ")");
            int count = 0;
            var items2 = items.Select(i =>
            {
                count++;
                return ConvertType(typeof(T), i, count.ToString());
            }).Where(n => n != null);

            Extractor.Logger.LogInfo($"Converted ROOT types. " + items2.Count());

            var folder = typeof(T).Namespace.Replace(".", "/") + "/";
            string path = Path.Join(Extractor.OutputDirectory, "ddc/json/" + folder);
            System.IO.Directory.CreateDirectory(path);
            Extractor.Logger.LogInfo($"Created Directory. " + path);
            await using FileStream stream = File.OpenWrite(path + "/" + name + ".json");
            await JsonSerializer.SerializeAsync(stream, items2, ExtractorBehaviour.JsonSerializerOptions);
            stream.Flush();
            //var json = JsonSerializer.Serialize(items2, JsonSerializerOptions);
            //Extractor.Logger.LogMessage(json);
            //File.WriteAllText(path + "/" + name + ".json", json);
            //await File.WriteAllTextAsync(path + "/" + name + ".json", json);
            Extractor.Logger.LogInfo($"Extracted ROOT of type {name} to {path}. (" + count + ")");
        }
        catch (Exception ex)
        {
            Extractor.Logger.LogError($"Exception extract ROOT (" + root0.ToString() + "): " + ex.Message + " -> " + ex.StackTrace);
        }
    }
    static object? ConvertType(Type type1, object original, string count = "")
    {
        try
        {
            if (type1.FullName.EndsWith("Regex"))
                return ((Il2CppSystem.Text.RegularExpressions.Regex) original).ToString();

            Type type2 = GetCorrespondingType(type1);

            if (debug)
                Extractor.Logger.LogInfo($"ConvertingType: {type2.FullName}. " + count);
            if (type1 == type2)
                return original;

            var inst = Activator.CreateInstance(type2);
            foreach (var prop in inst.GetType().GetProperties())
            {
                try
                {
                    if (ShouldSkipProperty(original, prop))
                        continue;
                    var oProp = original.GetType().GetProperty(prop.Name);
                    if (oProp == null)
                    {
                        //Extractor.Logger.LogInfo($"Converting property inst (" + inst + "), prop is null.");
                        continue;
                    }
                    var val = ConvertProperty(original, oProp);
                    prop.SetValue(inst, val);
                }
                catch (Exception ex)
                {
                    Extractor.Logger.LogWarning($"Exception ConvertingType loop: " + ex.Message + " -> " + ex.StackTrace);
                }
            }
            //Extractor.Logger.LogInfo($"Converted type.");
            return inst;
        }
        catch (Exception ex)
        {
            Extractor.Logger.LogWarning($"Exception ConvertingType: " + ex.Message + " -> " + ex.StackTrace);
            return null;
        }
    }

    static bool ShouldSkipProperty(object inst, PropertyInfo prop)
    {
        if (inst == null)
        {
            Extractor.Logger.LogError($"Converting property prop (" + prop + "), inst is null.");
            return true;
        }
        if (rootTypes.Contains(prop.PropertyType))
        {
            return true;
        }
        if (prop.PropertyType.Name == "SpellScripts")
        {
            //Extractor.Logger.LogError($"Skip weird, SpellScripts should be in rootTypes: " + prop.PropertyType.FullName);
            return true;
        }
        if (prop.PropertyType == typeof(Il2CppSystem.Object))
            return true;

        // Skip properties that dont have a corresponding field
        var staticField = inst.GetType().GetField("NativeFieldInfoPtr_" + prop.Name, BindingFlags.Static | BindingFlags.NonPublic);
        if (staticField == null)
        {
            //Extractor.Logger.LogWarning("SKIP DANGEROUS PROPERTY (" + inst + "." + prop.Name + ") that doesnt have a static field");
            //Extractor.Logger.LogMessage("fields: " + string.Join(", ", inst.GetType().GetFields(BindingFlags.Static | BindingFlags.NonPublic).Select(f => f.Name)));
            //Extractor.Logger.LogMessage("Special/Collectible: " + prop.IsSpecialName + ", " + prop.IsCollectible);
            //Extractor.Logger.LogMessage("GetMethod: " + prop.GetMethod + ", " + prop.GetGetMethod() + ", " + prop.GetMethod.Attributes + ", " + prop.GetMethod.IsPublic);
            //Extractor.Logger.LogMessage("SetMethod: " + prop.SetMethod + ", " + prop.GetSetMethod() + ", " + prop.SetMethod.Attributes + ", " + prop.SetMethod.IsPublic);
            //Extractor.Logger.LogMessage("Get attributes: " + string.Join(", ", prop.GetMethod.GetCustomAttributes().Select(a => a.ToString())));
            //Extractor.Logger.LogMessage("Set attributes: " + string.Join(", ", prop.SetMethod.GetCustomAttributes().Select(a => a.ToString())));
            return true;
        }

        //if (dangerousProperties.Contains(prop.Name))
        //    return true;
        //if (inst.GetType().GetProperty("m_" + prop.Name) != null)
        //{
        //    //Extractor.Logger.LogInfo("ConvertingType skip prop by _m: " + prop.Name);
        //    return true;
        //}
        //if (inst.GetType().GetProperty(prop.Name + "Id") != null || inst.GetType().GetProperty(prop.Name + "Ids") != null)
        //{
        //    //Extractor.Logger.LogInfo("ConvertingType skip prop by Id: " + prop.Name);
        //    return true;
        //}
        if (prop.PropertyType.Name == "MemoizedValues")
        {
            return true;
        }
        return false;
    }

    static object? ConvertProperty(object inst, PropertyInfo prop)
    {
        try
        {
            if (debug)
                Extractor.Logger.LogInfo($"Converting property  " + inst + "." + prop.Name + ": " + prop.PropertyType.FullName);

            if (prop.PropertyType.IsPrimitive)
            {
                try
                {
                    return prop.GetValue(inst);
                }
                catch (Exception ex)
                {
                    Extractor.Logger.LogError($"Exception Converting property primitive: " + ex.Message + " -> " + ex.StackTrace);
                    return null;
                }
            }
            else
            if (prop.PropertyType == typeof(System.String))
            {
                try
                {
                    return prop.GetValue(inst);
                }
                catch (Exception ex)
                {
                    Extractor.Logger.LogError($"Exception Converting property string: " + ex.Message + " -> " + ex.StackTrace);
                    return null;
                }
            }
            else
            if (prop.PropertyType.IsGenericType)
            {
                var val = prop.GetValue(inst);
                if (val == null)
                {
                    //Extractor.Logger.LogWarning($"Converting property list value is null");
                    return null;
                }
                var genericType = GetCorrespondingType(prop.PropertyType);
                if (genericType == prop.PropertyType)
                {
                    Extractor.Logger.LogInfo($"Converting property list type didn't change");
                    return val;
                }
                if (prop.PropertyType.GenericTypeArguments.Length == 1)
                {
                    //Extractor.Logger.LogInfo("ConvertingProperty list json: " + val.ToString() + " to " + list2);

                    IEnumerable list1 = null;
                    if (val is IEnumerable)
                    {
                        list1 = val as IEnumerable;
                    }
                    else if (val.GetType().Name.Contains("HashSet"))
                    {
                        // TODO HashSets unsupported for now. Only SpellScripts uses it and it's recursive anyway so we don't care.
                        return null;
                    }
                    else
                    {
                        list1 = val.GetType().GetProperty("_items")?.GetValue(val) as IEnumerable;
                    }
                    var list2 = Activator.CreateInstance(genericType) as ICollection;
                    if (list1 == null)
                    {
                        Extractor.Logger.LogError($"Converting property list error - val: " + val + ", list1: " + list1 + ", list2: " + list2);
                        return val;
                    }
                    var meth = genericType.GetMethod("Add");
                    //var json = JsonSerializer.Serialize(val, options: JsonSerializerOptions);

                    foreach (var item in list1)
                    {
                        if (item is null) continue;
                        var item2 = ConvertType(item.GetType(), item);
                        if (item2 != null && !rootTypes.Contains(item2.GetType()))
                            meth.Invoke(list2, [item2]);
                    }
                    return list2;
                }
                else
                if (prop.PropertyType.GenericTypeArguments.Length == 2)
                {
                    Extractor.Logger.LogWarning("Error: unimplemented dictionary: " + inst.GetType().FullName + " -> " + prop.Name + ": " + genericType);
                    //var dic2 = Activator.CreateInstance(genericType) as IDictionary;
                    ////Dictionary<int, int> asd;
                    ////asd.Add(0, 0);
                    //var meth = genericType.GetMethod("Add");

                    //foreach (var item in val)
                    //{
                    //    var item2 = ConvertType(item.GetType(), item);
                    //    meth.Invoke(dic2, [item2]);
                    //}
                    return null;
                }
            }
            else
            {
                var val = prop.GetValue(inst);
                if (val == null) return null;
                var val2 = ConvertType(val.GetType(), val);
                if (val2 == null) return null;
                if (val2.GetType().FullName.StartsWith("Core.") || val2.GetType().FullName.StartsWith("Metadata."))
                {
                    //Extractor.Logger.LogWarning("ConvertingProperty ignore external type: " + inst + "." + prop.Name + ":" + val2.GetType().FullName);
                    return null;
                }
                return val2;
            }
        }
        catch (Exception ex)
        {
            Extractor.Logger.LogWarning("Exception ConvertingProperty (" + inst + ", " + prop + "): " + ex.Message + " -> " + ex.StackTrace);
        }
        return null;
    }
    static Type GetCorrespondingType(Type type1)
    {
        try
        {
            if (type1.IsPrimitive)
            {
                return type1;
            }
            if (type1.IsGenericType)
            {
                Type tbase = typeof(List<>);
                if (type1.GenericTypeArguments.Length == 2)
                {
                    tbase = typeof(Dictionary<,>);
                }
                var args = type1.GenericTypeArguments.Select(GetCorrespondingType).ToArray();
                var listType = tbase.MakeGenericType(args);
                return listType;
            }
            if (type1.FullName.EndsWith("Regex")) return typeof(string);
            //if (type1.FullName == "Il2CppSystem.Text.RegularExpressions.Regex") return typeof(string);

            Type type2 = Type.GetType("Generated." + type1.FullName + ", DDC");
            if (type2 == null) return type1;
            return type2;
        }
        catch (Exception ex)
        {
            Extractor.Logger.LogWarning("Exception GetCorrespondingType: " + ex.Message);
            return type1;
        }
    }
}
