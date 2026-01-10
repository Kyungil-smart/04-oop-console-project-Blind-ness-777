public abstract class Item : GameObject
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

	// 아이템 분류 (인벤토리 보관/스왑/성물(퀘스트) 슬롯 등에 사용)
	public ItemCategory Category { get; protected set; }

    public Inventory Inventory { get; set; }
    public bool InInventory { get => Inventory != null; }

    public abstract void Use();

    public void PrintInfo()
    {
    }
}