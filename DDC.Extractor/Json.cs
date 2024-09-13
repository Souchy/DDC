using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.InteropServices;
using Il2CppSystem;
using Il2CppInterop.Runtime.InteropTypes;

namespace DDC.Extractor;
public class Json
{
    //static readonly JsonSerializerSettings jsonSerializerSettings = new()
    //{
    //    Formatting = Formatting.Indented,
    //    TypeNameHandling = TypeNameHandling.Auto,
    //    NullValueHandling = NullValueHandling.Ignore,
    //    ObjectCreationHandling = ObjectCreationHandling.Replace,
    //    MissingMemberHandling = MissingMemberHandling.Ignore,

    //    //Error = static (obj, args) => { },
    //    //ContractResolver = new IgnorePropertiesResolver(new[] { "ObjectClass", "Pointer" })
    //};
    //static Json()
    //{
    //    //jsonSerializerSettings.ContractResolver = (IContractResolver) (Il2CppObjectBase) (Il2CppSystem.Object) new IgnorePropertiesResolver([""]);
    //}

    //public static string serialize(Il2CppSystem.Object obj)
    //{
    //    return JsonConvert.SerializeObject(obj, jsonSerializerSettings);
    //}

    //public static T deserialize<T>(string json)
    //{
    //    return JsonConvert.DeserializeObject<T>(json, jsonSerializerSettings);
    //}

}

//public class IgnorePropertiesResolver : DefaultContractResolver
//{
//    private readonly HashSet<string> ignoreProps;
//    public IgnorePropertiesResolver(IEnumerable<string> propNamesToIgnore)
//    {
//        this.ignoreProps = new HashSet<string>(propNamesToIgnore);
//    }

//    public override JsonProperty CreateProperty(Il2CppSystem.Reflection.MemberInfo member, MemberSerialization memberSerialization)
//    {
//        JsonProperty property = base.CreateProperty(member, memberSerialization);
//        if (this.ignoreProps.Contains(property.PropertyName))
//        {
//            property.ShouldSerialize = (Il2CppSystem.Predicate<Il2CppSystem.Object>) (_ => false);
//        }
//        if (property.PropertyType.Name.EndsWith("Ptr"))
//        {
//            property.ShouldSerialize = (Il2CppSystem.Predicate<Il2CppSystem.Object>) (_ => false);
//        }
//        return property;
//    }

//    //protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
//    //{
//    //    JsonProperty property = base.CreateProperty(member, memberSerialization);
//    //    if (this.ignoreProps.Contains(property.PropertyName))
//    //    {
//    //        property.ShouldSerialize = _ => false;
//    //    }
//    //    if(property.PropertyType.Name.EndsWith("Ptr"))
//    //    {
//    //        property.ShouldSerialize = _ => false;
//    //    }
//    //    return property;
//    //}
//}
