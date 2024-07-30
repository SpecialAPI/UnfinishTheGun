using BepInEx;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace UnfinishTheGun
{
    [BepInPlugin(GUID, NAME, VERSION)]
    [HarmonyPatch]
    public class Plugin : BaseUnityPlugin
    {
        public const string GUID = "spapi.etg.unfinishthegun";
        public const string NAME = "Unfinish the Gun";
        public const string VERSION = "1.0.0";

        public static MethodInfo utg_u = AccessTools.Method(typeof(Plugin), nameof(UnfinishTheGun_Unfinish));

        public void Awake()
        {
            new Harmony(GUID).PatchAll();
        }

        [HarmonyPatch(typeof(RewardManager), nameof(RewardManager.ExcludeUnfinishedGunIfNecessary))]
        [HarmonyPatch(typeof(RewardManager), nameof(RewardManager.BuildExcludedShopList))]
        [HarmonyPatch(typeof(PickupObjectDatabase), nameof(PickupObjectDatabase.GetRandomGunOfQualities))]
        [HarmonyPatch(typeof(LootData), nameof(LootData.GetItemsForPlayer))]
        [HarmonyPatch(typeof(LootEngine), nameof(LootEngine.SpawnItem))]
        [HarmonyPatch(typeof(LootEngine), nameof(LootEngine.SpewLoot), typeof(GameObject), typeof(Vector3))]
        [HarmonyILManipulator]
        public static void UnfinishTheGun_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            while(crs.TryGotoNext(MoveType.After, x => x.MatchCallOrCallvirt<GameStatsManager>(nameof(GameStatsManager.GetFlag))))
            {
                crs.Emit(OpCodes.Call, utg_u);
            }
        }

        public static bool UnfinishTheGun_Unfinish(bool _)
        {
            return false;
        }
    }
}
