using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using Alexandria.ItemAPI;
using Alexandria.SoundAPI;
using Alexandria.VisualAPI;
using BepInEx;
using System.Collections.Generic;


namespace TheWeaponsPlatform
{
    class ExampleActive : PlayerItem
    {
        public static int ID; //The Item ID stored by the game.  Can be used by other functions to call your custom item.

        public static void Register()
        {
            string itemName = "Example Active"; //The name of the item, becomes lowercase with underscores when used in the console.
            /* Instructions to embed a sprite https://mtgmodders.gitbook.io/etg-modding-guide/all-things-spriting/importing-a-sprite-to-visual-studios */
            string resourceName = "TheWeaponsPlatform/Resources/ActiveCollection/example_active_sprite"; //Refers to an embedded png in the project.

            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<ExampleActive>(); //Add a PassiveItem component to the object

            /* Adds a sprite component to the object and adds your texture to the item sprite collection */
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            /* Ammonomicon entry variables */
            string shortDesc = "Example Active Item.";
            string longDesc = "Example Long Description\n\n" +
                              "Trade HP for money.";

            /* Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
             * Must come after ItemBuilder.AddSpriteToObject! */
            ItemBuilder.SetupItem(item, shortDesc, longDesc, TheWeaponsPlatformMain.MODPREFIX);

            /* Active items can also have passive effects */
            //ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Health, 1, StatModifier.ModifyMethod.ADDITIVE); //Add 1 Max HP.

            /* How the cooldown recharges between
             * "Damage" dealth, "PreRoom" cleared, Timed, and None followed by the amount of cooldown.*/
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.None, 0f);

            item.consumable = false; //Should the item dissapear on use?

            item.quality = PickupObject.ItemQuality.C; //Set the rarity of the item.
            ID = item.PickupObjectId; //Set the Item ID.
        }

        /* Effects granted on pickup */
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
        }
        

        public override void DoEffect(PlayerController user)
        {
            if (user.healthHaver.currentHealth > 0.5f)
            {
                user.healthHaver.ApplyHealing(-0.5f);
                user.carriedConsumables.Currency += 15;
                /* You can set a sound effect to play on use as well
                 * List of default sound files: https://mtgmodders.gitbook.io/etg-modding-guide/various-lists-of-ids-sounds-etc./sound-list
                 * Instructions on setting up custom sound files: https://mtgmodders.gitbook.io/etg-modding-guide/misc/using-custom-sounds */
                AkSoundEngine.PostEvent("Play_CHR_general_hurt_01", gameObject); 
            }
        }
    }
}
