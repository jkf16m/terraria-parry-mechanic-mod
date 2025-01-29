using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace parry_mechanic.Content.CriticalZenith
{
    public class HeightenedSensesBuff : ModBuff
    {
        public static int TypeIndex => ModContent.BuffType<HeightenedSensesBuff>();
        public static int TicksDuration => 120;
        private List<Dust> particles = new List<Dust>();
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false; // Set to true if it's a debuff
            Main.buffNoSave[Type] = true; // Buff persists when exiting the world
        }

        public override void Update(Player player, ref int buffIndex)
        {
            // Check if the buff is about to expire
            if (player.buffTime[buffIndex] == 0) // Last tick of the buff
            {

                // Handle recovery sound
                player.AddBuff(CriticalZenithBuff.TypeIndex, CriticalZenithBuff.TicksDuration, false, false);
                // small visual particles
                //for (int i = 0; i < 50; i++)
                //{
                //    Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);

                //    var offset = Main.rand.NextFloat(0f, 100f);

                //    Dust d = Dust.NewDustPerfect(player.Center + (speed * 4) * offset, DustID.Electric, null, Scale: 2f);
                //    d.noGravity = true;
                //}
            }
        }
    }
}
