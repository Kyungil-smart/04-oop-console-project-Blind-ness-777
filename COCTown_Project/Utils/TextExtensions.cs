using System;

public static class TextExtensions
{
    public static void Print(this string text)
    {
        Console.Write(text);
    }

    public static void Print(this char character)
    {
        Console.Write(character);
    }

    public static void Print(this string text, ConsoleColor color)
    {
        var prev = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.Write(text);
        Console.ForegroundColor = prev;
    }
}