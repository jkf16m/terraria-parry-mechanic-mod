using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.UI.ResourceSets;
using Terraria.ID;
using Terraria.ModLoader;

namespace parry_mechanic.Content
{

    /**
     * Contains all needed logic to implement a CSCMessage.
     * a CSCMessage, handles network interactions, following client-server-clients architecture.
     * 
     * First, the client sends the initial request to the server
     * Then the server handles the packet, makes game validations and performs internal operations
     * Finally, the server rebroadcasts the packet to all clients, if needed, to perform visual-synchronization
     */
    public abstract class SCMessage<ModType, ServerDataType, ClientDataType> where ModType : Mod
    {
        public delegate void Rebroadcast(int toClient = -1, int ignoreClient = -1);
        public abstract MessageType Type { get; }
        public virtual bool OnServerAndClientMode => false;
        




        // first, Fire is called on the server, to broadcast to the clients
        public void Fire(int whoAmI, int toClient = -1, int ignoreClient = -1)
        {
            OnServer(whoAmI);
            if(Main.netMode == NetmodeID.SinglePlayer)
            {
                HandleClientsPacket(Type, whoAmI);
            }
            else
            {
                var packet = ModContent.GetInstance<ModType>().GetPacket();

                BroadcastToClients(toClient, ignoreClient);
            }
        }

        public virtual void OnServer(int whoAmI) {}


        // after that, the server can rebroadcast to all-clients or not, depending whether the OnClients method is implemented, or if it is needed.
        private void BroadcastToClients(int toClient = -1, int ignoreClient = -1)
        {
            var packet = ModContent.GetInstance<ModType>().GetPacket();

            OnBroadcastToClients(packet, toClient, ignoreClient);
        }


        // broadcasting logic to all clients
        public virtual void OnBroadcastToClients(ModPacket packet, int toClient = -1, int ignoreClient = -1)
        {
            packet.Write((byte)Type);
            packet.Send(toClient, ignoreClient);
        }



        // Finally, the client handles the packet

        public void HandleClientsPacket(MessageType type, int whoAmI)
        {
            var result = OnHandleClientPacket(type, whoAmI);
            if (result.Item1)
            {
                OnClients(result.Item2, whoAmI);
            }
        }

        public virtual (bool, ServerDataType) OnHandleClientPacket(MessageType type, int whoAmI)
        {
            if (type == Type)
            {
                return (true, default);
            }
            else
            {
                return (false, default);
            }
        }

        // and executes client-side logic
        public virtual void OnClients(ServerDataType data, int whoAmI) {
            if(OnServerAndClientMode == true && Main.netMode != NetmodeID.SinglePlayer)
            {
                OnServer(Main.myPlayer);
            }
        }
    }
}
