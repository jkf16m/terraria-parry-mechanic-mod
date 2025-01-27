using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace parry_mechanic.Content.Parry
{
    public class ParryBuffSCMessage : SCMessage<ParryMechanicMod, EmptyData, EmptyData>
    {
        private GameplayModConfigService gameplayModConfigService;
        public ParryBuffSCMessage()
        {
            gameplayModConfigService = Container.Resolve<GameplayModConfigService>();
        }


        public override MessageType Type => MessageType.GiveParryBuff;

        /**
         * <summary>
         * This property indicates whether the OnServer method, should be called in both server and the client.
         * Be aware, that if you override OnClient, then this property would become useless, since this check
         * is done only in the base class, virtual method OnClient.
         * </summary>
         */
        public override bool OnServerAndClientMode => true;


        public override void OnServer(int whoAmI)
        {
            var player = Main.player[whoAmI];
            int parryTimeWindow = gameplayModConfigService.ParryTimeWindowOnTicks;
            int parryMinimumManaCost = gameplayModConfigService.ParryMinimumManaCost;

            if(player.CheckMana(parryMinimumManaCost, false, false)
                && player.HasBuff(ModContent.BuffType<StrainedReflexesDebuff>()) == false
                && player.HasBuff(ModContent.BuffType<ParryBuff>()) == false)
            {
                player.AddBuff(ModContent.BuffType<ManaVeilBuff>(), 99999, true, false);
                player.AddBuff(ModContent.BuffType<ParryBuff>(), parryTimeWindow, true, false);
            }
        }
    }
}
