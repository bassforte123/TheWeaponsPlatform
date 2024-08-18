using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Collections.ObjectModel;
using System.IO;
using System.ComponentModel;  // Debug stuff
using System.Threading;

using BepInEx;
using UnityEngine;
using UnityEngine.UI;

using MonoMod;
using MonoMod.RuntimeDetour;
using MonoMod.Cil;
using Mono.Cecil.Cil; //Instruction
using SGUI;
using FullSerializer;
using HarmonyLib; //

using Gungeon;
using Dungeonator;
using HutongGames.PlayMaker; //FSM___ stuff
using HutongGames.PlayMaker.Actions; //FSM___ stuff
using Alexandria.ItemAPI;

using Alexandria.EnemyAPI;
using Alexandria.Misc;
using Alexandria.NPCAPI;
using Brave.BulletScript;


using ResourceExtractor = Alexandria.ItemAPI.ResourceExtractor;
using Component = UnityEngine.Component;
using ShopAPI = Alexandria.NPCAPI.ShopAPI;
using RoomFactory = Alexandria.DungeonAPI.RoomFactory;
using ExoticObjects = Alexandria.DungeonAPI.SetupExoticObjects;
using StaticReferences = Alexandria.DungeonAPI.StaticReferences;

using static ProjectileModule;      //ShootStyle, ProjectileSequenceStyle
using static tk2dBaseSprite;        //Anchor
using static PickupObject;          //ItemQuality
using static BasicBeamController;   //BeamState




namespace TheWeaponsPlatform
{
    [BepInDependency(Alexandria.Alexandria.GUID)] // this mod depends on the Alexandria API: https://enter-the-gungeon.thunderstore.io/package/Alexandria/Alexandria/
    [BepInDependency(ETGModMainBehaviour.GUID)]
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class TheWeaponsPlatformMain : BaseUnityPlugin
    {
        /* Update these values for your own mod. */
        public const string GUID = "authorname.etg.theweaponsplatform";
        public const string MODNAME = "The Weapons Platform";
        public const string MODPREFIX = "twp";
        public const string VERSION = "1.0.0";
        public const string TEXT_COLOR = "#FFFFFF";


        public void Start()
        {
            ETGModMainBehaviour.WaitForGameManagerStart(GMStart);
        }
        public void GMStart(GameManager g)
        {
            Log($"{MODNAME} v{VERSION} started successfully.", TEXT_COLOR);

            BasicGun.Add();
            BasicChargeGun.Add();
            TemplateGun.Add();
            ExamplePassive.Register();
            ExampleActive.Register();

            CustomActions.OnRunStart += DropItemsOnStart;
                HP = 200; //Set the test dummy's hp.
                dummyMax = 1; //How many dummies to spawn (0-3).
            CustomActions.OnRunStart += SpawnTargetDummy;
            CustomActions.OnAnyHealthHaverDie += RespawnTargetDummy;
        }

        ////START OF TESTING FUNCTIONS////

        public static void DropItemsOnStart(PlayerController arg1, PlayerController arg2, GameManager.GameMode arg3)
        {
            // List the IDs of items you want to automatically spawn at the start of the game.
            // https://raw.githubusercontent.com/ModTheGungeon/ETGMod/master/Assembly-CSharp.Base.mm/Content/gungeon_id_map/items.txt
            List<int> itemList = new List<int> {
                BasicGun.ID,
                BasicChargeGun.ID,
                TemplateGun.ID,
                ExamplePassive.ID,
                ExampleActive.ID,
            };

            for (int i = 0; i < itemList.Count; i++)
              LootEngine.SpawnItem(PickupObjectDatabase.GetById(itemList.ElementAt(i)).gameObject, SpotNear(arg1.CenterPosition), Vector2.up, 1f, true, false, false);
        }

        private static Vector2 SpotNear(Vector2 where)
        {
            Vector2 offset = new Vector2(2f, 2f);
            Vector2 pos = BraveUtility.RandomVector2(where - offset, where + offset);
            return pos;
        }

        public static int HP;
        public static int dummyMax;
        public static int dummyCount;
        private static void SpawnTargetDummy(PlayerController arg1, PlayerController arg2, GameManager.GameMode arg3)
        {
            //List of enemy IDs https://mtgmodders.gitbook.io/etg-modding-guide/various-lists-of-ids-sounds-etc./enemy-guids
            AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5"); //Bullet Kin ID.

            if (dummyMax >= 1)
            {
                // White/Red Dummy
                IntVector2? intVectorA = (GameManager.Instance.Dungeon.data.Entrance.Epicenter.ToVector2() + new Vector2(8f, -2f)).ToIntVector2();
                AIActor dummyA = AIActor.Spawn(orLoadByGuid.aiActor, intVectorA.Value, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(intVectorA.Value), true, AIActor.AwakenAnimationType.Spawn, true);
                dummyA.healthHaver.IsVulnerable = true;
                dummyA.IgnoreForRoomClear = true;
                dummyA.IsHarmlessEnemy = false;
                dummyA.CanTargetPlayers = false; //Whether the test dummy tries to attack you.
                dummyA.healthHaver.maximumHealth = HP;
                dummyA.healthHaver.currentHealth = HP;
                if(dummyMax == 1)dummyA.RegisterOverrideColor(new Color(3f, 3f, 3f, 1f), "Pale BulletKin");
                else dummyA.RegisterOverrideColor(new Color(2f, 0f, 0f, 1f), "Pale BulletKin");
            }

            if (dummyMax >= 2)
            {
                // Blue Dummy
                IntVector2? intVectorB = (GameManager.Instance.Dungeon.data.Entrance.Epicenter.ToVector2() + new Vector2(8f, 2f)).ToIntVector2();
                AIActor dummyB = AIActor.Spawn(orLoadByGuid.aiActor, intVectorB.Value, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(intVectorB.Value), true, AIActor.AwakenAnimationType.Spawn, true);
                dummyB.healthHaver.IsVulnerable = true;
                dummyB.IgnoreForRoomClear = true;
                dummyB.IsHarmlessEnemy = false;
                dummyB.CanTargetPlayers = false; //Whether the test dummy tries to attack you.
                dummyB.healthHaver.maximumHealth = HP;
                dummyB.healthHaver.currentHealth = HP;
                dummyB.RegisterOverrideColor(new Color(0f, 0f, 2f, 1f), "Pale BulletKin");
            }

            if (dummyMax >= 3)
            {
                // Green Dummy
                IntVector2? intVectorC = (GameManager.Instance.Dungeon.data.Entrance.Epicenter.ToVector2() + new Vector2(8f, -6f)).ToIntVector2();
                AIActor dummyC = AIActor.Spawn(orLoadByGuid.aiActor, intVectorC.Value, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(intVectorC.Value), true, AIActor.AwakenAnimationType.Spawn, true);
                dummyC.healthHaver.IsVulnerable = true;
                dummyC.IgnoreForRoomClear = true;
                dummyC.IsHarmlessEnemy = false;
                dummyC.CanTargetPlayers = false; //Whether the test dummy tries to attack you.
                dummyC.healthHaver.maximumHealth = HP;
                dummyC.healthHaver.currentHealth = HP;
                dummyC.RegisterOverrideColor(new Color(0f, 2f, 0f, 1f), "Pale BulletKin");
            }
            dummyCount = dummyMax;
        }
        private static void RespawnTargetDummy(HealthHaver targetdummy)
        {
            // Only trigger respawn when all Dummies have been killed.
            if (targetdummy.maximumHealth == HP) dummyCount -= 1;
            if (dummyCount == 0)
                        SpawnTargetDummy(GameManager.Instance.PrimaryPlayer, GameManager.Instance.SecondaryPlayer, GameManager.GameMode.NORMAL);
        }

        ////END OF TESTING FUNCTIONS////

        public static void Log(string text, string color = "#FFFFFF")
        {
            ETGModConsole.Log($"<color={color}>{text}</color>");
        }
    }
}
