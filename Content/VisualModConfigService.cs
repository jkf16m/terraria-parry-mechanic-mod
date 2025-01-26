using System;
using System.ComponentModel;
using System.Configuration;
using Terraria.ModLoader.Config;

namespace parry_mechanic.Content
{
    public class VisualModConfigService : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Range(0f, 1f)]
        [Slider]
        [Increment(0.1f)]
        [DefaultValue(1f)]
        public float ManaVeilParticleDensity = 1f;

        [Range(0, 5000)]
        [Increment(20)]
        [DefaultValue(200)]
        public int MaxManaCapParticleDensity = 200;

        [Range(0f, 3f)]
        [Increment(0.5f)]
        [Slider]
        [DefaultValue(1f)]
        public float ManaVeilParticlesSizeScale = 1f;
    }
}
