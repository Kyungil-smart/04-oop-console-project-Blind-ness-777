public class Inventory
{
    private const int SlotCount = 6;
    private Item[] _itemSlots = new Item[SlotCount];
    
    public int Count
    {
        get { return SlotCount; }
    }

    public bool TryAdd(Item item)
    {
        if (item == null) return false;
        
        int emptySlotIndex = FindEmptySlotIndex();
        if (emptySlotIndex == -1) return false;

        _itemSlots[emptySlotIndex] = item;
        return true;
    }

    public Item GetItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= SlotCount) return null;
        return _itemSlots[slotIndex];
    }

    public void RemoveAt(int slotIndex)
    {
        if (!IsValidIndex(slotIndex)) return;
        _itemSlots[slotIndex] = null;
    }

    public void UseAt(int slotIndex)
    {
        if (!IsValidIndex(slotIndex)) return false;

        Item item = _itemSlots[slotIndex];
        if (item == null) return false;

        item.Use();
        _itemSlots[slotIndex] = null;
        return true;
    }

    public bool IsFull()
    {
        return FindEmptySlotIndex() == -1;
    }

    public string GetSlotName(int slotIndex)
    {
        if (!IsValidIndex(slotIndex)) return "";
        if (_itemSlots[slotIndex] == null) return "빈칸";

        // Item에 Name 같은 프로퍼티가 있으면 그걸 쓰는 게 베스트
        // 없으면 ToString()을 임시로 사용
        return _itemSlots[slotIndex].ToString();
    }

    private int FindEmptySlotIndex()
    {
        for (int i = 0; i < SlotCount; i++)
        {
            if (_itemSlots[i] == null)
                return i;
        }

        return -1;
    }

    private bool IsValidIndex(int slotIndex)
    {
        return slotIndex >= 0 && slotIndex < SlotCount;
    }
}