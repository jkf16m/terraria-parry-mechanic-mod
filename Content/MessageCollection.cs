using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using parry_mechanic.Content.Parry;

namespace parry_mechanic.Content
{
    /**
     * <summary>This class has all the defined SyncActions.</summary>
     */
    public static class MessageCollection
    {
        public static Dictionary<MessageType, CSCMessage<ParryMechanicMod, EmptyData, EmptyData>> CSCMessages = new Dictionary<MessageType, CSCMessage<ParryMechanicMod, EmptyData, EmptyData>>() {
            { MessageType.Parry, new ParryCSCMessage() }
        };
        public static Dictionary<MessageType, SCMessage<ParryMechanicMod, EmptyData, EmptyData>> SCMessages = new Dictionary<MessageType, SCMessage<ParryMechanicMod, EmptyData, EmptyData>>() {
            { MessageType.GiveParryBuff, new ParryBuffSCMessage() }
        };

        public static void HandleCSCServerPackets(MessageType type, int whoAmI)
        {
            CSCMessages[type].HandleServerPacket(type, whoAmI);
        }

        public static bool HandleCSCClientPackets(MessageType type, int whoAmI)
        {
            if(CSCMessages.ContainsKey(type) == false)
            {
                return false;
            }
            CSCMessages[type].HandleClientsPacket(type, whoAmI);

            return true;
        }

        public static bool HandleSCClientPackets(MessageType type, int whoAmI)
        {
            if(SCMessages.ContainsKey(type) == false)
            {
                return false;
            }
            SCMessages[type].HandleClientsPacket(type, whoAmI);

            return true;
        }
    }
}
