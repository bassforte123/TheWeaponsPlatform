using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using Alexandria.ItemAPI;
using BepInEx;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using static UnityEngine.UI.CanvasScaler;


namespace TheWeaponsPlatform
{
    public class HeroinesEcho : AdvancedGunBehavior
    {
        public static void Add()
        {
            //GUN BLOCK
            
            //Newgun(x,y) where
            string SHORTNAME = "samusD";
            Gun gun = ETGMod.Databases.Items.NewGun("Heroine Dark", SHORTNAME);
            Game.Items.Rename("outdated_gun_mods:heroine_dark", TheWeaponsPlatformMain.MODPREFIX + ":"+SHORTNAME);
            gun.gameObject.AddComponent<HeroinesEcho>();

            //Gun descriptions
            gun.SetShortDescription("Dark Suit Acquired");
            gun.SetLongDescription("An Echo of a Heroine. Longer description.  damage to jammed and unjammed.");

            // Sprite setup
            gun.SetupSprite(null, SHORTNAME+"_idle_001", 8);
            gun.SetAnimationFPS(gun.shootAnimation, 10);
            gun.SetAnimationFPS(gun.reloadAnimation, 1);
            gun.SetAnimationFPS(gun.chargeAnimation, 10);
            tk2dSpriteAnimationClip clip = gun.spriteAnimator.GetClipByName(SHORTNAME+"_intro");  //startup animation
            gun.SetAnimationFPS(gun.introAnimation, 10);

            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).loopStart = 2;


            // Projectile setup
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(41) as Gun, true, false);
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(41) as Gun).muzzleFlashEffects;
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1f;
            gun.DefaultModule.cooldownTime = 0.2f;
            gun.DefaultModule.numberOfShotsInClip = 15;
            gun.SetBaseMaxAmmo(150);
            gun.gunClass = GunClass.CHARGE;
            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(41) as Gun).gunSwitchGroup; // GET RID OF THAT CURSED DEFAULT RELOAD NOISE
            gun.gunHandedness = GunHandedness.HiddenOneHanded;
            gun.carryPixelOffset += new IntVector2(0, 3);
            gun.barrelOffset.transform.localPosition += new Vector3(0f, -3f/16f);

            gun.quality = PickupObject.ItemQuality.A;
            gun.encounterTrackable.EncounterGuid = "Samus Dark";
            //gun.AddToSubShop(ItemBuilder.ShopType.Trorc);

            //cloning block
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(41) as Gun).DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;

            // UNCHARGED PROJECTILE
            projectile.SetProjectileSpriteRight("samusD", 6, 6, true, tk2dBaseSprite.Anchor.MiddleCenter, 4, 4);
            projectile.baseData.damage = 5f;
            projectile.baseData.speed = 25f;
            projectile.baseData.range = 100f;
            projectile.baseData.force = 10f;
            projectile.transform.parent = gun.barrelOffset;
            projectile.ignoreDamageCaps = false;
            projectile.AdditionalScaleMultiplier = 1f;
            //HomingModifier homing = projectile.gameObject.AddComponent<HomingModifier>();
            //homing.AngularVelocity = 1600f;
            //homing.HomingRadius = 50f;

            



            /*
        https://raw.githubusercontent.com/ModTheGungeon/ETGMod/master/Assembly-CSharp.Base.mm/Content/gungeon_id_map/items.txt

        projectile.AdditionalScaleMultiplier = 1;
        projectile.BlackPhantomDamageMultiplier = 1;
        projectile.BossDamageMultiplier = 1;
        projectile.ignoreDamageCaps = false;
        projectile.pierceMinorBreakables = false;
        BounceProjModifier bounce = projectile.gameObject.GetOrAddComponent<BounceProjModifier>();
        bounce.numberOfBounces += 1;
        bounce.damageMultiplierOnBounce = 1;
        bounce.bouncesTrackEnemies = false;
        bounce.TrackEnemyChance = 1;
        bounce.bounceTrackRadius = 7;

        projectile.hitEffects.alwaysUseMidair = true;  //use end of range effect if hits something
        projectile.hitEffects.midairInheritsFlip = false; //should impact be directional facing
        projectile.hitEffects.midairInheritsRotation = false; //should midair effect rotate

        gun.usesContinuousFireAnimation = false;
        gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
        gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).loopStart = 1;  //frame to start looping

        gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.FISH;
        OR
        gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
        gun.DefaultModule.customAmmoType = "Samus";
        https://mtgmodders.gitbook.io/etg-modding-guide/various-lists-of-ids-sounds-etc./all-custom-ammo-types

        Gun.DefaultModule.finalAmmoType;
        Gun.DefaultModule.finalCustomAmmoType;

        //bleed
        projectile.AppliesBleed = true;
        projectile.BleedApplyChance = 0.5f;

        //Charm
        projectile.AppliesCharm = true;
        projectile.CharmApplyChance = 0.5f;

        //cheese
        projectile.AppliesCheese = true;
        projectile.CheeseApplyChance = 0.5f;

        //fire
        projectile.AppliesFire = true;
        projectile.FireApplyChance = 0.5f; 

        //freeze
        projectile.AppliesFreeze = true;
        projectile.FreezeApplyChance = 0.5f;

        //poison
        projectile.AppliesPoison = true;
        projectile.PoisonApplyChance = 0.5f;

        //stun
        projectile.AppliesStun = true;
        projectile.StunApplyChance = 0.5f;
        projectile.AppliedStunDuration = 2f;

        */





            // CHARGE PROJECTILE
            Projectile chargeprojectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(274) as Gun).DefaultModule.projectiles[0]);
            chargeprojectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(chargeprojectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(chargeprojectile);
            chargeprojectile.baseData.damage = 70f;
            chargeprojectile.baseData.speed = 25f;
            chargeprojectile.baseData.range = 100f;
            chargeprojectile.baseData.force = 10f;
            chargeprojectile.transform.parent = gun.barrelOffset;
            chargeprojectile.AdditionalScaleMultiplier = 4f;
            //HungryProjectileModifier eat = chargeprojectile.gameObject.AddComponent<HungryProjectileModifier>();
            //eat.HungryRadius = 5f;
            //chargeprojectile.collidesWithProjectiles = true;
            //chargeprojectile.collidesWithEnemies = true; // this causes it to make a hit sound everytime it hits a projectile... 
            
            //PierceProjModifier pierce = chargeprojectile.gameObject.AddComponent<PierceProjModifier>();
            //pierce.penetratesBreakables = true;
            //pierce.penetration += 10000;


            ProjectileModule.ChargeProjectile chargeProj1 = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile,
                ChargeTime = 0f,
                VfxPool = null,
                AmmoCost = 1,
            };
            ProjectileModule.ChargeProjectile chargeProj2 = new ProjectileModule.ChargeProjectile
            {
                Projectile = chargeprojectile,
                ChargeTime = 1.5f,
                AmmoCost = 25,
            };
            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile> { chargeProj1, chargeProj2 };

            ETGMod.Databases.Items.Add(gun, false, "ANY");
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = "samus";
            ID = gun.PickupObjectId;

            //gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].eventAudio = "Play_rocket_reload";
            //gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].triggerEvent = true;
        }
        public static int ID;

        public override void OnPostFired(PlayerController player, Gun gun)
        {
            // Sound setup (gungroop overwrite?)
            gun.PreventNormalFireAudio = true;
            AkSoundEngine.PostEvent("Play_wpn_voidcannon_shot_01", gameObject);
        }
        private bool HasReloaded;
        protected override void Update()
        {
            if (gun.CurrentOwner)
            {

                if (!gun.PreventNormalFireAudio)
                {
                    this.gun.PreventNormalFireAudio = true;
                }
                if (!gun.IsReloading && !HasReloaded)
                {
                    this.HasReloaded = true;
                }
            }
        }
        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);
                AkSoundEngine.PostEvent("Play_WPN_SAA_reload_01", base.gameObject);
            }
        }
    }
}
