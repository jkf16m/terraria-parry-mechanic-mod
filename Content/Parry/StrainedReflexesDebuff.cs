using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace parry_mechanic.Content.Parry
{
    class StrainedReflexesDebuff : ModBuff
    {
        public static int TypeIndex => ModContent.BuffType<StrainedReflexesDebuff>();
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true; // Set to true if it's a debuff
            Main.buffNoSave[Type] = true; // Buff persists when exiting the world
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<ParryModPlayer>().parryDodge = false;

            // Check if the buff is about to expire
            if (player.buffTime[buffIndex] == 0) // Last tick of the buff
            {
                // Handle recovery sound
                SoundEngine.PlaySound(SoundID.MaxMana with { Pitch = -1f, Volume = 0.5f });
                // You can also trigger other actions here, such as sound effects or visual cues
            }
        }
    }
}