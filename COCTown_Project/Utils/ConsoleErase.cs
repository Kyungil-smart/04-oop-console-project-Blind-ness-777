using System;

public static class ConsoleErase
{
    public static void ClearLinesFrom(int startY)
    {
        int width = Console.WindowWidth;
        int height = Console.WindowHeight;

        if (startY < 0) startY = 0;
        if (startY >= height) return;

        string blank = new string(' ', width);

        for (int y = startY; y < height; y++)
        {
            Console.SetCursorPosition(0, y);
            Console.Write(blank);
        }
    }
}