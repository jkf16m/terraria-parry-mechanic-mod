using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace parry_mechanic.Content.Parry
{
    /**
     * <summary>
     * Class that handles all of the interactions needed for a successful parry.
     * 
     * Contains all underlying logic for both client and server, to handle a parry
     * </summary>
     */
    public class ParryCSCMessage : CSCMessage<ParryMechanicMod, EmptyData, EmptyData>
    {
        public override MessageType Type => MessageType.Parry;


        public override void OnServer(EmptyData data, int whoAmI, Rebroadcast rebroadcast)
        {
            new ParryBuffSCMessage().Fire(whoAmI);

            rebroadcast();
        }

        public override void OnClients(EmptyData data, int whoAmI)
        {
            var player = Main.player[whoAmI];
            //// small visual particles
            for (int i = 0; i < 20; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                Dust d = Dust.NewDustPerfect(player.Center + speed * 4, DustID.PurpleCrystalShard, speed * 2, Scale: 1f);
                d.noGravity = true;
            }
            SoundEngine.PlaySound(SoundID.MaxMana with { Pitch = 2f, Volume = 0.5f });
        }
    }
}
