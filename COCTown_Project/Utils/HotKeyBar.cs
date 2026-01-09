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
        Console.SetCursorPosition(startX, startY);

        for (int i = 0; i < _inventory.Count; i++)
        {
            string name = _inventory.GetSlotName(i);
            Console.Write("[" + (i + 1) + ":" + name + "] ");
        }
    }
}