using System;
using System.Collections.Generic;

public class Inventory
{
    private const int SlotCount = 6;
    private const int HolyRelicSlotCount = 5;
    private Item[] _itemSlots = new Item[SlotCount];

    private HolyRelicPiece[] _holyRelicSlots = new HolyRelicPiece[HolyRelicSlotCount];

    private List<Item> _keyItems = new List<Item>();


    public PlayerCharacter Owner { get; set; }

    public int Count
    {
        get { return SlotCount; }
    }

    public bool TryAdd(Item item)
    {
        if (item == null) return false;

        item.Inventory = this;

        // 열쇠만 별도 보관
        if (item.Category == ItemCategory.Key || IsKeyItem(item))
        {
            _keyItems.Add(item);
            return true;
        }

        // ✅ 퀘스트용 성물(5개 모으는 것)은 별도 슬롯
        if (item.Category == ItemCategory.HolyRelic)
        {
            HolyRelicPiece relic = item as HolyRelicPiece;
            if (relic == null) return false;

            int emptyRelicIndex = FindEmptyHolyRelicSlotIndex();
            if (emptyRelicIndex == -1)
                return false;

            _holyRelicSlots[emptyRelicIndex] = relic;
            return true;
        }

        int emptySlotIndex = FindEmptySlotIndex();
        if (emptySlotIndex != -1)
        {
            _itemSlots[emptySlotIndex] = item;
            return true;
        }

        return SwapWhenFull(item);
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

    public bool UseAt(int slotIndex)
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

    public void ShowInventoryScreen()
    {
        while (true)
        {
            Console.Clear();
            PrintInventoryCard();

            Console.WriteLine();
            Console.WriteLine("사용할 슬롯 번호 (1~6), Enter = 닫기");

            string input = Console.ReadLine();
            if (string.IsNullOrEmpty(input))
                break;

            int number;
            if (int.TryParse(input, out number))
            {
                int index = number - 1;
                if (UseAt(index))
                {
                    Console.WriteLine("아이템을 사용했다.");
                }
                else
                {
                    Console.WriteLine("사용할 수 없다.");
                }
                Console.WriteLine("[Enter] 계속");
                Console.ReadLine();
            }
        }
    }

    private void PrintInventoryCard()
    {
        Console.WriteLine("┌────────────────────────────────────────┐");
        Console.WriteLine("│               소 지 품                 │");
        Console.WriteLine("├────────────────────────────────────────┤");

        Console.WriteLine("│ [일반(6칸)]                            │");
        for (int i = 0; i < SlotCount; i++)
        {
            string itemName = (_itemSlots[i] == null) ? "(비어있음)" : _itemSlots[i].ToString();

            string line = (i + 1) + ". " + itemName;
            Console.WriteLine("│ " + PadRightTo(line, 36) + "│");
        }

        Console.WriteLine("├────────────────────────────────────────┤");
        Console.WriteLine("│ [열쇠]                                 │");
        Console.WriteLine("│  " + PadRightTo(JoinNames(_keyItems), 36) + "│");

        Console.WriteLine("├────────────────────────────────────────┤");
        Console.WriteLine("│ [성물(정화용) 5칸]                      │");
        Console.WriteLine("│  " + PadRightTo(JoinHolyRelicNames(), 36) + "│");

        Console.WriteLine("└────────────────────────────────────────┘");
        Console.WriteLine("          [Enter] 닫기");
    }


    private string PadRightTo(string text, int width)
    {
        if (text == null) text = "";
        if (text.Length >= width) return text.Substring(0, width);
        return text + new string(' ', width - text.Length);
    }

    private string JoinNames(List<Item> items)
    {
        if (items == null || items.Count == 0) return "(없음)";

        string result = "";
        for (int i = 0; i < items.Count; i++)
        {
            if (i > 0) result += ", ";
            result += items[i] == null ? "" : items[i].ToString();
        }
        return result;
    }

    private bool SwapWhenFull(Item newItem)
    {
        Console.Clear();
        Console.WriteLine("가방이 가득 찼다. 무엇을 버릴까?");
        Console.WriteLine("새로 얻을 아이템: " + newItem);

        List<int> swappable = new List<int>();

        for (int i = 0; i < SlotCount; i++)
        {
            Item item = _itemSlots[i];

            if (item != null && IsSwappable(item)) // 보호 아이템/성물은 제외
            {
                swappable.Add(i);
                Console.WriteLine((i + 1) + ". " + item);
            }
            else
            {
                Console.WriteLine((i + 1) + ". [보호됨]");
            }
        }

        if (swappable.Count == 0)
        {
            Console.WriteLine("버릴 수 있는 아이템이 없다.");
            Console.ReadLine();
            return false;
        }

        Console.Write("버릴 슬롯 번호: ");
        int index = ReadSlotIndex(swappable);

        _itemSlots[index] = newItem;
        return true;
    }

    private int ReadSlotIndex(List<int> allowed)
    {
        while (true)
        {
            int n;
            if (int.TryParse(Console.ReadLine(), out n))
            {
                int idx = n - 1;
                if (allowed.Contains(idx))
                    return idx;
            }
            Console.Write("잘못된 입력: ");
        }
    }

    // ✅ 임시 판별(지금 Item에 타입이 없으니 이름 기반)
    // 나중에 ItemType이 생기면 여기만 갈아끼우면 됨.
    private bool IsKeyItem(Item item)
    {
        if (item == null) return false;
        string name = item.ToString();
        return name.Contains("열쇠") || name.Contains("키");
    }

    private bool IsSwappable(Item item)
    {
        if (item == null) return false;

        // 일반 아이템만 교체 가능
        return item.Category == ItemCategory.Normal;
    }

    public int GetHolyRelicCount()
    {
        int count = 0;
        for (int i = 0; i < HolyRelicSlotCount; i++)
        {
            if (_holyRelicSlots[i] != null) count++;
        }
        return count;
    }

    public bool TryConsumeHolyWater()
    {
        return TryConsumeFromItemSlots(typeof(HolyWater));
    }

    public bool TryConsumeCross()
    {
        return TryConsumeFromItemSlots(typeof(Cross));
    }

    private bool TryConsumeFromItemSlots(Type targetType)
    {
        for (int i = 0; i < SlotCount; i++)
        {
            Item item = _itemSlots[i];
            if (item != null && item.GetType() == targetType)
            {
                _itemSlots[i] = null;
                return true;
            }
        }
        return false;
    }

    private int FindEmptyHolyRelicSlotIndex()
    {
        for (int i = 0; i < HolyRelicSlotCount; i++)
        {
            if (_holyRelicSlots[i] == null)
                return i;
        }
        return -1;
    }

    private string JoinHolyRelicNames()
    {
        int count = GetHolyRelicCount();
        if (count == 0) return "(없음)";

        string result = "";
        for (int i = 0; i < HolyRelicSlotCount; i++)
        {
            if (_holyRelicSlots[i] == null) continue;

            if (result.Length > 0) result += ", ";
            result += _holyRelicSlots[i].ToString();
        }
        return result;
    }
}