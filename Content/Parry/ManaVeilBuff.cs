using Terraria;
using Terraria.ModLoader;

namespace parry_mechanic.Content.Parry
{
    class ManaVeilBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false; // Set to true if it's a debuff
            Main.buffNoSave[Type] = true; // Buff persists when exiting the world
            Main.buffNoTimeDisplay[Type] = true;
        }
    }
}