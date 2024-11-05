using Core.DataCenter;
using Core.DataCenter.Metadata.Alliance;
using Core.DataCenter.Metadata.Appearance;
using Core.DataCenter.Metadata.Bonus;
using Core.DataCenter.Metadata.Bonus.Criteria;
using Core.DataCenter.Metadata.Breed;
using Core.DataCenter.Metadata.Challenge;
using Core.DataCenter.Metadata.Effect;
using Core.DataCenter.Metadata.Effect.Instance;
using Core.DataCenter.Metadata.Guild;
using Core.DataCenter.Metadata.House;
using Core.DataCenter.Metadata.Idol;
using Core.DataCenter.Metadata.Item;
using Core.DataCenter.Metadata.Job;
using Core.DataCenter.Metadata.Monster;
using Core.DataCenter.Metadata.Npc;
using Core.DataCenter.Metadata.OptionalFeatures;
using Core.DataCenter.Metadata.Progression;
using Core.DataCenter.Metadata.Quest;
using Core.DataCenter.Metadata.Quest.Objective;
using Core.DataCenter.Metadata.Seasons;
using Core.DataCenter.Metadata.Social;
using Core.DataCenter.Metadata.Spell;
using Core.DataCenter.Metadata.World;
using Core.DataCenter.Types;
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

namespace DDC.Extractor;

internal static class SubtypeCasting
{

    //public static Type[] GetSubTypes(Type t)
    //{
    //    var types = t.Assembly.GetTypes().Where(t => t.BaseType == t);
    //    return types.ToArray();
    //}

    public static object Converts(Il2CppObjectBase b)
    {
        object[] converts = [b];

        switch (b)
        {
            case EffectInstance:
                converts = [
                    b.TryCast<EffectInstanceDice>(), b.TryCast<EffectInstanceMinMax>(), b.TryCast<EffectInstanceInteger>(),
                    b.TryCast<EffectInstanceDate>(), b.TryCast<EffectInstanceDuration>(), b.TryCast<EffectInstanceMount>(),
                    b.TryCast<EffectInstanceLadder>(), b.TryCast<EffectInstanceCreature>(), b.TryCast<EffectInstanceString>(),
                ];
                break;
            case Items:
                converts = [
                    b.TryCast<Weapons>(),
                ];
                break;
            case Bonuses:
                converts = [
                    b.TryCast<MonsterDropChanceBonus>(), b.TryCast<MonsterStarRateBonus>(), b.TryCast<MonsterXPBonus>(),
                    b.TryCast<MonsterBonus>(), b.TryCast<MonsterLightBonus>(),
                    b.TryCast<MountBonus>(),
                    b.TryCast<QuestBonus>(),b.TryCast<QuestKamasBonus>(), b.TryCast<QuestXPBonus>(),
                ];
                break;
            case BonusesCriterions:
                converts = [
                    b.TryCast<BonusesAreaCriterion>(), b.TryCast<BonusesEquippedItemCriterion>(), b.TryCast<BonusesMonsterCriterion>(),
                    b.TryCast<BonusesMonsterFamilyCriterion>(), b.TryCast<BonusesQuestCategoryCriterion>(), b.TryCast<BonusesSubAreaCriterion>(),
                ];
                break;
            case QuestObjectives:
                converts = [
                    b.TryCast<QuestObjectiveBringItemToNpc>(), b.TryCast<QuestObjectiveBringSoulToNpc>(),
                    b.TryCast<QuestObjectiveCraftItem>(), b.TryCast<QuestObjectiveDiscoverMap>(), b.TryCast<QuestObjectiveDiscoverSubArea>(),
                    b.TryCast<QuestObjectiveDuelSpecificPlayer>(), b.TryCast<QuestObjectiveFightMonster>(), b.TryCast<QuestObjectiveFightMonstersOnMap>(),
                    b.TryCast<QuestObjectiveFreeForm>(), b.TryCast<QuestObjectiveGoToNpc>(), b.TryCast<QuestObjectiveMultiFightMonster>(),
                ];
                break;
            case Seasons:
                converts = [
                    b.TryCast<ArenaLeagueSeasons>(),
                    b.TryCast<ExpeditionSeasons>(),
                    b.TryCast<ServerSeasons>(),
                ];
                break;
            case AnimFunData:
                converts = [
                    b.TryCast<AnimFunMonsterData>(),
                    b.TryCast<AnimFunNpcData>(),
                    b.TryCast<NestedAnimFunNpcData>(),
                ];
                break;
            case SocialRightsGroup:
                converts = [b.TryCast<AllianceRightGroups>(), b.TryCast<GuildRightGroups>(),];
                break;
            case SocialRights:
                converts = [b.TryCast<AllianceRights>(), b.TryCast<GuildRights>(),];
                break;
            case SocialTags:
                converts = [b.TryCast<AllianceTags>(), b.TryCast<GuildTags>(),];
                break;
            case SocialTagsTypes:
                converts = [b.TryCast<AllianceTagsTypes>(), b.TryCast<GuildTagsTypes>(),];
                break;
        }
        return converts.FirstOrDefault(o => o != null) ?? b;
    }

}
