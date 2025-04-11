using BrutalAPI;
using System;
using System.Collections.Generic;
using System.Text;
using BepInEx;
using BepInEx.Logging;
using UnityEngine;

namespace net.squish.Bebo {
    public class BeboFool {
        
        public static void Add() {
            
            // Character Definition
            Character Bebo = new Character("Bebo", "Bebo_CH") {
                HealthColor = Pigments.Blue,
                UsesBasicAbility = true,
                UsesAllAbilities = false, 
                MovesOnOverworld = true, 
                FrontSprite = ResourceLoader.LoadSprite("BeboFront", new Vector2(0.5f, 0f), 32), 
                BackSprite = ResourceLoader.LoadSprite("CharacterNameBack", new Vector2(0.5f, 0f), 32),
                OverworldSprite = ResourceLoader.LoadSprite("BeboFront", new Vector2(0.5f, 0f), 32),
                DamageSound = LoadedAssetsHandler.GetCharacter("Nowak_CH").damageSound,                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     
                DeathSound = LoadedAssetsHandler.GetEnemy("Revola_EN").deathSound, 
                DialogueSound = LoadedAssetsHandler.GetCharacter("Nowak_CH").dxSound, 
                // Support - IgnoredAbilitiesForSupportBuilds
                IgnoredAbilitiesForDPSBuilds = [1], //For excluding abilities when game chooses fool loadout, not necessary for all fools
            };
            
            Bebo.GenerateMenuCharacter(ResourceLoader.LoadSprite("CharacterNameMenu"), ResourceLoader.LoadSprite("CharacterNameLocked")); //Menu Locked and Unlocked sprites are 32x48.
            Bebo.AddPassives([]); // If you want a different existing passive at a different degree, most of them have a built-in generator.
            Bebo.SetMenuCharacterAsFullDPS(); // Sets a Support/DPS bias for your fool. Used when your Fool is picked randomly by the game.
            // Support - .SetMenuCharacterAsFullSupport()

            //general format for effects is EffectToDo EffectName = ScriptableObject.CreateInstance<EffectToDo>();
            //List of SOME (not all) effects - https://brutalorchestramodding.miraheze.org/wiki/All_base_game_Effects

            DamageEffect DirectDamage = ScriptableObject.CreateInstance<DamageEffect>();

            DamageEffect IndirectDamage = ScriptableObject.CreateInstance<DamageEffect>(); //DamageEffect has multiple further definitions, including specifying Indirect Damage
            IndirectDamage._indirect = IndirectDamage;
			HealEffect healEffect = ScriptableObject.CreateInstance<HealEffect>();

            AddPassiveEffect addPassiveEffect = ScriptableObject.CreateInstance<AddPassiveEffect>();
            addPassiveEffect._passiveToAdd = Passives.FleetingGenerator(1);

            IntentInfoBasic fleetIntent = new IntentInfoBasic(); // some passives dont have intents by default, so you may have to create them yourself
            fleetIntent._color = Color.white;
            fleetIntent._sprite = Passives.Fleeting1.passiveIcon; //calls the sprite from the passive for the intent
            LoadedDBsHandler.IntentDB.AddNewBasicIntent("FleetingIntent", fleetIntent); //creates the actual intent to call in your move

            RefreshAbilityUseEffect refreshAbilityUseEffect = ScriptableObject.CreateInstance<RefreshAbilityUseEffect>();
            PercentageEffectCondition refreshOne = Effects.ChanceCondition(20); // chance rate of refreshing a fool's abilities.

            FieldEffect_Apply_Effect ShieldApply = ScriptableObject.CreateInstance<FieldEffect_Apply_Effect>();
            ShieldApply._Field = StatusField.Shield; //Both field and status effects are under StatusField now

            StatusEffect_Apply_Effect ScarsApply = ScriptableObject.CreateInstance<StatusEffect_Apply_Effect>(); //Sets up the general effect. Can't do anything without further definitions below.
            ScarsApply._Status = StatusField.Scars; //Defines what exact effect will be used. Even if the EffectName reads "ScarsApply", if it calls for StatusField.Rupture, it will apply Rupture.

            // Arrays for leveling up
            string[] ability1Names = { "Bat", "Swipe", "Claw", "Tear" };
            string[] ability2Names = { "Dip", "Splatter", "Coat", "Drown" };
            string[] ability3Names = { "Lean", "Lay", "Depend", "Love" };
            string[] fleetingChances = { "70", "50", "50", "30" };
			
			int[] poisonChances = { 1, 1, 2, 3 };
			int[] ability1Damage = { 3, 4, 4, 5};
			int[] beboHealing = { 5, 6, 7, 8 };
            
            int[] beboHealth = { 7, 8, 10, 12 };

            // Ability Loop
            for (int i = 0; i < 4; i++) {
                Ability ability1 = new Ability(ability1Names[i] + " Them Away", "ABILITY_1") {
                    Description = "Deal" + ability1Damage[i] + " to the opposing enemy and give (" + poisonChances[i] + ") poison to the opposing enemy. Has a " + fleetingChances[i] + " percent chance to apply fleeting(1) to this party member",
                    AbilitySprite = ResourceLoader.LoadSprite("Bebo.SwipeAttack"),
                    Cost = [Pigments.Red, Pigments.Blue],
                    Visuals = Visuals.Scream, //Visuals are now under 'Visuals. '. List here: https://github.com/kondorriano/BrutalAPI/wiki/Visuals Visual Aid here: https://www.youtube.com/watch?v=YJsGBPA-OP0
                    AnimationTarget = Targeting.Slot_Front,
                    Effects = [
                        Effects.GenerateEffect(IndirectDamage, ability1Damage[i] , Targeting.Slot_Front), // In order, calls for (EffectToDo, #ToApply, Targeting)
                    ]
                };
                ability1.AddIntentsToTarget(Targeting.Slot_Front, [nameof(IntentType_GameIDs.Damage_7_10)]); //Damage_#_# is parameters for damage calculations. Heal_#_# for heals. Ranges for all numbers between given #s. EX: 7,8,9,10.

                Ability ability2 = new Ability(ability2Names[i] + " them in Poison", "ABILITY_2") {
                    Description = "Give the opposing enemy 3 poison. Has a " + fleetingChances[i] + " Percent chance to apply fleeting(1) to this party member",
                    AbilitySprite = ResourceLoader.LoadSprite("AbilityIcon"),
                    Cost = [Pigments.Red, Pigments.Blue],
                    Visuals = Visuals.Scream,
                    AnimationTarget = Targeting.Slot_Front,
                    Effects = [
						// this damage is just placeholder
                        Effects.GenerateEffect(IndirectDamage, 5, Targeting.Slot_Front),
                    ]
                };

                Ability ability3 = new Ability(ability3Names[i] + " on Them", "ABILITY_3") {
                    Description = "Heal the Party Member to the Left for 5 Health. Has a " + fleetingChances[i] + " Percent chance to apply fleeting(1) to this party member",
                    AbilitySprite = ResourceLoader.LoadSprite("Bebo.CuddleAttack"),
                    Cost = [Pigments.Red, Pigments.Blue],
                    Visuals = Visuals.Scream,
                    AnimationTarget = Targeting.Slot_Front,
                    Effects = [
						// healing was weirdly easy to add
                        Effects.GenerateEffect(healEffect, beboHealing[i], Targeting.Slot_AllyLeft),
                    ]
                };
                Bebo.AddLevelData(beboHealth[i], new Ability[] { ability1, ability2, ability3 }); 
            }
            
            Bebo.AddCharacter(true, false); //The first bool (true/false) is whether the fool is unlocked initially. The second is whether they show up in shops.
        }
    }
}