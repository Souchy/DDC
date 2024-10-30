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

namespace DDC.ModelExtractor;

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
        ModelExtractorPlugin.Logger.LogInfo("Start extracting data...");

        yield return WaitForCompletion(ExtractModelTypes.GetAllModels());
        ModelExtractorPlugin.Logger.LogInfo("DDC model generation complete.");

        Application.Quit(0);
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