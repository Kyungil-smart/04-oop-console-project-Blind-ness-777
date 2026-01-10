using System;

public class HolyWater : Item
{
    public HolyWater(int id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
        Category = ItemCategory.Protection;
    }

    public override void Use()
    {
        Console.Clear();
        Console.WriteLine(Name + " 은(는) 위기의 순간에 자동으로 사용된다.");
        Console.WriteLine("[Enter] 계속");
        Console.ReadLine();
    }

    public override string ToString()
    {
        return Name;
    }
}