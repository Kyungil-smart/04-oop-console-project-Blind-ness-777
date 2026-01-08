using System;

public static class InputManager
{
    private static ConsoleKey _current;
    private static bool _hasKey;

    private static readonly ConsoleKey[] _keys =
    {
        ConsoleKey.UpArrow,
        ConsoleKey.DownArrow,
        ConsoleKey.LeftArrow,
        ConsoleKey.RightArrow,
        ConsoleKey.Enter,
        ConsoleKey.I,
        ConsoleKey.Z,
        ConsoleKey.B,
    };

    public static void Poll()
    {
        _hasKey = false;
        _current = ConsoleKey.Clear;

        if (!Console.KeyAvailable) return;

        ConsoleKey input = Console.ReadKey(true).Key;

        for (int i = 0; i < _keys.Length; i++)
        {
            if (_keys[i] == input)
            {
                _current = input;
                _hasKey = true;
                break;
            }
        }
    }

    public static bool GetKey(ConsoleKey key)
    {
        if (!_hasKey) return false;
        return _current == key;
    }
}
