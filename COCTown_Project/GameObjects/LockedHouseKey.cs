using System;

// 잠긴 집(예: House4) 입장에 필요한 열쇠
public class LockedHouseKey : Item
{
	public LockedHouseKey(int id, string name, string description)
	{
		Id = id;
		Name = name;
		Description = description;
		Category = ItemCategory.Key;
	}

	public override void Use()
	{
		Console.Clear();
		Console.WriteLine(Name + " 은(는) 소지하고 있으면 자동으로 사용된다.");
		Console.WriteLine("(직접 사용할 수 없다)");
		Console.WriteLine("[Enter] 계속");
		Console.ReadLine();
	}

	public override string ToString()
	{
		return Name;
	}
}
