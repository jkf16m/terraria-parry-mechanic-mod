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

namespace parry_mechanic
{
    // Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
    public class ParryMechanicMod : Mod
    {
        internal enum MessageType : byte
        {
            ParryDodge,
        }

        private NetworkService networkService;

        

        public override void Load()
        {
            DIService.Register(new ParryModKeybindService(this));
            DIService.Register(ModContent.GetInstance<VisualModConfigService>());
            DIService.Register(ModContent.GetInstance<GameplayModConfigService>());
            networkService = new NetworkService();
            DIService.Register(networkService);
        }
        public override void PostSetupContent()
        {
            networkService.PostSetupContent();
        }

        public override void Unload()
        {
            DIService.Clear();
        }


        // Override this method to handle network packets sent for this mod.
        //TODO: Introduce OOP packets into tML, to avoid this god-class level hardcode.
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            networkService.HandlePacket(reader, whoAmI);



            MessageType msgType = (MessageType)reader.ReadByte();

            switch (msgType)
            {
                case MessageType.ParryDodge:
                    ParryModPlayer.HandleParryDodgeMessage(reader, whoAmI);
                    break;
                default:
                    Logger.WarnFormat("Parry Mechanic: Unknown Message type: {0}", msgType);
                    break;
            }
        }
    }
}
