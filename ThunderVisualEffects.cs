using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework; // Assuming XNA/MonoGame for Vector2

public static class ThunderVisualEffects
{
    public static Vector2[] GenerateThunderPoints(Vector2 origin, int originBifurcationQuantity, float size, int volume, float randomness)
    {
        // Step 1: Generate the tree of points
        var tree = GenerateTree(origin, originBifurcationQuantity, size);

        // Step 2: Generate interpolated points with volume and randomness
        var interpolatedPoints = InterpolateTree(tree, volume, randomness);

        // Step 3: Flatten the points into a single list
        var result = FlattenTree(interpolatedPoints);

        return result.ToArray();
    }

    private static Dictionary<Vector2, List<Vector2>> GenerateTree(Vector2 origin, int bifurcationQuantity, float size)
    {
        var tree = new Dictionary<Vector2, List<Vector2>>();
        var queue = new Queue<(Vector2, int)>();
        queue.Enqueue((origin, bifurcationQuantity));

        var random = new Random();

        while (queue.Count > 0)
        {
            var (currentPoint, bifurcations) = queue.Dequeue();

            if (!tree.ContainsKey(currentPoint))
                tree[currentPoint] = new List<Vector2>();

            for (int i = 0; i < bifurcations; i++)
            {
                // Generate a random direction and distance for the child point
                float angle = (float)(random.NextDouble() * Math.PI * 2);
                float distance = (float)(random.NextDouble() * size);
                Vector2 childPoint = currentPoint + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * distance;

                tree[currentPoint].Add(childPoint);

                // Recursively add bifurcations to the child point with reduced quantity
                if (bifurcations > 1)
                {
                    queue.Enqueue((childPoint, bifurcations - 1));
                }
            }
        }

        return tree;
    }

    private static List<(Vector2, Vector2)> InterpolateTree(Dictionary<Vector2, List<Vector2>> tree, int volume, float randomness)
    {
        var interpolatedSegments = new List<(Vector2, Vector2)>();
        var random = new Random();

        foreach (var kvp in tree)
        {
            Vector2 parent = kvp.Key;

            foreach (var child in kvp.Value)
            {
                // Generate points along the line from parent to child
                for (int i = 0; i < volume; i++)
                {
                    float t = i / (float)(volume - 1); // Normalize t between 0 and 1
                    Vector2 interpolatedPoint = Vector2.Lerp(parent, child, t);

                    // Apply randomness to deviate from the main line
                    float deviationX = (float)(random.NextDouble() - 0.5) * 2 * randomness;
                    float deviationY = (float)(random.NextDouble() - 0.5) * 2 * randomness;
                    interpolatedPoint += new Vector2(deviationX, deviationY);

                    interpolatedSegments.Add((interpolatedPoint, parent));
                }
            }
        }

        return interpolatedSegments;
    }

    private static List<Vector2> FlattenTree(List<(Vector2, Vector2)> interpolatedSegments)
    {
        var allPoints = new HashSet<Vector2>();

        foreach (var (point, _) in interpolatedSegments)
        {
            allPoints.Add(point);
        }

        return new List<Vector2>(allPoints);
    }
}
