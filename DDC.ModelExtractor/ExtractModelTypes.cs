using Core.DataCenter.Metadata.Item;
using System.Reflection;
using System.Text;

namespace DDC.ModelExtractor;

public class ExtractModelTypes
{
    public const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;

    public static async Task GetAllModels()
    {
        var datacenterAssembly = typeof(Items).Assembly;
        ModelExtractor.Logger.LogInfo("====== ASSEMBLIES ======");
        await extractAssembly(datacenterAssembly);
        ModelExtractor.Logger.LogInfo("====== DONE WRITE CSHARP ======");
    }
    static async Task extractAssembly(Assembly ass)
    {
        ModelExtractor.Logger.LogInfo(ass.FullName);
        ModelExtractor.Logger.LogInfo("====== TYPES ======");
        var types = ass.GetTypes();
        //Extractor.Logger.LogInfo(string.Join(", ", types.Where(t => t.FullName.ToLower().StartsWith("Core.DataCenter.Metadata".ToLower())).Select(t => t.FullName)));
        foreach (var t in types)
        {
            if (t.IsGenericType)
                continue;
            if (!(
                //t.FullName.ToLower().StartsWith("Core.DataCenter".ToLower()) ||
                //t.FullName.ToLower().StartsWith("Metadata.".ToLower())
                t.FullName.ToLower().StartsWith("Core.DataCenter.Metadata".ToLower()) ||
                t.FullName.ToLower().StartsWith("Core.DataCenter.Types".ToLower()) ||
                t.FullName.ToLower().StartsWith("Core.DataCenter.Interfaces".ToLower()) ||
                t.FullName.ToLower().StartsWith("Metadata.Enums.".ToLower()) ||
                t.FullName.ToLower().StartsWith("Metadata.Appearance.".ToLower())
                ))
                continue;
            if (t.FullName.EndsWith("Root") || t.Name.StartsWith("__")) // || t.Namespace.EndsWith(".Sound"))
                continue;
            //ModelExtractor.Logger.LogInfo(t.FullName);

            await WriteCSharp(t);
            //await WriteProto(t);
        }
    }

    //static async Task WriteProto(Type type)
    //{
    //    string path = Path.Join(Extractor.OutputDirectory, "ddc/protos/" + type.Name + ".proto");
    //    StringBuilder sb = new StringBuilder();
    //    sb.AppendLine($"message {type.Name} " + "{");
    //    int i = 1;
    //    foreach (var prop in type.GetProperties())
    //    {
    //        sb.Append('\t');

    //        if (prop.PropertyType.Name.EndsWith("Ptr") || prop.Name == "WasCollected")
    //            sb.Append("// ");

    //        sb.Append("optional ");

    //        var propType = prop.PropertyType.Name;
    //        if (prop.PropertyType.IsGenericType)
    //        {
    //            sb.Append("repeated ");
    //            propType = prop.PropertyType.GenericTypeArguments[0].Name;
    //        }
    //        if (prop.PropertyType.IsPrimitive)
    //        {
    //            propType = propType.ToLower();
    //        }

    //        sb.Append(propType);
    //        sb.Append(' ');
    //        sb.Append(prop.Name);

    //        sb.AppendLine(" = " + i + ";");
    //        i++;
    //    }
    //    sb.Append('}');
    //    await File.WriteAllTextAsync(path, sb.ToString());
    //}

    static async Task WriteCSharp(Type type)
    {
        var folderName = type.Namespace.Replace(".", "/");
        var folderPath = Path.Combine(ModelExtractor.OutputDirectory, folderName);
        Directory.CreateDirectory(folderPath);
        string filePath = $"{folderPath}/{type.Name}.cs";
        ModelExtractor.Logger.LogInfo(filePath);

        var str = "";
        try
        {
            if (type.IsEnum)
            {
                str = typeToEnumString(type);
            }
            else
            if (type.IsValueType)
            {
                str = typeToStructString(type);
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
                await File.WriteAllTextAsync(filePath, str);
            }
        }
        catch (Exception ex)
        {
            ModelExtractor.Logger.LogError("Exception WriteCSharp: " + ex.Message + " -> " + ex.StackTrace);
        }
    }

    static string typeToEnumString(Type type)
    {
        try
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("namespace Generated." + type.Namespace + ";");
            sb.AppendLine($"public enum {type.Name} " + "{");
            var names = type.GetEnumNames();
            var values = type.GetEnumValues();
            for (int i = 0; i < names.Length; i++)
            {
                sb.Append('\t');
                sb.Append(names[i]);
                sb.Append(" = ");
                sb.Append(Convert.ToInt32(values.GetValue(i)));
                sb.AppendLine(",");
            }
            sb.AppendLine("}");
            return sb.ToString();
        }
        catch (Exception ex)
        {
            ModelExtractor.Logger.LogError("Exception typeToEnumString (" + type.FullName + "): " + ex.Message + " -> " + ex.StackTrace);
            return null;
        }
    }
    static string typeToStructString(Type type)
    {
        try
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine("namespace Generated." + type.Namespace + ";");
            sb.Append($"public struct {type.Name} ");
            sb.Append('{');
            sb.AppendLine();
            int i = 1;
            foreach (var field in type.GetFields(bindingFlags))
            {

                if (field.FieldType.Name.EndsWith("Ptr") || field.Name == "WasCollected")
                    continue;
                if (field.DeclaringType != type)
                    continue;
                if (field.Name.Contains(".")) //"_002")) // should be fixed by isVirtual
                    continue;
                var fieldType = ConvertTypeName(field.FieldType);
                sb.Append('\t');
                sb.Append("public ");
                sb.Append(fieldType);
                sb.Append(' ');
                sb.Append(field.Name);
                sb.Append(';');
                sb.Append('\n');
                i++;
            }
            sb.AppendLine("}");
            return sb.ToString();
        }
        catch (Exception ex)
        {
            ModelExtractor.Logger.LogError("Exception typeToStructString (" + type.FullName + "): " + ex.Message + " -> " + ex.StackTrace);
            return null;
        }
    }
    static string typeToClassString(Type type)
    {
        try
        {

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using UnityEngine;");

            //if (type.FullName == "Core.DataCenter.Metadata.Appearance.SkinSlotsRules")
            //    sb.AppendLine("using Metadata.Appearance;");
            if (type.FullName == "Core.DataCenter.Metadata.Sound.SoundBones")
                sb.AppendLine("using static Core.DataCenter.Metadata.Sound.SoundBones;");
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
            foreach (var prop in type.GetProperties(bindingFlags)) //BindingFlags.Instance))
            {
                if (shouldSkipProp(type, prop))
                    continue;
                if (prop.PropertyType.Name.EndsWith("Ptr") || prop.Name == "WasCollected") // || prop.Name.StartsWith("m_"))
                    continue;
                if (prop.DeclaringType != type)
                    continue;
                if (prop.Name.Contains(".")) //"_002")) // should be fixed by isVirtual
                    continue;
                var propType = ConvertTypeName(prop.PropertyType);
                sb.Append('\t');
                sb.Append("public ");
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
            ModelExtractor.Logger.LogError("Exception typeToClassString: " + ex.Message + " -> " + ex.StackTrace);
            return null;
        }
    }
    static bool shouldSkipProp(Type original, PropertyInfo prop)
    {
        // Skip properties that dont have a corresponding field
        Type baseType = original;
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
            return true;
        }
        return false;
    }

    static string ConvertTypeName(Type type)
    {
        var propType = type.FullName;
        if (propType.StartsWith("Core.DataCenter.Metadata")) propType = "Generated." + propType;
        else if (propType.StartsWith("Core.DataCenter.Types")) propType = "Generated." + propType;
        else if (propType.StartsWith("Core.DataCenter.Interfaces")) propType = "Generated." + propType;
        else if (propType.StartsWith("Core.")) propType = type.Name;
        if (propType.StartsWith("Metadata.Enums")) propType = "Generated." + propType;
        if (propType.StartsWith("Metadata.Appearance")) propType = "Generated." + propType;
        else if (propType.StartsWith("Metadata.")) propType = type.Name;

        if (propType.EndsWith("Regex"))
            return typeof(string).FullName;
        propType = propType.Replace("Il2CppSystem.Object", "object");
        propType = propType.Replace("Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStringArray", typeof(string).FullName + "[]");

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
        return propType;
    }

}
