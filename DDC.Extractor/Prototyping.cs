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
using Core.DataCenter.Metadata.Alliance;
using Core.DataCenter.Metadata.Appearance;
using Core.DataCenter.Metadata.Breed;
using Core.DataCenter.Metadata.Effect;
using Core.DataCenter.Metadata.Effect.Instance;
using Core.DataCenter.Metadata.Idol;
using Core.DataCenter.Metadata.Item;
using Core.DataCenter.Metadata.Monster;
using Core.DataCenter.Metadata.Quest;
using Core.DataCenter.Metadata.Quest.TreasureHunt;
using Core.DataCenter.Metadata.Social;
using Core.DataCenter.Metadata.Spell;
using Core.DataCenter.Metadata.Stat;
using Core.DataCenter.Metadata.World;
using Core.DataCenter.Types;
using Core.Engine.Messages;
using Core.Localization;
using Il2CppInterop.Runtime.Runtime;
using Il2CppInterop.Runtime;
using Il2CppSystem;
using Il2CppSystem.IO;
using Metadata.Enums;
using UnityEngine;
using System.Runtime.InteropServices;
using Il2CppInterop.Runtime.InteropTypes;
using IntPtr = System.IntPtr;
using Il2CppSystem.Reflection;
using System.Runtime.CompilerServices;


namespace DDC.Extractor;

public static class Prototyping
{

    public static void TestSpells()
    {
        //Spells spells = new Spells();
        //SpellLevels spellLevels = new SpellLevels();

        //Extractor.Logger.LogMessage("Root types (" + ExtractRoots.rootTypes.Count + "): " + string.Join(", ", ExtractRoots.rootTypes.Select(t => t.Name)));
        //var spellLevel = DataCenterModule.spellLevelsRoot.GetObjectById(41053); //12984);
        //var eff = spellLevel.effects[0];
        //Extractor.Logger.LogInfo($"SpellLevel.EffectInstance: {eff.spellId}, e {eff.effectId}, {eff.effectUid}, c {eff.category}, z {eff.zoneDescr}={eff.zoneDescr.param1}x{eff.zoneDescr.shape}, // {eff.zoneSize}, {eff.zoneShape}");
        //var spellLevel2 = (Generated.Core.DataCenter.Metadata.Spell.SpellLevels) ExtractRoots.ConvertType(spellLevel.GetType(), spellLevel);
        //var eff2 = spellLevel2.effects[0];
        //Extractor.Logger.LogInfo($"SpellLevel2.EffectInstance: {eff2.spellId}, e {eff2.effectId}, {eff2.effectUid}, c {eff2.category}, z {eff2.zoneDescr}={eff2.zoneDescr.param1}x{eff2.zoneDescr.shape}, // {eff2.zoneSize}, {eff2.zoneShape}");
        //var eff3 = (Generated.Core.DataCenter.Metadata.Effect.EffectInstance) ExtractRoots.ConvertType(eff.GetType(), eff);
        //Extractor.Logger.LogInfo($"EffectInstance2: {eff3.spellId}, e {eff3.effectId}, {eff3.effectUid}, c {eff3.category}, z {eff3.zoneDescr}={eff3.zoneDescr.param1}x{eff3.zoneDescr.shape}, // {eff3.zoneSize}, {eff3.zoneShape}");
    }

    public static void TestWeapons()
    {
        Items baguette = DataCenterModule.itemsRoot.GetObjectById(25219);
        Extractor.Logger.LogWarning("baguette: " + baguette.GetType() + ", " + baguette.GetType().BaseType);
        Extractor.Logger.LogWarning("baguette is weapons: " + (baguette is Weapons));
        try
        {
            //var weapons = DataCenterModule.GetDataRoot<MetadataRoot<Weapons>>();
            //Extractor.Logger.LogWarning("weapons: " + weapons);
            Weapons wep = (Weapons) Weapons.GetItemById(25219);
            Extractor.Logger.LogWarning("wep: " + wep + ", " + wep.GetType() + ", " + wep.GetType().BaseType);
            Extractor.Logger.LogWarning("wep effect: " + wep.apCost);
        }
        catch (System.Exception e)
        {
            Extractor.Logger.LogError(e);
        }
        //baguette.possibleEffects
        //var weapons = DataCenterModule.itemsRoot.GetObjects()._items.Where(i => i is Weapons).ToList();
        //Extractor.Logger.LogWarning("weapons: " + weapons.Count);
    }

    public static void TestItemsEffects()
    {
        //DataCenterModule.LoadData();

        Items baguette = DataCenterModule.itemsRoot.GetObjectById(25219);
        Extractor.Logger.LogWarning("baguette: " + baguette.GetType() + ", " + baguette.GetType().BaseType);

        var vitality = baguette.possibleEffects._items.First(e => e.effectId == ActionId.CharacterBoostVitality);
        Extractor.Logger.LogWarning("baguette effect life: " + vitality + ", " + vitality.GetType());

        var wepbaguette = baguette.TryCast<Weapons>();
        Extractor.Logger.LogMessage("baguette weapon: " + wepbaguette.GetType() + ", " + wepbaguette.apCost);

        Extractor.Logger.LogMessage("--------");

        // pano bouftou
        var panoBouftou = DataCenterModule.itemSetsRoot.GetObjectById(1);
        EffectInstance effect = panoBouftou.effects[1].values[0];

        //Extractor.Logger.LogWarning("panoBouftou effect b0: " + effect + ", " + effect.GetType() + " vs " + effect.GetIl2CppType());
        //Extractor.Logger.LogWarning("panoBouftou effect 0 param 0: " + effect.m_memoizedValues?.parameter0 + " vs " + effect.parameter0);
        //Extractor.Logger.LogWarning("panoBouftou effect 0 param 1: " + effect.m_memoizedValues?.parameter1 + " vs " + effect.parameter1);
        //Extractor.Logger.LogWarning("panoBouftou effect 0 param 2: " + effect.m_memoizedValues?.parameter2 + " vs " + effect.parameter2);
        //Extractor.Logger.LogWarning("panoBouftou effect 0 param 3: " + effect.m_memoizedValues?.parameter3 + " vs " + effect.parameter3);
        //Extractor.Logger.LogWarning("panoBouftou effect 0 param 4: " + effect.m_memoizedValues?.parameter4 + " vs " + effect.parameter4);
        //Extractor.Logger.LogMessage("param0 is " + effect.parameter0.GetType() + " vs " + effect.parameter0.GetIl2CppType().InternalNameIfAvailable);
        //var val = (Il2CppSystem.Int32) asd.parameter0;

        var p = effect.parameter0;
        var t = p.GetIl2CppType();
        var name = t.InternalNameIfAvailable;
        //p.Pointer.


        //Extractor.Logger.LogWarning("panoBouftou effect 0 vs param0 obj class: " 
        //    + Marshal.PtrToStringAnsi(IL2CPP.il2cpp_class_get_name(effect.ObjectClass)) + " vs " 
        //    + Marshal.PtrToStringAnsi(IL2CPP.il2cpp_class_get_name(p.ObjectClass)));

        var dice = effect.TryCast<EffectInstanceDice>();
        var minmax = effect.TryCast<EffectInstanceMinMax>();
        var integer = effect.TryCast<EffectInstanceInteger>();
        Extractor.Logger.LogMessage($"effect cast: {dice}, {minmax}, {integer}");


        var effectTypeName = Marshal.PtrToStringAnsi(IL2CPP.il2cpp_class_get_name(effect.ObjectClass));
        var effectTypeNamespace = Marshal.PtrToStringAnsi(IL2CPP.il2cpp_class_get_namespace(effect.ObjectClass));
        var effectTypeAssembly = Marshal.PtrToStringAnsi(IL2CPP.il2cpp_class_get_assemblyname(effect.ObjectClass));
        var str = effectTypeNamespace + "." + effectTypeName + ", " + effectTypeAssembly;
        Extractor.Logger.LogWarning($"Effect type str: " + str);
        //System.Type cd = System.Type.GetType(str);
        //Extractor.Logger.LogWarning($"Effect type C#: " + cd);
        //var asdf = Il2CppObjectPool.Get<EffectInstanceDice>(effect.Pointer);
        //Extractor.Logger.LogWarning($"Effect value as EffectInstanceDice C#: " + asdf + ", " + asdf.diceNum + ", " + asdf.diceSide + ", " + asdf.value);

        IntPtr nativeClassPtr = Il2CppClassPointerStore<EffectInstanceDice>.NativeClassPtr;
        Extractor.Logger.LogInfo("Dice class ptr: " + nativeClassPtr + " vs obj class pointer " + effect.ObjectClass + ", pointer: " + effect.Pointer);

        var obj = TryCast2(effect);  //TryCast(effect, nativeClassPtr);
        Extractor.Logger.LogWarning($"Effect value as object class: " + obj + " is " + obj.GetType());

        effect.TryCast<EffectInstanceDice>();


        if (obj is EffectInstanceDice)
        {
            Extractor.Logger.LogWarning("obj effect: " + obj + " is dice");
        }
        else
        if (obj is EffectInstanceMinMax)
        {
            Extractor.Logger.LogWarning("obj effect: " + obj + " is minMax ");
        }
        else
        if (obj is EffectInstanceInteger)
        {
            Extractor.Logger.LogWarning("obj effect: " + obj + " is integer ");
        }
        else
        {
            Extractor.Logger.LogWarning("obj effect is nothing interesting");
        }

        //var weap = (Weapons) baguette;
        //Extractor.Logger.LogWarning("weap: " + weap);
        //baguette = DataCenterModule.s_itemsRootCached.GetObjectById(25219);
        //Extractor.Logger.LogWarning("baguette: " + baguette.GetType() + ", " + baguette.GetType().BaseType);
        //var weapons = DataCenterModule.s_itemsRootCached.GetObjects()._items.Where(i => i is Weapons).ToList();
        //Extractor.Logger.LogWarning("weapons: " + weapons.Count);
    }

    //public static object TryCast(Il2CppObjectBase obj, IntPtr nativeClassPtr) //System.Type csType)
    //{
    //    //IntPtr nativeClassPtr = Il2CppClassPointerStore<T>.NativeClassPtr;
    //    if (nativeClassPtr == IntPtr.Zero)
    //    {
    //        throw new System.ArgumentException($"{nativeClassPtr} is not an Il2Cpp reference type");
    //    }

    //    IntPtr intPtr = IL2CPP.il2cpp_object_get_class(obj.Pointer);
    //    if (!IL2CPP.il2cpp_class_is_assignable_from(nativeClassPtr, intPtr))
    //    {
    //        return null;
    //    }

    //    var isInjected = RuntimeSpecificsStore.IsInjected(intPtr);
    //    Extractor.Logger.LogMessage("is Injected: " + isInjected);
    //    var objcs = ClassInjectorBase.GetMonoObjectFromIl2CppPointer(obj.Pointer);
    //    Extractor.Logger.LogMessage("objcs: " + objcs);
    //    if (isInjected) // ClassInjectorBase.GetMonoObjectFromIl2CppPointer(obj.Pointer) is T result)
    //    {
    //        return objcs;
    //    }

    //    Extractor.Logger.LogMessage("Try Cast fail");
    //    ////return InitializerStore<T>.Initializer(obj.Pointer); // cant use, internal class
    //    return null;
    //}
    public static object TryCast2(Il2CppObjectBase obj)
    {
        IntPtr nativeClassPtr = obj.ObjectClass;// Il2CppClassPointerStore<T>.NativeClassPtr;
        if (nativeClassPtr == IntPtr.Zero)
        {
            throw new System.ArgumentException($"nativeClassPtr is not an Il2Cpp reference type");
        }

        IntPtr intPtr = IL2CPP.il2cpp_object_get_class(obj.Pointer);
        if (!IL2CPP.il2cpp_class_is_assignable_from(nativeClassPtr, intPtr))
        {
            return null;
        }

        var injected = RuntimeSpecificsStore.IsInjected(intPtr);
        var c = ClassInjectorBase.GetMonoObjectFromIl2CppPointer(obj.Pointer);
        //if ( &&  is T result)
        //{
        //    return result;
        //}
        return c;
        //return InitializerStore<T>.Initializer(Pointer);
        //unsafe
        //{
        //var asd = IL2CPP.il2cpp_object_unbox(obj.Pointer).ToPointer();
        //    return Unsafe.AsRef<T>(asd);
        //}
    }

    public static void pointerToEffect(EffectInstance ei)
    {
        //nint num = (nint) IL2CPP.Il2CppObjectBaseToPtrNotNull(this) + (int) IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_effects);
        //System.IntPtr intPtr = *(System.IntPtr*) num;
        //return (intPtr != (System.IntPtr) 0) ? Il2CppObjectPool.Get<List<WrappedEffectInstanceList>>(intPtr) : null;
    }

}
