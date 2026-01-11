using System;

public class HotKeyBar
{
    private Inventory _inventory;

    public HotKeyBar(Inventory inventory)
    {
        _inventory = inventory;
    }

    public void Render(int startX, int startY)
    {
        // 콘솔 버퍼가 유효하지 않으면 출력 생략
        if (Console.BufferWidth <= 0 || Console.BufferHeight <= 0)
            return;

        // startX/startY 범위 보정(클램프)
        if (startX < 0) startX = 0;
        if (startY < 0) startY = 0;

        if (startX >= Console.BufferWidth) startX = Console.BufferWidth - 1;
        if (startY >= Console.BufferHeight) startY = Console.BufferHeight - 1;

        Console.SetCursorPosition(startX, startY);

        for (int i = 0; i < _inventory.Count; i++)
        {
            string name = _inventory.GetSlotName(i);
            Console.Write("[" + (i + 1) + ":" + name + "] ");
        }
    }
}