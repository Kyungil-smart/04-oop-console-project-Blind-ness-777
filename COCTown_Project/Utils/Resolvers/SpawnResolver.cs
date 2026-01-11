using System;

public static class SpawnResolver
{
    public static bool TryFind(string[] template, char symbol, out Vector spawn)
    {
        spawn = new Vector(0, 0);
        if (template == null) return false;

        for (int y = 0; y < template.Length; y++)
        {
            string line = template[y];
            if (line == null) continue;

            for (int x = 0; x < line.Length; x++)
            {
                if (line[x] == symbol)
                {
                    spawn = new Vector(x, y);
                    return true;
                }
            }
        }

        return false;
    }
}