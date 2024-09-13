using Core.DataCenter.Metadata.Item;
using DDC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDC.Extractor.Converters;

internal class ItemConverter : IConverter<Items, Models.Item>
{
    public static ItemConverter instance = new();

    public Models.Item Convert(Items data) =>
        new()
        {
            possibleEffects = data.possibleEffects._items.Select(EffectInstanceConverter.instance.Convert).ToList(),
            //itemType = new()
            //{
            //    id = data.itemType.id,
            //    nameId = data.itemType.nameId,
            //    superTypeId = data.itemType.superTypeId,
            //    categoryId = data.itemType.categoryId,
            //    isInEncyclopedia = data.itemType.isInEncyclopedia,
            //},
            id = data.id,
            name = data.name,
            level = data.level,
            nameId = data.nameId,
            typeId = data.typeId,
            descriptionId = data.descriptionId,
            evolutiveEffectIds = data.evolutiveEffectIds.ToArray().ToList(),
            //category = data.category,
            isLegendary = data.isLegendary,
            changeVersion = data.changeVersion,
            iconId = data.iconId,
            itemSetId = data.itemSetId,
            appearanceId = data.appearanceId,
            criteria = data.criteria,
            criteriaTarget = data.criteriaTarget,
            bonusIsSecret = data.bonusIsSecret,
            isColorable = data.isColorable,
            isDestructible = data.isDestructible,
            isSaleable = data.isSaleable,
            objectIsDisplayOnWeb = data.objectIsDisplayOnWeb,
            adminSelectionIconId = data.adminSelectionIconId,
            adminSelectionId = data.adminSelectionId,
            adminSelectionName = data.adminSelectionName,
            adminSelectionTypeId = data.adminSelectionTypeId,
            //categoryName = data.categoryName,
            craftConditional = data.craftConditional,
            craftFeasible = data.craftFeasible,
            craftVisible = data.craftVisible,
            craftXpRatio = data.craftXpRatio,
            cursed = data.cursed,
            //description = data.description,
            dropMonsterIds = data.dropMonsterIds.ToList(),
            dropTemporisMonsterIds = data.dropTemporisMonsterIds.ToList(),
            enhanceable = data.enhanceable,
            etheral = data.etheral,
            exchangeable = data.exchangeable,
            favoriteRecyclingSubareas = data.favoriteRecyclingSubareas.ToList(),
            recyclingNuggets = data.recyclingNuggets,
            favoriteSubAreas = data.favoriteSubAreas.ToList(),
            hideEffects = data.hideEffects,
            favoriteSubAreasBonus = data.favoriteSubAreasBonus,
            importantNoticeId = data.importantNoticeId,
            m_flags = data.m_flags,
            needUseConfirm = data.needUseConfirm,
            realWeight = data.realWeight,
            price = data.price,
            recipeSlots = data.recipeSlots,
            targetable = data.targetable,
            tooltipExpirationDate = data.tooltipExpirationDate,
            twoHanded = data.twoHanded,
            unDiacriticalName = data.unDiacriticalName,
            weight = data.weight,
            useAnimationId = data.useAnimationId,
            visibility = data.visibility,
            recipeIds = data.recipeIds.ToList(),
            nonUsableOnAnother = data.nonUsableOnAnother,
            secretRecipe = data.secretRecipe,
            usable = data.usable,
        };
}
