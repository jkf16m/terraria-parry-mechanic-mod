using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace parry_mechanic.Content.Parry
{
    class ParryBuff : ModBuff
    {
        public static int TypeIndex => ModContent.BuffType<ParryBuff>();
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false; // Set to true if it's a debuff
            Main.buffNoSave[Type] = true; // Buff persists when exiting the world
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<ParryModPlayer>().parryDodge = true;

            // Check if the buff is about to expire
            if (player.buffTime[buffIndex] == 0) // Last tick of the buff
            {
                // Handle missed parry logic
                player.AddBuff(ModContent.BuffType<StrainedReflexesDebuff>(), 50); // Apply a penalty buff
                // You can also trigger other actions here, such as sound effects or visual cues
            }
        }
    }
}