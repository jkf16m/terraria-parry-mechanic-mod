using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameInput;
using System.Collections.Generic;
using parry_mechanic;
using parry_mechanic.Content.CriticalZenith;
using parry_mechanic.Content.Network;

namespace parry_mechanic.Content.Parry
{
    public class ParryModPlayer : ModPlayer
    {
        private GameplayModConfigService    gameplayModConfigService;
        private VisualModConfigService      visualModConfigService;
        private ParryModKeybindService      parryModKeybindService;
        private NetworkService              networkService;

        public override void Initialize()
        {
            gameplayModConfigService    = Container.Resolve<GameplayModConfigService>();
            visualModConfigService      = Container.Resolve<VisualModConfigService>();
            parryModKeybindService      = Container.Resolve<ParryModKeybindService>();
        }

        void AddParryBuff(BinaryReader reader, int whoAmI)
        {
            int player = reader.ReadByte();
            if (Main.netMode == NetmodeID.Server)
            {
                player = whoAmI;
            }

        }

        /**
         * <summary>The minimal mana amount to be able to use the Parry ability</summary>
         */
        private int parryMinimumManaCost = 20;
        /**
         * <summary>The max spent amount, in the worst case scenario, for the most imprecise parry.</summary>
         */
        private int parryMaxManaCost = 100;
        /**
         * <summary>The invulnerability time in frames</summary>
         */
        private int invulnerabilityTime = 50;
        /**
         * <summary>The parry time window in frames</summary>
         */
        private int parryTimeWindow = 50;

        /**
         * <summary>The damage multiplier, when the player is currently using parry mechanics.</summary> 
         */
        private float takenDamageMultiplier = 1.25f;

        /**
         * <summary>indicates how much the buff will last, in game ticks (60 = 1 second)</summary>
         */
        private int heightenedSensesBuffTime = 120;

        // These 3 fields relate to the Example Dodge. Example Dodge is modeled after the dodge ability of the Hallowed armor set bonus.
        // parryDodge indicates if the player actively has the ability to dodge the next attack. This is set by parryDodgeBuff, which in this example is applied by the HitModifiersShowcase weapon. The buff is only applied if parryDodgeCooldown is 0 and will be cleared automatically if an attack is dodged or if the player is no longer holding HitModifiersShowcase.
        public bool parryDodge;

        public override void OnConsumeAmmo(Item weapon, Item ammo)
        {

        }


        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (parryModKeybindService.ParryKeybind.JustPressed
                && Player.CheckMana(parryMinimumManaCost, false, false)
                && Player.HasBuff(ModContent.BuffType<StrainedReflexesDebuff>()) == false
                && Player.HasBuff(ModContent.BuffType<ParryBuff>()) == false)
            {
                Player.AddBuff(ModContent.BuffType<ManaVeilBuff>(), 99999, false, false);
                Player.AddBuff(ModContent.BuffType<ParryBuff>(), parryTimeWindow, false, false);

                // small visual particles
                for (int i = 0; i < 20; i++)
                {
                    Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                    Dust d = Dust.NewDustPerfect(Player.Center + speed * 4, DustID.PurpleCrystalShard, speed * 2, Scale: 1f);
                    d.noGravity = true;
                }
                SoundEngine.PlaySound(SoundID.MaxMana with { Pitch = 2f, Volume = 0.5f });
            }
        }
        private List<Dust> thunderDusts = new List<Dust>();
        private int thunderDustsTime = 0;
        private int thunderDustsMaxTime = 4;
        private void CriticalZenithEffects(Vector2 effectsOrigin)
        {
            Player.SetImmuneTimeForAllTypes(invulnerabilityTime / 2);

            var randomThundersAmount = Main.rand.Next(1, 3);

            for (int i = 0; i < randomThundersAmount; i++)
            {
                var randomPitch = Main.rand.NextFloat(1.5f, 3f);
                SoundEngine.PlaySound(SoundID.Thunder with { Pitch = randomPitch, Volume = 0.5f });
            }

            var originBifurcations = 3;

            var thunderPoints = ThunderVisualEffects.GenerateThunderPoints(Player.Center, originBifurcations, 300f,
                50, 2f);

            foreach (var thunderPoint in thunderPoints)
            {
                var whichThunder = Main.rand.Next(0, 2);

                Dust d = Dust.NewDustPerfect(thunderPoint, DustID.Electric, null, Scale: 4f);
                d.noGravity = true;
                thunderDusts.Add(d);
            }

            thunderDustsTime = 0;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Player.HasBuff(HeightenedSensesBuff.TypeIndex))
            {
                Player.ClearBuff(HeightenedSensesBuff.TypeIndex);
            }
            else if (Player.HasBuff(CriticalZenithBuff.TypeIndex))
            {
                var finalDamage = modifiers.FinalDamage;

                finalDamage.Flat = target.lifeMax * CriticalZenithBuff.MeleeCriticalFactor;

                modifiers = modifiers with
                {
                    FinalDamage = finalDamage
                };
                CriticalZenithEffects(target.Center);
            }
        }
        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            //if (Player.HasBuff(HeightenedSensesBuff.TypeIndex))
            //{
            //    Player.ClearBuff(HeightenedSensesBuff.TypeIndex);
            //}
            //else if (Player.HasBuff(CriticalZenithBuff.TypeIndex))
            //{
            //    var finalDamage = modifiers.FinalDamage;

            //    finalDamage.Flat = target.lifeMax * CriticalZenithBuff.RangedCriticalFactor;

            //    modifiers = modifiers with
            //    {
            //        FinalDamage = finalDamage
            //    };
            //    CriticalZenithEffects(proj.Center);
            //}
        }

        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            modifiers = modifiers with { IncomingDamageMultiplier = modifiers.IncomingDamageMultiplier * takenDamageMultiplier };

            var heightenedSensesBuffType = ModContent.BuffType<HeightenedSensesBuff>();
            if (Player.HasBuff(heightenedSensesBuffType))
            {
                Player.ClearBuff(heightenedSensesBuffType);
            }
        }
        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            modifiers = modifiers with { IncomingDamageMultiplier = modifiers.IncomingDamageMultiplier * takenDamageMultiplier };

            var heightenedSensesBuffType = ModContent.BuffType<HeightenedSensesBuff>();
            if (Player.HasBuff(heightenedSensesBuffType))
            {
                Player.ClearBuff(heightenedSensesBuffType);
            }
        }

        private float GetRemainingManaPercentage()
        {
            return Player.statMana / Player.statManaMax2;
        }

        int manaVeilParticleDelay = 0;
        public override void PostUpdate()
        {
            base.PostUpdate();

            if (thunderDustsTime < thunderDustsMaxTime)
            {
                thunderDustsTime++;
            }
            else
            {
                foreach (var dust in thunderDusts)
                {
                    dust.active = false;
                }
                thunderDusts.Clear();
            }


            // Check if ManaVeil buff is active (or any other aura-related buff)
            if (
                Player.HasBuff(ModContent.BuffType<ManaVeilBuff>())
                // adding this condition, in case the mod has no particle density configured
                && visualModConfigService.ManaVeilParticleDensity > 0f
                )
            {
                manaVeilParticleDelay++;
                // Random chance to determine if a particle should be created

                /**
                 * <summary>This delay, is calculated using the Lerp algorithm, by using:
                 * - The percentage of the mana that the player has, this will
                 * indicate how often the mana particles should show.
                 * - The amount of max mana the player has, when the player has 200, then the particles will show
                 * much more often.
                 * 
                 * For a player with 100% of their mana, this will show a lot more particles</summary>
                 */

                // then, getting the factor of mana the player has, with 200 as a basis.
                // if a player has more than 200 of mana, then there will be no more visual changes.
                float manaFactor = Player.statMana / (float)visualModConfigService.MaxManaCapParticleDensity;

                // since manaFactor could become bigger than 1.0, then its value will be clamped.
                manaFactor = MathHelper.Clamp(manaFactor, 0f, 1f);

                // Configuration step.
                // In this step, it will use the configuration value of the mod, to determine how much delay this effect
                // must have.
                manaFactor = MathHelper.Lerp(0f, manaFactor, visualModConfigService.ManaVeilParticleDensity);

                // now the final factor is actually:
                // - checking the player's mana potential
                // - the current amount of the player's mana

                // in this case:
                // a player with 200 max mana that has 0, would have a value of 0 in the final factor
                // a player with 100 max mana that has 100, would have a value of 0.5 in the final factor
                // a player with 40 max mana that has 40, would have a value of almost 0.2 or something, final factor
                int delay = (int)MathHelper.Lerp(30, 3, manaFactor);




                // for the size of the mana, it will take into account the max mana of the player.
                float scaleFactor = Player.statManaMax2 / 200f;

                // this clamped value, so in case the player has more than 200, it will not go overboard
                scaleFactor = MathHelper.Clamp(manaFactor, 0f, 1f);

                var finalScale = MathHelper.Lerp(1f, 2f, scaleFactor);

                // adds a final multiplication step, in case one wants to make the particles, bigger or smaller
                finalScale *= visualModConfigService.ManaVeilParticlesSizeScale;


                // now, the condition should work, using major or equal than, because this
                // tick could be skipped, or I'm not sure.
                if (manaVeilParticleDelay >= delay) // 100% chance to create a particle each frame
                {
                    manaVeilParticleDelay = 0;
                    // Get the player's body parts' positions
                    Vector2 playerCenter = Player.Center;

                    // Random angle for particle positioning
                    float angle = Main.rand.NextFloat(0f, MathHelper.TwoPi); // Random angle
                    float radius = Main.rand.NextFloat(0f, 30f); // Random radius for the aura

                    // Randomly choose between body parts to position particles around
                    Vector2 auraPosition;
                    auraPosition = playerCenter + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * radius;

                    // Create a particle at the calculated position

                    Dust d = Dust.NewDustPerfect(auraPosition, DustID.BlueCrystalShard, Vector2.Zero, Scale: finalScale);
                    d.noGravity = true; // Make the particle float and not fall
                    d.alpha = Main.rand.Next(245, 255);
                }
            }
        }


        public override void ResetEffects()
        {
            parryDodge = false;
        }


        public override bool ConsumableDodge(Player.HurtInfo info)
        {
            if (parryDodge)
            {
                ParryDodgeEffects();
                return true;
            }

            return false;
        }

        /**
         * <summary>It indicates how precise the parry was the last time, based on the remaining time of the parry</summary>
         */
        private float GetParryPrecisionFactor()
        {
            var remainingParryBuffTime = GetParryBuffRemainingTime();
            return 1 - (float)remainingParryBuffTime / parryTimeWindow;
        }

        // parryDodgeEffects() will be called from ConsumableDodge and HandleparryDodgeMessage to sync the effect.
        public void ParryDodgeEffects()
        {

            int remainingParryBuffTime = GetParryBuffRemainingTime();

            float precisionFactor = GetParryPrecisionFactor();


            // adds the buff of the heightened senses to the player
            // only if the feature is enabled
            if (
                precisionFactor < 0.33f
                && gameplayModConfigService.CriticalZenithFeatureFlag == true
                )
            {
                Player.AddBuff(ModContent.BuffType<HeightenedSensesBuff>(), heightenedSensesBuffTime, false, false);
            }

            // Calculate the dynamic mana cost
            int manaCost = CalculateParryManaCost(precisionFactor, parryMinimumManaCost, parryMaxManaCost);

            // Deduct mana
            if (manaCost > Player.statManaMax2)
            {
                Player.CheckMana(Player.statMana, true, false);
            }
            else
            {
                Player.CheckMana(manaCost, true, false);
            }


            // now, instead of adding the parry delay time of always, the parry delay time
            // will be determined for how perfect the parry was
            int parryDelayTime = (int)(invulnerabilityTime * 0.85 * 3 * GetParryPrecisionFactor());

            Player.AddBuff(ModContent.BuffType<StrainedReflexesDebuff>(), invulnerabilityTime + parryDelayTime, false, false);
            Player.SetImmuneTimeForAllTypes(invulnerabilityTime);



            // Adjust particle effect size and spread based on precision
            int particleCount = (int)MathHelper.Lerp(400, 50, precisionFactor); // More particles for near-perfect parry
            float particleSpread = MathHelper.Lerp(100f, 2f, precisionFactor); // Smaller spread for near-perfect parry
            float particleScale = MathHelper.Lerp(3f, 1f, precisionFactor); // Larger particles for near-perfect parry


            byte redColored = (byte)MathHelper.Lerp(255, 0, precisionFactor);
            // Some sound and visual effects
            for (int i = 0; i < 50; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                Dust d = Dust.NewDustPerfect(Player.Center + speed * particleSpread, DustID.BlueCrystalShard, speed * 5, Scale: particleScale);
                d.noGravity = true;
                d.color = d.color with { R = redColored };
            }

            float pitch = MathHelper.Lerp(2f, -2f, precisionFactor);
            SoundEngine.PlaySound(SoundID.Item37 with { Pitch = pitch });

            // The visual and sound effects happen on all clients, but the code below only runs for the dodging player 
            if (Player.whoAmI != Main.myPlayer)
            {
                return;
            }

            // Clearing the buff and assigning the cooldown time
            Player.ClearBuff(ModContent.BuffType<ParryBuff>());

            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                SendparryDodgeMessage(Player.whoAmI);
            }
        }

        private int GetParryBuffRemainingTime()
        {
            for (int i = 0; i < Player.buffType.Length; i++)
            {
                if (Player.buffType[i] == ModContent.BuffType<ParryBuff>())
                {
                    return Player.buffTime[i]; // Returns the remaining ticks for the ParryBuff
                }
            }
            return 0; // If the buff is not found, return 0
        }

        public static void HandleParryDodgeMessage(BinaryReader reader, int whoAmI)
        {
            int player = reader.ReadByte();
            if (Main.netMode == NetmodeID.Server)
            {
                player = whoAmI;
            }

            Main.player[player].GetModPlayer<ParryModPlayer>().ParryDodgeEffects();

            if (Main.netMode == NetmodeID.Server)
            {
                // If the server receives this message, it sends it to all other clients to sync the effects.
                SendparryDodgeMessage(player);
            }
        }

        private int CalculateParryManaCost(float precisionFactor, int minimumManaCost, int maxManaCost)
        {
            return (int)MathHelper.Lerp(0, maxManaCost, precisionFactor);
        }

        public static void SendparryDodgeMessage(int whoAmI)
        {
            // This code is called by both the initial 
            ModPacket packet = ModContent.GetInstance<ParryMechanicMod>().GetPacket();
            packet.Write((byte)ParryMechanicMod.MessageType.ParryDodge);
            packet.Write((byte)whoAmI);
            packet.Send(ignoreClient: whoAmI);
        }



    }
}