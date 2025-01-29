using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace parry_mechanic.Content.CriticalZenith
{
    public class ThunderBoltSystem : ModSystem
    {
        private ThunderBoltService thunderBoltService;
        public override void PostSetupContent()
        {
            thunderBoltService = Container.Resolve<ThunderBoltService>();
        }
        public override void PreUpdateWorld()
        {
            thunderBoltService.Update();
        }
        public override void PostDrawTiles()
        {
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            thunderBoltService.DrawAll(Main.spriteBatch);
            Main.spriteBatch.End();
        }
    }
}
