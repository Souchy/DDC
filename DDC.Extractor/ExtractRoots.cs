﻿using Core.DataCenter;
using Core.DataCenter.Metadata.Effect;
using Core.DataCenter.Metadata.Item;
using Core.DataCenter.Metadata.Monster;
using Core.DataCenter.Metadata.Spell;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DDC.Extractor;
internal class ExtractRoots
{
    public static List<Type> rootTypes = [typeof(Monsters), typeof(Items), typeof(Spells), typeof(SpellLevels), typeof(ItemTypes), typeof(ItemSuperTypes), typeof(Effects)];

    public static async Task GetAllRoots()
    {
        try
        {
            var roots = typeof(DataCenterModule).GetProperties().Where(p => p.Name.EndsWith("Root"));
            //.Select(rootProp =>
            //{
            //    string r = "";
            //    try
            //    {
            //        r = rootProp?.Name;
            //        var type = rootProp.PropertyType;
            //        r += ": " + type?.ToString();
            //        if (type.GenericTypeArguments.Length > 0)
            //        {
            //            return type.ToString() + "<" + string.Join(", ", type.GenericTypeArguments.Select(a => a.Name)) + ">";
            //        }
            //        if (type.BaseType == null)
            //            return type.ToString();
            //        var basetype = type.BaseType;
            //        if (basetype.GenericTypeArguments.Length > 0)
            //        {
            //            return basetype.ToString() + "<" + string.Join(", ", basetype.GenericTypeArguments.Select(a => a.Name)) + ">";
            //        }
            //        return type.ToString();
            //    }
            //    catch (Exception ex)
            //    {
            //        Extractor.Logger.LogInfo("Exception GetAllRoots iter (" + r + "): " + ex.Message + " -> " + ex.StackTrace);
            //        return "";
            //    }
            //}).Where(s => s != "");
            //Extractor.Logger.LogInfo("Roots: " + string.Join(", ", roots));

            foreach (var root in roots)
            {
                await ExtractRoot2(root);
            }
            Extractor.Logger.LogInfo($"Extracting ROOTs DONE =================");
        }
        catch (Exception ex)
        {
            Extractor.Logger.LogInfo("Exception GetAllRoots: " + ex.Message + " -> " + ex.StackTrace);
        }
    }
    public static async Task ExtractRoot2(object root0 = null)
    {
        try
        {
            var objects = root0.GetType().GetMethod("GetObjects").Invoke(root0, []);
            var items = objects.GetType().GetProperty("_items")?.GetValue(objects) as ICollection;
            var type = items.GetType().GenericTypeArguments[0];
            Extractor.Logger.LogInfo($"Extracting ROOT of type {type.Name}. " + items.Count);
           
            var genericType = GetCorrespondingType(items.GetType());
            var items2 = Activator.CreateInstance(genericType) as ICollection;
            var meth = genericType.GetMethod("Add");
            foreach (var item in items)
            {
                if (item is null) continue;
                var item2 = ConvertType(item.GetType(), item);
                if (item2 != null)
                    meth.Invoke(items2, [item2]);
            }
            var folder = type.Namespace.Replace(".", "/") + "/";
            string path = Path.Join(Extractor.OutputDirectory, "ddc/json/" + folder);
            System.IO.Directory.CreateDirectory(path);
            Extractor.Logger.LogInfo($"Created Directory. " + path);
            await using FileStream stream = File.OpenWrite(path + "/" + type.Name + ".json");
            await JsonSerializer.SerializeAsync(stream, items2, ExtractorBehaviour.JsonSerializerOptions);
            stream.Flush();
            Extractor.Logger.LogInfo($"Extracted ROOT of type {type.Name} to {path}.");
        }
        catch (Exception ex)
        {
            Extractor.Logger.LogError($"Exception extract ROOT (" + root0.ToString() + "): " + ex.Message + " -> " + ex.StackTrace);
        }

    }
    public static async Task ExtractRoot<T>(MetadataRoot<T> root0 = null)
    //static void ExtractRoot<T>(MetadataRoot<T> root0 = null)
    {
        try
        {
            string name = typeof(T).Name;
            MetadataRoot<T> root = root0; // DataCenterModule.GetDataRoot<MetadataRoot<T>>()
            Extractor.Logger.LogInfo($"Extracting ROOT of type {name}. " + root.GetObjects().Count);
            var items = root.GetObjects()._items.Where(i => i != null).Take(5).ToList();
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
            Extractor.Logger.LogInfo($"Extracted ROOT of type {name} to {path}.");
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
            if (type1.FullName == "Il2CppSystem.Text.RegularExpressions.Regex")
                return original.ToString();

            Type type2 = GetCorrespondingType(type1);
            //Extractor.Logger.LogInfo($"Converting type: {type2.FullName}. " + count);
            if (type1 == type2)
                return original;
            //if (rootTypes.Contains(type2))
            //{
            //    return null;
            //}
            var inst = Activator.CreateInstance(type2);
            foreach (var prop in inst.GetType().GetProperties())
            {
                if (prop.Name.StartsWith("m_") && inst.GetType().GetProperty(prop.Name[2..]) != null)
                {
                    continue;
                }
                var oProp = original.GetType().GetProperty(prop.Name);
                if (oProp == null)
                {
                    //Extractor.Logger.LogInfo($"Converting property inst (" + inst + "), prop is null.");
                    continue;
                }
                var val = ConvertProperty(original, oProp);
                prop.SetValue(inst, val);
            }
            //Extractor.Logger.LogInfo($"Converted type.");
            return inst;
        }
        catch (Exception ex)
        {
            Extractor.Logger.LogWarning($"Exception Converting type: " + ex.Message + " -> " + ex.StackTrace);
            return null;
        }
    }
    static object? ConvertProperty(object inst, PropertyInfo prop)
    {
        try
        {
            //if (prop == null)
            //{
            //    Extractor.Logger.LogInfo($"Converting property inst (" + inst + "), prop is null.");
            //    return null;
            //}
            if (rootTypes.Contains(prop.PropertyType))
            {
                return null;
            }
            if (inst == null)
            {
                Extractor.Logger.LogError($"Converting property prop (" + prop + "), inst is null.");
                return null;
            }
            //Extractor.Logger.LogInfo($"Converting property {prop.Name}.");
            if (prop.PropertyType == typeof(Il2CppSystem.Object))
            {
                return null;
                //Extractor.Logger.LogInfo($"Converting property is Object: " + inst + ". " + prop.Name + " = " + prop.GetValue(inst)?.GetType().ToString());
                //var val = prop.GetValue(inst);
                //if (val == null) return val;
                //var val2 = ConvertType(val.GetType(), val);
                //return val2;
            }
            if (prop.PropertyType.IsPrimitive)
            {
                return prop.GetValue(inst);
            }
            else
            if (prop.PropertyType == typeof(String))
            {
                return prop.GetValue(inst);
            }
            else
            if (prop.PropertyType.Name == "MemoizedValues")
            {
                return null;
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
                    IEnumerable list1;
                    if (val is IEnumerable)
                    {
                        list1 = val as IEnumerable;
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
            if (type1.FullName == "Il2CppSystem.Text.RegularExpressions.Regex") return typeof(string);

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
