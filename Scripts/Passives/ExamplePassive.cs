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
    public class ExamplePassive : PassiveItem
    {
        public static int ID; //The Item ID stored by the game.  Can be used by other functions to call your custom item.
        private bool ArmorGiven; //Flag so that an armor giving item only gives armor once.
        public static void Register()
        {
            string itemName = "Example Passive"; //The name of the item, becomes lowercase with underscores when used in the console.
            /* Instructions to embed a sprite https://mtgmodders.gitbook.io/etg-modding-guide/all-things-spriting/importing-a-sprite-to-visual-studios */
            string resourceName = "TheWeaponsPlatform/Resources/PassiveCollection/example_passive_sprite"; //Refers to an embedded png in the project.

            GameObject obj = new GameObject(itemName); 
            var item = obj.AddComponent<ExamplePassive>(); //Add a PassiveItem component to the object

            /* Adds a sprite component to the object and adds your texture to the item sprite collection */
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            /* Ammonomicon entry variables */
            string shortDesc = "Example Short Desc.";
            string longDesc = "Example Long Description\n\n" +
                              "Grants 1 max HP and 1 armor.";

            /* Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
             * Must come after ItemBuilder.AddSpriteToObject! */
            ItemBuilder.SetupItem(item, shortDesc, longDesc, TheWeaponsPlatformMain.MODPREFIX);

            /* Adds the actual passive effect to the item */
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Health, 1, StatModifier.ModifyMethod.ADDITIVE); //Add 1 Max HP.

            item.quality = PickupObject.ItemQuality.C; //Set the rarity of the item.
            ID = item.PickupObjectId; //Set the Item ID.
        }

        /* Effects granted on pickup */
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            if (!ArmorGiven)
            {
                GameManager.Instance.PrimaryPlayer.healthHaver.Armor += 1; //Give 1 armor.
                ArmorGiven = true; //Flat that armor has been given so it should not be given again.
            }
        }
        /* Effects that occur when dropped */
        public override void DisableEffect(PlayerController player)
        {
            if (GameManager.Instance.PrimaryPlayer.healthHaver.Armor > 0)
                GameManager.Instance.PrimaryPlayer.healthHaver.Armor -= 1; //Remove 1 armor if item is dropped.
        }
    }
}
        