using BrutalAPI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace net.squish.Bebo {
    public class Bebo_Fool {
        public static void Add() {
            Character yourCharacter = new Character("Bebo", "Bebo_CH") {
                HealthColor = Pigments.Purple,
                UsesBasicAbility = true, //set this to false if you don't want your fool to use slap
                UsesAllAbilities = false, //set this to true if you want your fool to use all their abilities (like Longliver, Gospel or Mordrake)
                MovesOnOverworld = true, //set this to false if you want your fool to move like Gospel or Leviat in the overworld
                //BasicAbility = basicAbility //if your character has a different basic ability than Slap, you need to define it above the character code (this block) to call it here
                FrontSprite = ResourceLoader.LoadSprite("Bebo_front", new Vector2(0.5f, 0f), 32), // Only needs Image file name as long as it's embedded, no .png/.file addition needed.
                BackSprite = ResourceLoader.LoadSprite("CharacterNameBack", new Vector2(0.5f, 0f), 32),
                OverworldSprite = ResourceLoader.LoadSprite("Bebo_front", new Vector2(0.5f, 0f), 32), // Portrait and Overworld sprites are 64x64
                DamageSound = LoadedAssetsHandler.GetCharacter("Nowak_CH").damageSound, //character IDs end in _CH
                DeathSound = LoadedAssetsHandler.GetEnemy("Revola_EN").deathSound, //enemy IDs end in _EN
                DialogueSound = LoadedAssetsHandler.GetCharacter("Nowak_CH").dxSound, //.dxSound is for Dialogue sounds
                // Support - IgnoredAbilitiesForSupportBuilds
                IgnoredAbilitiesForDPSBuilds = [1], //For excluding abilities when game chooses fool loadout, not necessary for all fools
            };
            yourCharacter.GenerateMenuCharacter(ResourceLoader.LoadSprite("CharacterNameMenu"), ResourceLoader.LoadSprite("CharacterNameLocked")); //Menu Locked and Unlocked sprites are 32x48.
            yourCharacter.AddPassives([Passives.Catalyst, Passives.SkittishGenerator(2)]); // If you want a different existing passive at a different degree, most of them have a built-in generator.
            yourCharacter.SetMenuCharacterAsFullDPS(); // Sets a Support/DPS bias for your fool. Used when your Fool is picked randomly by the game.
            // Support - .SetMenuCharacterAsFullSupport()

            //general format for effects is EffectToDo EffectName = ScriptableObject.CreateInstance<EffectToDo>();
            //List of SOME (not all) effects - https://brutalorchestramodding.miraheze.org/wiki/All_base_game_Effects

            DamageEffect DirectDamage = ScriptableObject.CreateInstance<DamageEffect>();

            DamageEffect IndirectDamage = ScriptableObject.CreateInstance<DamageEffect>(); //DamageEffect has multiple further definitions, including specifying Indirect Damage
            IndirectDamage._indirect = IndirectDamage;

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

            //NOTES: Right Clicking an Effect (like DamageEffect) will tell you what additional definitions can be chosen for certain things, like Indirect Damage or Status Effects.
            //Typing things out manually is helpful, VS will bring up a menu of available items to enter. Use it to your advantage.

            //Ability 1. 4 different scales needed, 1 for each level.
            Ability ability0 = new Ability("Repetability", "Ability_1_A") {
                Description = "Perform one random fool ability. Has a 70/50/50/30 percent chance to apply fleeting(1) to this party member",
                AbilitySprite = ResourceLoader.LoadSprite("AbilityIcon"),
                Cost = [Pigments.Red, Pigments.Blue],
                Visuals = Visuals.Scream, //Visuals are now under 'Visuals. '. List here: https://github.com/kondorriano/BrutalAPI/wiki/Visuals Visual Aid here: https://www.youtube.com/watch?v=YJsGBPA-OP0
                AnimationTarget = Targeting.Slot_Front,
                Effects =
                [
                    Effects.GenerateEffect(IndirectDamage, 7, Targeting.Slot_Front), // In order, calls for (EffectToDo, #ToApply, Targeting)
                ]
            };
            ability0.AddIntentsToTarget(Targeting.Slot_Front, [nameof(IntentType_GameIDs.Damage_7_10)]); //Damage_#_# is parameters for damage calculations. Heal_#_# for heals. Ranges for all numbers between given #s. EX: 7,8,9,10.

            Ability ability1 = new Ability("Crazy Ability", "Ability_2_A") {
                Description = "Deal 10-11 indirect damage to the Opposing enemy.",
                AbilitySprite = ResourceLoader.LoadSprite("AbilityIcon"),
                Cost = [Pigments.Red, Pigments.BlueYellow], //Split pigments are under one name now
                Visuals = Visuals.Scream,
                AnimationTarget = Targeting.Slot_Front,
                Effects =
                [
                    Effects.GenerateEffect(IndirectDamage, 11, Targeting.Slot_Front),
                ]
            };
            ability1.AddIntentsToTarget(Targeting.Slot_Front, new string[] { IntentType_GameIDs.Damage_7_10.ToString(), IntentType_GameIDs.Damage_11_15.ToString() }); //Good format to call multiple calculations.
            
            //Ability 2.
            Ability otherAbility0 = new Ability("The Other Ability", "otherAbility_1_A") {
                Description = "Apply 5 Shield to this position.\nApply 1 Scar to the Left and Right enemies.",
                AbilitySprite = ResourceLoader.LoadSprite("OtherAbilityIcon"),
                Cost = [Pigments.Purple, Pigments.Blue],
                Visuals = Visuals.Chomp,
                AnimationTarget = Targeting.Slot_SelfSlot,
                Effects =
                [
                    Effects.GenerateEffect(ShieldApply, 5, Targeting.Slot_SelfSlot),
                    Effects.GenerateEffect(ScarsApply, 1, Targeting.Slot_OpponentSides),
                ]
            };
            otherAbility0.AddIntentsToTarget(Targeting.Slot_SelfSlot, [nameof(IntentType_GameIDs.Field_Shield)]);
            otherAbility0.AddIntentsToTarget(Targeting.Slot_OpponentSides, [nameof(IntentType_GameIDs.Status_Scars)]); //Intents are added sequentially

            Ability otherAbility1 = new Ability("The Other Ability", "otherAbility_2_A") {
                Description = "Apply 10 Shield to this position.\nApply 1 Scar to the Left, Right, and Opposing enemies.",
                AbilitySprite = ResourceLoader.LoadSprite("OtherAbilityIcon"),
                Cost = [Pigments.Purple, Pigments.Blue],
                Visuals = Visuals.Chomp,
                AnimationTarget = Targeting.Slot_SelfSlot,
                Effects =
                [
                    Effects.GenerateEffect(ShieldApply, 10, Targeting.Slot_SelfSlot),
                    Effects.GenerateEffect(ScarsApply, 1, Targeting.Slot_FrontAndSides),
                ]
            };
            otherAbility1.AddIntentsToTarget(Targeting.Slot_SelfSlot, [nameof(IntentType_GameIDs.Field_Shield)]);
            otherAbility1.AddIntentsToTarget(Targeting.Slot_FrontAndSides, [nameof(IntentType_GameIDs.Status_Scars)]);

            //Ability 3.
            Ability thirdAbility0 = new Ability("Third Ability", "thirdAbility_1_A") {
                Description = "Deal 2 damage to the Opposing enemy.\nApply 1 Fleeting to this party member.",
                AbilitySprite = ResourceLoader.LoadSprite("ThirdAbilityIcon"),
                Cost = [Pigments.Blue],
                Visuals = Visuals.Wriggle,
                AnimationTarget = Targeting.Slot_Front,
                Effects =
                [
                    Effects.GenerateEffect(DirectDamage, 2, Targeting.Slot_Front),
                    Effects.GenerateEffect(addPassiveEffect, 1, Targeting.Slot_SelfSlot),
                ]
            };
            thirdAbility0.AddIntentsToTarget(Targeting.Slot_Front, [nameof(IntentType_GameIDs.Status_Spotlight)]);
            thirdAbility0.AddIntentsToTarget(Targeting.Slot_SelfSlot, new string[1] { "FleetingIntent" }); //method of writing custom intents

            //Ability 3.
            Ability thirdAbility1 = new Ability("Third Ability", "thirdAbility_2_A") {
                Description = "Deal 4 damage to the Opposing enemy.\nApply 1 Fleeting to this party member. 20% chance to refresh this Party Member.",
                AbilitySprite = ResourceLoader.LoadSprite("ThirdAbilityIcon"),
                Cost = [Pigments.Blue],
                Visuals = Visuals.Wriggle,
                AnimationTarget = Targeting.Slot_Front,
                Effects =
                [
                    Effects.GenerateEffect(DirectDamage, 4, Targeting.Slot_Front),
                    Effects.GenerateEffect(addPassiveEffect, 1, Targeting.Slot_SelfSlot, refreshOne), // refresh rates can be stuck nicely right after your Targeting.
                ]
            };
            
            thirdAbility1.AddIntentsToTarget(Targeting.Slot_Front, [nameof(IntentType_GameIDs.Status_Spotlight)]);
            thirdAbility1.AddIntentsToTarget(Targeting.Slot_SelfSlot, new string[1] { "FleetingIntent" }); //method of writing custom intents
            
            yourCharacter.AddLevelData(1, new Ability[] { ability0, otherAbility0, }); //# (5) is health at level.
            yourCharacter.AddLevelData(10, new Ability[] { ability0, otherAbility0 }); //Levels are added sequentially
            yourCharacter.AddLevelData(15, new Ability[] { ability1, otherAbility1 });
            yourCharacter.AddLevelData(20, new Ability[] { ability1, otherAbility1 });
            
            
            yourCharacter.AddCharacter(true, false); //The first bool (true/false) is whether the fool is unlocked initially. The second is whether they show up in shops.
        }
    }
}
