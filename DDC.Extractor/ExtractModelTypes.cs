using Core.DataCenter.Metadata.Item;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DDC.Extractor;
public class ExtractModelTypes
{
    public static async Task GetAllModels()
    {
        //System.IO.Directory.Delete("C:\\Robyn\\Git\\ankama\\BPI\\DDC\\DDC\\Generated");
        //System.IO.Directory.Delete(Path.Join(Extractor.OutputDirectory, "ddc/cs/"));
        //System.IO.Directory.CreateDirectory(Path.Join(Extractor.OutputDirectory, "ddc/cs/"));

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var theAss = typeof(Items).Assembly;
        Extractor.Logger.LogInfo("====== ASSEMBLIES ======");
        await extractAssembly(theAss);
        //foreach (var ass in assemblies)
        //{
        //    if (ass.FullName == null || !ass.FullName.Contains("Ankama.Dofus.Core.DataCenter"))
        //        continue;
        //    await extractAssembly(ass);
        //}
        Extractor.Logger.LogInfo("====== DONE WRITE CSHARP ======");
    }
    static async Task extractAssembly(Assembly ass)
    {
        Extractor.Logger.LogInfo(ass.FullName);
        Extractor.Logger.LogInfo("====== TYPES ======");
        var types = ass.GetTypes();
        //Extractor.Logger.LogInfo(string.Join(", ", types.Where(t => t.FullName.ToLower().StartsWith("Core.DataCenter.Metadata".ToLower())).Select(t => t.FullName)));
        foreach (var t in types)
        {
            if (t.IsGenericType)
                continue;
            if (!(
                t.FullName.ToLower().StartsWith("Core.DataCenter.Metadata".ToLower()) ||
                t.FullName.ToLower().StartsWith("Core.DataCenter.Types".ToLower()) ||
                t.FullName.ToLower().StartsWith("Metadata.Enums.".ToLower())
                ))
                continue;
            if (t.FullName.EndsWith("Root") || t.Name.StartsWith("__")) // || t.Namespace.EndsWith(".Sound"))
                continue;
            Extractor.Logger.LogInfo(t.FullName);

            await WriteCSharp(t);
            //await WriteProto(t);
        }
    }
    static async Task WriteProto(Type type)
    {
        string path = Path.Join(Extractor.OutputDirectory, "ddc/protos/" + type.Name + ".proto");
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"message {type.Name} " + "{");
        int i = 1;
        foreach (var prop in type.GetProperties())
        {
            sb.Append('\t');

            if (prop.PropertyType.Name.EndsWith("Ptr") || prop.Name == "WasCollected")
                sb.Append("// ");

            sb.Append("optional ");

            var propType = prop.PropertyType.Name;
            if (prop.PropertyType.IsGenericType)
            {
                sb.Append("repeated ");
                propType = prop.PropertyType.GenericTypeArguments[0].Name;
            }
            if (prop.PropertyType.IsPrimitive)
            {
                propType = propType.ToLower();
            }

            sb.Append(propType);
            sb.Append(' ');
            sb.Append(prop.Name);

            sb.AppendLine(" = " + i + ";");
            i++;
        }
        sb.Append('}');
        await File.WriteAllTextAsync(path, sb.ToString());
    }
    static async Task WriteCSharp(Type type)
    {
        var folder = type.Namespace.Replace(".", "/") + "/";
        System.IO.Directory.CreateDirectory("C:\\Robyn\\Git\\ankama\\BPI\\DDC\\DDC\\Generated\\" + folder);
        //System.IO.Directory.CreateDirectory(Path.Join(Extractor.OutputDirectory, "ddc/cs/" + folder));

        //string path = Path.Join(Extractor.OutputDirectory, "ddc/cs/" + folder + type.Name + ".cs");
        string path2 = $"C:\\Robyn\\Git\\ankama\\BPI\\DDC\\DDC\\Generated\\{folder + type.Name}.cs";
        //Extractor.Logger.LogInfo("folder: " + folder);

        var str = "";
        try
        {
            if (type.IsEnum)
            {
                str = typeToEnumString(type);
            }
            else
            if (type.IsClass)
            {
                str = typeToClassString(type);
            }
            else
            {
                return;
            }
            if (!string.IsNullOrWhiteSpace(str))
            {
                //await File.WriteAllTextAsync(path, str);
                await File.WriteAllTextAsync(path2, str);
            }
        }
        catch (Exception ex)
        {
            Extractor.Logger.LogError("Exception WriteCSharp: " + ex.Message + " -> " + ex.StackTrace);
        }
    }
    static string typeToEnumString(Type type)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("namespace Generated." + type.Namespace + ";");
        sb.AppendLine($"public enum {type.Name} " + "{");
        var values = type.GetEnumValues();
        var names = type.GetEnumNames();

        for (int i = 0; i < names.Length; i++)
        {
            sb.Append('\t');
            if (values.Length > 0)
                sb.Append(values.GetValue(i));
            else
                sb.Append(names[i]);
            sb.AppendLine(",");
        }
        sb.AppendLine("}");
        return sb.ToString();
    }
    static string typeToClassString(Type type)
    {
        try
        {

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using Core.DataCenter.Interfaces;");
            sb.AppendLine("using Core.DataCenter.Metadata;");
            sb.AppendLine("using Core.DataCenter.Types;");
            sb.AppendLine("using Metadata;");
            sb.AppendLine("using Metadata.Enums;");
            sb.AppendLine("using UnityEngine;");
            //if(type.FullName == "SkinSlotsRules")
            //{
            //    sb.AppendLine("using Metadata.Appearance;");
            //}
            if (type.FullName == "Core.DataCenter.Metadata.Sound.SoundBones")
                sb.AppendLine("using static Core.DataCenter.Metadata.Sound.SoundBones;");
            if (type.FullName == "Core.DataCenter.Metadata.Appearance.SkinSlotsRules")
                sb.AppendLine("using Metadata.Appearance;");
            if (type.Name == "SoundBonesDictionary")
            {
                return null;
            }

            sb.AppendLine();
            sb.AppendLine("namespace Generated." + type.Namespace + ";");

            sb.Append($"public class {type.Name} ");
            if (type.BaseType != null && (type.BaseType.FullName.StartsWith("Core.DataCenter") || type.BaseType.FullName.StartsWith("Metadata")))
            {
                sb.Append(": ");
                sb.AppendLine(ConvertTypeName(type.BaseType));
            }
            sb.Append('{');
            sb.AppendLine();
            int i = 1;
            foreach (var prop in type.GetProperties())
            {

                if (prop.PropertyType.Name.EndsWith("Ptr") || prop.Name == "WasCollected") // || prop.Name.StartsWith("m_"))
                    continue;
                //if (prop.GetGetMethod()?.IsVirtual ?? false)
                //{
                //    if (!prop.Name.Contains("_002"))
                //    {
                //        Extractor.Logger.LogWarning("Skipping virtual property: " + type.Name + "." + prop.Name + ": " + prop.PropertyType);
                //    }
                //    continue;
                //}
                if (prop.Name.Contains(".")) //"_002")) // should be fixed by isVirtual
                    continue;

                sb.Append('\t');
                sb.Append("public ");

                var propType = ConvertTypeName(prop.PropertyType);

                sb.Append(propType);
                sb.Append(' ');
                sb.Append(prop.Name);
                sb.Append(" { get; init; }");
                sb.Append('\n');
                i++;
            }
            sb.AppendLine("}");
            return sb.ToString();
        }
        catch (Exception ex)
        {
            Extractor.Logger.LogError("Exception typeToClassString: " + ex.Message + " -> " + ex.StackTrace);
            return "";
        }
    }
    static string ConvertTypeName(Type type)
    {
        var propType = type.FullName;
        if (propType.StartsWith("Core.DataCenter.Metadata")) propType = "Generated." + propType;
        else if (propType.StartsWith("Core.DataCenter.Types")) propType = "Generated." + propType;
        else if (propType.StartsWith("Core.")) propType = type.Name;
        if (propType.StartsWith("Metadata.Enums")) propType = "Generated." + propType;
        else if (propType.StartsWith("Metadata.")) propType = type.Name;

        if(propType.EndsWith("Regex"))
            return typeof(string).FullName;
        propType = propType.Replace("Il2CppSystem.Object", "object");
        propType = propType.Replace("Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStringArray", typeof(string).FullName + "[]");
        //propType = propType.Replace("Il2CppSystem.Text.RegularExpressions.Regex", typeof(string).Name); //System.Text.RegularExpressions.Regex");

        if (propType.Contains("+"))
        {
            propType = type.Name;
        }
        if (type.IsGenericType && propType.Contains("`"))
        {
            propType = type.Name;
            propType = propType.Substring(0, propType.IndexOf("`"));
            propType = propType.Replace("Il2CppStructArray", "List");
            propType += "<";
            propType += string.Join(", ", type.GenericTypeArguments.Select(ConvertTypeName));
            propType += ">";
        }
        //if (prop.PropertyType.IsPrimitive)
        //{
        //    propType = propType.ToLower().Replace("boolean", "bool").Replace("32", "");
        //}
        return propType;
    }

}
