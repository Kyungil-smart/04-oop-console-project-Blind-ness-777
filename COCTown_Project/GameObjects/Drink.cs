using System;

public class Drink : Item
{
    private int _recoverAmount;

    public Drink(int id, string name, string description, int recoverAmount)
    {
        Id = id;
        Name = name;
        Description = description;
        _recoverAmount = recoverAmount;
        Category = ItemCategory.Normal;
    }

    public override void Use()
    {
        if (Inventory == null || Inventory.Owner == null)
        {
            Console.Clear();
            Console.WriteLine(Name + " 을(를) 사용할 수 없다.");
            Console.WriteLine("[Enter] 계속");
            Console.ReadLine();
            return;
        }

        Inventory.Owner.IncreaseSanity(_recoverAmount);

        Console.Clear();
        Console.WriteLine(Name + " 을(를) 마셨다.");
        Console.WriteLine("정신력이 " + _recoverAmount + " 회복되었다.");
        Console.WriteLine("[Enter] 계속");
        Console.ReadLine();
    }

    public override string ToString()
    {
        return Name;
    }
}
