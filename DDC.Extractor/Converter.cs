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
using static MS.Internal.Xml.XPath.QueryBuilder;

namespace DDC.Extractor;

internal static class Converter
{

    public static string Serialize(object o)
    {
        StringBuilder sb = new();
        var type = o.GetType();
        foreach (var prop in type.GetProperties())
        {
            if (prop.PropertyType.IsEnum)
            {

            }
            else
            if (prop.PropertyType.IsPrimitive)
            {

            }
            else
            if (prop.PropertyType == typeof(System.String))
            {

            }
            else
            if (prop.PropertyType.IsGenericType)
            {

            }
            Serialize(prop.GetValue(o));
        }
        return sb.ToString();
    }

    public static object? ConvertType(object original, string count = "")
    {
        try
        {
            // Convert to implemented type
            if (original is Il2CppObjectBase b)
            {
                original = SubtypeCasting.Converts(original, b);
                //if(original is EffectInstanceDice d)
                //{
                //    Extractor.Logger.LogWarning("ei dicenum: " + original.GetType() + " : " + d.diceNum);
                //}
            }

            var type1 = original.GetType();
            if (type1.FullName.EndsWith("Regex"))
                return ((Il2CppSystem.Text.RegularExpressions.Regex) original).ToString();

            Type type2 = GetCorrespondingType(type1);

            if (ExtractRoots.debug && !string.IsNullOrEmpty(count))
                Extractor.Logger.LogInfo($"ConvertingType: {type1.FullName} to {type2.FullName}. " + count);

            if (type1 == type2)
                return original;

            var inst = Activator.CreateInstance(type2);
            foreach (var prop in inst.GetType().GetProperties())
            {
                try
                {
                    if (ShouldSkipProperty(inst, original, prop))
                        continue;
                    var oProp = original.GetType().GetProperty(prop.Name, BindingFlags.Instance | BindingFlags.Public); //, BindingFlags.Instance);
                    if (oProp == null)
                    {
                        if (ExtractRoots.debug)
                            Extractor.Logger.LogWarning($"Converting property original (" + original + "), oProp is null (" + prop.Name + ": " + prop.PropertyType.Name + ").");
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

    public static object? ConvertProperty(object inst, PropertyInfo prop)
    {
        try
        {
            if (ExtractRoots.debug) // || inst.GetType() == typeof(SpellZoneDescr) || inst.GetType() == typeof(Spells))
                Extractor.Logger.LogInfo($"Converting property  " + inst + "." + prop.Name + ": " + prop.PropertyType.FullName);

            if (prop.PropertyType.IsEnum)
            {
                try
                {
                    return Convert.ToInt32(prop.GetValue(inst));
                }
                catch (Exception ex)
                {
                    Extractor.Logger.LogError($"Exception Converting property enum (" + inst + "." + prop.Name + ": " + prop.PropertyType.FullName + "): " + ex.Message + " -> " + ex.StackTrace);
                    return null;
                }
            }
            else
            if (prop.PropertyType.IsPrimitive || prop.PropertyType == typeof(System.String))
            {
                try
                {
                    return prop.GetValue(inst);
                }
                catch (Exception ex)
                {
                    Extractor.Logger.LogError($"Exception Converting property primitive or string (" + inst + "." + prop.Name + ": " + prop.PropertyType.FullName + "): " + ex.Message + " -> " + ex.StackTrace);
                    return null;
                }
            }
            else
            if (prop.PropertyType.IsGenericType)
            {
                return ConvertListProperty(inst, prop);
            }
            else
            {
                var val = prop.GetValue(inst);
                if (val == null) return null;
                var val2 = ConvertType(val);
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

    public static object ConvertListProperty(object inst, PropertyInfo prop)
    {
        var val = prop.GetValue(inst);
        if (val == null)
        {
            if (ExtractRoots.debug)
                Extractor.Logger.LogWarning($"Converting property list value is null in " + inst + "." + prop.Name + ": " + prop.PropertyType);
            return null;
        }
        var newGenericType = GetCorrespondingType(prop.PropertyType);
        if (newGenericType == prop.PropertyType)
        {
            Extractor.Logger.LogInfo($"Converting property list type didn't change");
            return val;
        }
        if (prop.PropertyType.GenericTypeArguments.Length == 1)
        {
            IEnumerable list1 = null;
            //Extractor.Logger.LogInfo("ConvertingProperty list json1: " + val.GetType() + " vs " + genericType);
            if (val is IEnumerable)
            {
                list1 = val as IEnumerable;
            }
            else if (val.GetType().Name.Contains("HashSet"))
            {
                // TODO HashSets unsupported for now. Only SpellScripts uses it and it's recursive anyway so we don't care.
                Extractor.Logger.LogError("HashSets not supported for now. " + prop.Name + ": " + val.GetType() + " vs " + newGenericType);
                return null;
            }
            else
            {
                list1 = val.GetType().GetProperty("_items")?.GetValue(val) as IEnumerable;
            }
            //IEnumerable list2;
            ICollection list2 = Activator.CreateInstance(newGenericType) as ICollection;
            //Extractor.Logger.LogInfo("ConvertingProperty list json2: " + val.ToString() + " to " + list2);
            if (list1 == null)
            {
                Extractor.Logger.LogError($"Converting property list error - val: " + val + ", list1: " + list1 + ", list2: " + list2);
                return val;
            }
            var meth = newGenericType.GetMethod("Add");
            //var json = JsonSerializer.Serialize(val, options: JsonSerializerOptions);

            List<string> strList = new();

            foreach (var i in list1)
            {
                var str = JsonSerializer.Serialize(i, ExtractorBehaviour.JsonSerializerOptions);
                strList.Add(str);

                var item = i;
                if (item is null) continue;
                //if (item is EffectInstance ei)
                //{
                //    var a = ei as Il2CppObjectBase;
                //    object dice = a.TryCast<EffectInstanceDice>();
                //    dice ??= a.TryCast<EffectInstanceMinMax>();
                //    dice ??= a.TryCast<EffectInstanceInteger>();
                //    if (dice != null) item = dice;
                //}
                var item2 = ConvertType(item);
                //if (item is EffectInstance ei)
                //{
                //    //var a = ei as Il2CppObjectBase;
                //    //object dice = a.TryCast<EffectInstanceDice>();
                //    //dice ??= a.TryCast<EffectInstanceMinMax>();
                //    //dice ??= a.TryCast<EffectInstanceInteger>();
                //    //if (dice != null) item = dice;
                //    Extractor.Logger.LogMessage("list item type: " + item.GetType() + " vs converted: " + item2.GetType());
                //}
                // faut pas que ce soit un root type, ceux là sont déjà sérializer on their own.
                // faut seulement les référencer par ID plutôt que par object reference, sinon on a une sérialization en boucle infinie
                if (item2 != null && !ExtractRoots.rootTypes.Contains(item2.GetType()))
                    meth.Invoke(list2, [item2]);
                //list2.Add(item2);
            }

            string json = $"[{string.Join(", ", strList)}]";

            return list2;
        }
        else
        if (prop.PropertyType.GenericTypeArguments.Length == 2)
        {
            Extractor.Logger.LogError("Error: unimplemented dictionary: " + inst.GetType().FullName + " -> " + prop.Name + ": " + newGenericType);
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
        return null;
    }

    public static Type GetCorrespondingType(Type type1)
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

            //Type type2 = Type.GetType("Generated." + type1.FullName + ", DDC");
            Type type2 = Type.GetType("GenPolymorphic." + type1.FullName + ", DDC");

            if (type2 == null) return type1;
            return type2;
        }
        catch (Exception ex)
        {
            Extractor.Logger.LogWarning("Exception GetCorrespondingType: " + ex.Message);
            return type1;
        }
    }

    public static bool ShouldSkipProperty(object inst, object original, PropertyInfo prop)
    {
        if (inst == null)
        {
            Extractor.Logger.LogError($"Converting property prop (" + original + "." + prop.Name + "), inst is null.");
            return true;
        }
        if (ExtractRoots.rootTypes.Contains(prop.PropertyType))
        {
            if (ExtractRoots.debug)
                Extractor.Logger.LogWarning($"Skip property " + prop.Name + ": " + prop.GetType().FullName);
            return true;
        }
        if (prop.PropertyType.Name == "SpellScripts")
        {
            if (ExtractRoots.debug)
                Extractor.Logger.LogWarning($"Skip weird, SpellScripts should be in rootTypes: " + prop.PropertyType.FullName);
            return true;
        }
        if (prop.PropertyType == typeof(Il2CppSystem.Object))
        {
            if (ExtractRoots.debug)
                Extractor.Logger.LogWarning($"Skip property: IL2.Object");
            return true;
        }

        // Skip properties that dont have a corresponding field
        Type baseType = original.GetType();
        bool foundField = false;
        while (baseType != null)
        {
            var staticField = baseType.GetField("NativeFieldInfoPtr_" + prop.Name, BindingFlags.Static | BindingFlags.NonPublic);
            if (staticField != null)
                foundField = true;
            baseType = baseType.BaseType;
        }
        if (!foundField)
        {
            if (ExtractRoots.debug)
                Extractor.Logger.LogWarning($"Skip property: no static field");
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
        if (prop.PropertyType.Name.Contains("MemoizedValues") || prop.Name.ToLower().Contains("memoized"))
        {
            if (ExtractRoots.debug)
                Extractor.Logger.LogWarning($"Skip property: memoized value");
            return true;
        }
        return false;
    }


}
