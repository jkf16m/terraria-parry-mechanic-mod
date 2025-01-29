using System;
using System.ComponentModel;
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

        [SeparatePage]
        public int DummyPage = 0;


        [Range(0, 255)]
        [Increment(1)]
        [Slider]
        [SliderColor(255, 0, 0)]
        public int CriticalZenithPrimaryColorRed = 255;

        [Range(0, 255)]
        [Increment(1)]
        [Slider]
        [SliderColor(0, 255, 0)]
        public int CriticalZenithPrimaryColorGreen = 0;

        [Range(0, 255)]
        [Increment(1)]
        [Slider]
        [SliderColor(0, 0, 255)]
        public int CriticalZenithPrimaryColorBlue = 0;

        [Range(0, 255)]
        [Increment(1)]
        [Slider]
        [SliderColor(255, 0, 0)]
        public int CriticalZenithSecondaryColorRed = 255;

        [Range(0, 255)]
        [Increment(1)]
        [Slider]
        [SliderColor(0, 255, 0)]
        public int CriticalZenithSecondaryColorGreen = 0;

        [Range(0, 255)]
        [Increment(1)]
        [Slider]
        [SliderColor(0, 0, 255)]
        public int CriticalZenithSecondaryColorBlue = 0;
    }
}
