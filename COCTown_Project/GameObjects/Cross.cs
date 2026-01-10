using System;

public class Cross : Item
{
    public Cross(int id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
        Category = ItemCategory.Protection;
    }

    public override void Use()
    {
        Console.Clear();
        Console.WriteLine(Name + "은(는) 죽음을 막는 순간에 자동으로 발동된다.");
        Console.WriteLine("[Enter] 계속");
        Console.ReadLine();
    }

    public override string ToString()
    {
        return Name;
    }
}