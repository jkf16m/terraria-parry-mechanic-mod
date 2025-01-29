using Terraria.ModLoader;

namespace parry_mechanic.Content.Parry
{
    public class ParryModKeybindService
    {
        public ModKeybind ParryKeybind { get; private set; }
        public ParryModKeybindService(Mod mod)
        {
            ParryKeybind = KeybindLoader.RegisterKeybind(mod, "Parry", "Z");
        }
    }
}
