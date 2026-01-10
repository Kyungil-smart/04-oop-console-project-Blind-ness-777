using System;

// "성물(퀘스트용)" : 5개를 모아 성당 지하에서 정화하는 목적 아이템
public class HolyRelicPiece : Item
{
    public HolyRelicPiece(int id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
        Category = ItemCategory.HolyRelic;
    }

    public override void Use()
    {
        Console.Clear();
        Console.WriteLine(Name + " 은(는) 지금 사용할 수 없다.");
        Console.WriteLine("성당에서 정화할 때 필요하다.");
        Console.WriteLine("[Enter] 계속");
        Console.ReadLine();
    }

    public override string ToString()
    {
        return Name;
    }
}
