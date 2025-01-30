using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace parry_mechanic.Content.Network
{
    public class NetworkService
    {
        public Dictionary<string, Action<BinaryReader, int>> packetDefinitions { get; private set; } = new Dictionary<string, Action<BinaryReader, int>>();
        public Dictionary<byte, Action<BinaryReader, int>> finalDefinitions { get; private set; } = new Dictionary<byte, Action<BinaryReader, int>>();
        public bool Ready { get; private set; } = false;
        public NetworkService() {}

        public void HandlePacket(BinaryReader reader, int whoAmI)
        {
            if (Ready == false) return;

            var nextByte = reader.ReadByte();
            if (finalDefinitions.ContainsKey(nextByte) == false)
            {
                return;
            }

            finalDefinitions[nextByte].Invoke(reader, whoAmI);
        }

        public void PostSetupContent()
        {
            var orderedPacketDefinitions = packetDefinitions.OrderBy(q => q.Key).ToList();

            byte key = 0x00;
            foreach (var packetDefinition in orderedPacketDefinitions)
            {
                finalDefinitions[key] = packetDefinition.Value;
                key++;
            }

            Ready = true;
        }

        public void RegisterClientToServerAction(string name, Action<BinaryReader, int> handler)
        {
            packetDefinitions[name] = handler;
        }
    }
}
