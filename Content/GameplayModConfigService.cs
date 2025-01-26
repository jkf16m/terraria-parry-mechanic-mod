using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace parry_mechanic.Content
{
    public class GameplayModConfigService : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [DefaultValue(false)]
        public bool CriticalZenithFeatureFlag = false;
    }
}
