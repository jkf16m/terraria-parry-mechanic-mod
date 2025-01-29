using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace parry_mechanic.Content.CriticalZenith
{
    public class ThunderBoltService
    {
        private class ThunderBolt
        {
            public Vector2 Start { get; }
            public Vector2 End { get; }
            public Color Color { get; }
            public float Thickness { get; }
            public int Lifetime { get; private set; }

            public ThunderBolt(Vector2 start, Vector2 end, Color color, float thickness, int lifetime)
            {
                Start = start;
                End = end;
                Color = color;
                Thickness = thickness;
                Lifetime = lifetime;
            }

            public void Update()
            {
                Lifetime--;
            }

            public bool IsExpired => Lifetime <= 0;
        }

        private readonly List<ThunderBolt> thunderBolts = new();

        public void AddLightning(Vector2 start, Vector2 end, Color color, float thickness, int lifetime = 10)
        {
            thunderBolts.Add(new ThunderBolt(start, end, color, thickness, lifetime));
        }

        public void AddRandomLightningFromOrigin(Vector2 origin, int bifurcations, float size, Color color, float thickness, int lifetime = 10)
        {
            Vector2 direction = new Vector2(Main.rand.NextFloat(-1f, 1f), 1).SafeNormalize(Vector2.UnitY);
            Vector2 end = origin + direction * size;

            AddLightning(origin, end, color, thickness, lifetime);

            GenerateBifurcations(origin, end, bifurcations, size * 0.5f, color, thickness * 0.8f, lifetime - 2);
        }

        private void GenerateBifurcations(Vector2 start, Vector2 end, int bifurcations, float size, Color color, float thickness, int lifetime)
        {
            if (bifurcations <= 0 || size < 10f) return;

            for (int i = 0; i < bifurcations; i++)
            {
                Vector2 midPoint = Vector2.Lerp(start, end, Main.rand.NextFloat(0.4f, 0.8f));
                Vector2 offset = new Vector2(Main.rand.NextFloat(-size, size), Main.rand.NextFloat(-size, size));
                Vector2 newEnd = midPoint + offset;

                AddLightning(midPoint, newEnd, color, thickness, lifetime);

                GenerateBifurcations(midPoint, newEnd, bifurcations - 1, size * 0.5f, color, thickness * 0.7f, lifetime - 1);
            }
        }

        public void DrawAll(SpriteBatch spriteBatch)
        {
            for (int i = thunderBolts.Count - 1; i >= 0; i--)
            {
                var bolt = thunderBolts[i];

                DrawLightning(spriteBatch, bolt.Start, bolt.End, bolt.Color * (bolt.Lifetime / 10f), bolt.Thickness);

                bolt.Update();
                if (bolt.IsExpired)
                {
                    thunderBolts.RemoveAt(i);
                }
            }
        }

        private void DrawLightning(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, float thickness)
        {
            Texture2D pixel = Terraria.GameContent.TextureAssets.MagicPixel.Value;
            Vector2 direction = end - start;
            float length = direction.Length();
            float rotation = (float)System.Math.Atan2(direction.Y, direction.X);

            spriteBatch.Draw(pixel, start, null, color, rotation, Vector2.Zero, new Vector2(length, thickness), SpriteEffects.None, 0f);
        }

        public void Update()
        {
            for (int i = thunderBolts.Count - 1; i >= 0; i--)
            {
                thunderBolts[i].Update();
                if (thunderBolts[i].Lifetime <= 0)
                {
                    thunderBolts.RemoveAt(i);
                }
            }
        }
    }
}