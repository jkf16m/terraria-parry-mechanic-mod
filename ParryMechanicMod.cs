using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using parry_mechanic.Content;
using parry_mechanic.Content.Parry;
using parry_mechanic.Content.Network;
using Microsoft.Extensions.DependencyInjection;
using Terraria.ModLoader.IO;
using log4net;

namespace parry_mechanic
{
    // Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
    public class ParryMechanicMod : Mod
    {

        public override void Load()
        {
            Container.Initialize(this);
        }
        public override void Unload()
        {
            Container.Clear();
        }


        // Override this method to handle network packets sent for this mod.
        //TODO: Introduce OOP packets into tML, to avoid this god-class level hardcode.
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            var type = (MessageType)reader.ReadByte();
            if (Main.netMode == NetmodeID.Server)
            {
                MessageCollection.HandleCSCServerPackets(type, whoAmI);
            }else if(Main.netMode == NetmodeID.MultiplayerClient)
            {
                if(MessageCollection.HandleSCClientPackets(type, whoAmI) == true)
                {
                    return;
                }
                
                if (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    int originalSenderId = (int)reader.ReadByte();
                    MessageCollection.HandleCSCClientPackets(type, originalSenderId);
                }
            }
            //MessageType msgType = (MessageType)type;

            //switch (msgType)
            //{
            //    case MessageType.ParryDodge:
            //        ParryModPlayer.HandleParryDodgeMessage(reader, whoAmI);
            //        break;
            //    default:
            //        Logger.WarnFormat("Parry Mechanic: Unknown Message type: {0}", msgType);
            //        break;
            //}
        }
    }
}
