using Core.DataCenter.Metadata.Effect;
using Core.DataCenter.Metadata.Item;
using Core.DataCenter.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Core.DataCenter.Metadata.Item.Items;

namespace DDC.Models;

public class Item
{
    public ItemFlags m_flags { get; init; }
    public ushort id { get; init; }
    public uint nameId { get; init; }
    public ushort typeId { get; init; }
    public uint descriptionId { get; init; }
    public int iconId { get; init; }
    public byte level { get; init; }
    public ushort realWeight { get; init; }
    public sbyte useAnimationId { get; init; }
    public float price { get; init; }
    public short itemSetId { get; init; }
    public string criteria { get; init; }
    public string criteriaTarget { get; init; }
    public ushort appearanceId { get; init; }
    public bool isColorable { get; init; }
    public byte recipeSlots { get; init; }
    public List<ushort> recipeIds { get; init; }
    public List<ushort> dropMonsterIds { get; init; }
    public List<ushort> dropTemporisMonsterIds { get; init; }

    public List<EffectInstance> possibleEffects { get; init; }

    public List<ushort> evolutiveEffectIds { get; init; }
    public List<ushort> favoriteSubAreas { get; init; }
    public ushort favoriteSubAreasBonus { get; init; }
    public short craftXpRatio { get; init; }
    public string craftVisible { get; init; }
    public string craftConditional { get; init; }
    public string craftFeasible { get; init; }
    public string visibility { get; init; }
    public float recyclingNuggets { get; init; }
    public List<int> favoriteRecyclingSubareas { get; init; }
    public List<List<int>> resourcesBySubarea { get; init; }
    public string importantNoticeId { get; init; }
    public string changeVersion { get; init; }
    public double tooltipExpirationDate { get; init; }
    //public MemoizedValues m_memoizedValues { get; init; }
    public bool cursed { get; init; }
    public bool usable { get; init; }
    public bool targetable { get; init; }
    public bool exchangeable { get; init; }
    public bool twoHanded { get; init; }
    public bool etheral { get; init; }
    public bool hideEffects { get; init; }
    public bool enhanceable { get; init; }
    public bool nonUsableOnAnother { get; init; }
    public bool secretRecipe { get; init; }
    public bool objectIsDisplayOnWeb { get; init; }
    public bool bonusIsSecret { get; init; }
    public bool needUseConfirm { get; init; }
    public bool isDestructible { get; init; }
    public bool isSaleable { get; init; }
    public bool isLegendary { get; init; }
    public uint weight { get; init; }
    //public string description { get; init; }
    //public ItemType itemType { get; init; }
    public string name { get; init; }
    public string unDiacriticalName { get; init; }
    //public IAdminSelectionEntryType adminSelectionEntryType { get; init; }
    public string adminSelectionName { get; init; }
    public int adminSelectionId { get; init; }
    public int adminSelectionIconId { get; init; }
    public uint adminSelectionTypeId { get; init; }
    //public ItemCategoryEnum category { get; init; }
    //public string categoryName { get; init; }
}
