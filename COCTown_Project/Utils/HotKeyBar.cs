using System;

public class HotkeyBar
{
    private Inventory _inventory;

    public HotkeyBar(Inventory inventory)
    {
        _inventory = inventory;
    }

    public void Render(int startX, int startY)
    {
        Console.SetCursorPosition(startX, startY);

        for (int i = 0; i < _inventory.Count; i++)
        {
            string name = _inventory.GetSlotName(i);
            Console.Write("[" + (i + 1) + ":" + name + "] ");
        }
    }
}