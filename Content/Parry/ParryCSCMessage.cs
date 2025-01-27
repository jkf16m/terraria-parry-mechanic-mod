using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
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
    }
}
