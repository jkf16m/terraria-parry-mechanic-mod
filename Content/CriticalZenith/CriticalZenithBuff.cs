using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace parry_mechanic.Content.CriticalZenith
{
    public class CriticalZenithBuff : ModBuff
    {
        public static int TypeIndex => ModContent.BuffType<CriticalZenithBuff>();
        public static int TicksDuration => 30;
        public static float MeleeCriticalFactor => 0.55f;
        public static float RangedCriticalFactor => 0.33f;
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false; // Set to true if it's a debuff
            Main.buffNoSave[Type] = true; // Buff persists when exiting the world
        }

        private int delay = 0;
        public override void Update(Player player, ref int buffIndex)
        {
            // Check if the buff is not expired
            delay += 1;
            if (player.buffTime[buffIndex] > 0 && delay >= 3)
            {
                delay = 0;
                Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);

                var offset = Main.rand.NextFloat(0f, 10f);

                Dust d = Dust.NewDustPerfect(player.Center + speed * 4 * offset, DustID.Electric, null, Scale: 2f);
                d.noGravity = true;
                d.color = Color.Yellow;

                var offset2 = Main.rand.NextFloat(0f, 10f);

                Dust d2 = Dust.NewDustPerfect(player.Center + speed * 4 * offset2, DustID.DarkCelestial, null, Scale: 2f);
                d2.noGravity = true;
                d2.color = Color.Black;
            }
        }
    }
}
