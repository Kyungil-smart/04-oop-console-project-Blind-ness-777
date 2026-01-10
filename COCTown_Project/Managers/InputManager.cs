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
        ConsoleKey.B,

        // 단축키(일반 아이템 1~6)
        ConsoleKey.D1,
        ConsoleKey.D2,
        ConsoleKey.D3,
        ConsoleKey.D4,
        ConsoleKey.D5,
        ConsoleKey.D6,

        // 넘패드도 허용
        ConsoleKey.NumPad1,
        ConsoleKey.NumPad2,
        ConsoleKey.NumPad3,
        ConsoleKey.NumPad4,
        ConsoleKey.NumPad5,
        ConsoleKey.NumPad6,
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
