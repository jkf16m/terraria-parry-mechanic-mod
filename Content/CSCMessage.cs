using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.UI.ResourceSets;
using Terraria.ID;
using Terraria.ModLoader;

namespace parry_mechanic.Content
{

    public class EmptyData { }
    /**
     * Contains all needed logic to implement a CSCMessage.
     * a CSCMessage, handles network interactions, following client-server-clients architecture.
     * 
     * First, the client sends the initial request to the server
     * Then the server handles the packet, makes game validations and performs internal operations
     * Finally, the server rebroadcasts the packet to all clients, if needed, to perform visual-synchronization
     */
    public abstract class CSCMessage<ModType, ServerDataType, ClientDataType> where ModType : Mod
    {
        public delegate void Rebroadcast(int toClient = -1, int ignoreClient = -1);
        public abstract MessageType Type { get; }
        public virtual bool OnServerAndClientMode => false;
        




        // first, Fire is called on the client, to broadcast to the server
        // in case this is not multiplayer, then GetPacket will not execute, and instead, redirect the call to HandleServerPacket
        public void Fire()
        {
            if(Main.netMode == NetmodeID.SinglePlayer)
            {
                HandleServerPacket(Type, Main.myPlayer);
            }
            else
            {
                var packet = ModContent.GetInstance<ModType>().GetPacket();

                OnBroadcastToServer(packet);
            }
        }

        // The OnBroadcastToServer is executed
        public virtual void OnBroadcastToServer(ModPacket packet)
        {
            packet.Write((byte)Type);
            packet.Send();
        }

        // then, the server handles the packet
        // simultaneously, the server also receives as a parameter, the action to re-broadcast to the clients
        public void HandleServerPacket(MessageType type, int whoAmI)
        {
            var result = OnHandleServerPacket(type, whoAmI);
            if (result.Item1)
            {

                // in case this is a single player game, then the rebroadcast runs directly the client side logic instead
                if(Main.netMode == NetmodeID.SinglePlayer)
                {
                    OnServer(result.Item2, whoAmI, rebroadcast: SinglePlayerRebroadcast(whoAmI));
                }
                else
                {
                    OnServer(result.Item2, whoAmI, rebroadcast: BroadcastToClients(whoAmI));
                }
            }
        }


        public virtual (bool, ClientDataType) OnHandleServerPacket(MessageType type, int whoAmI)
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


        // and executes server-side logic
        public virtual void OnServer(ClientDataType data, int whoAmI, Rebroadcast rebroadcast) { }



        private Rebroadcast SinglePlayerRebroadcast(int whoAmI)
        {
            return (toClient, ignoreClient) => HandleClientsPacket(Type, whoAmI);
        }


        // after that, the server can rebroadcast to all-clients or not, depending whether the OnClients method is implemented, or if it is needed.
        private Rebroadcast BroadcastToClients(int whoAmI)
        {
            return (toClient, ignoreClient) =>
            {
                var packet = ModContent.GetInstance<ModType>().GetPacket();

                OnBroadcastToClients(packet, whoAmI, toClient, ignoreClient);
            };
        }


        // broadcasting logic to all clients
        public virtual void OnBroadcastToClients(ModPacket packet, int whoAmI, int toClient = -1, int ignoreClient = -1)
        {
            packet.Write((byte)Type);
            packet.Write((byte)whoAmI);
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
        public virtual void OnClients(ServerDataType data, int whoAmI) { }
    }
}
