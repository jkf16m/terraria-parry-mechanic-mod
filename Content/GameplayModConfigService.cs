using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace parry_mechanic.Content
{
    public class GameplayModConfigService : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [DefaultValue(50)]
        [Range(1, 1000)]
        [Increment(1)]
        public int ParryTimeWindowOnTicks = 50;

        [DefaultValue(20)]
        [Increment(20)]
        public int ParryMinimumManaCost = 20;

        [DefaultValue(false)]
        public bool CriticalZenithFeatureFlag = false;
    }
}
