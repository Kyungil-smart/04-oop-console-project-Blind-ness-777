public class LootResult
{
    public bool FoundSomething { get; private set; }
    public Item Item { get; private set; }

    public LootResult(bool foundSomething, Item item)
    {
        FoundSomething = foundSomething;
        Item = item;
    }
}

public static class LootSystem
{
    // 루팅은 "실패"가 포함되어야 함.
    // 또한 성물(정화용) 보유 수가 많을수록 실패/위험이 약간 증가하도록(압박감)
    public static LootResult RollLoot(EventContext context)
    {
        if (context == null) return new LootResult(false, null);

        int relicCount = context.HolyRelicCount;

        // 실패 확률 기본값
        int failWeight = 50 + (relicCount * 3); // 0개:50 / 5개:65

        // 아이템 가중치
        int drinkWeight = 25;
        int holyWaterWeight = 10;
        int crossWeight = 5;
        int relicWeight = 10;

        // 상황 보정: 정신력이 낮으면 드링크 가중치 살짝 올림
        if (context.Player != null && context.Player.Sanity.Value <= 2)
            drinkWeight += 10;

        // 정화용 성물 슬롯이 이미 꽉 차 있으면 성물은 나오지 않게
        if (context.Player != null && context.Player.Inventory.GetHolyRelicCount() >= 5)
            relicWeight = 0;

        int total = failWeight + drinkWeight + holyWaterWeight + crossWeight + relicWeight;
        int roll = RandomProvider.Next(total);

        if (roll < failWeight)
            return new LootResult(false, null);

        roll -= failWeight;
        if (roll < drinkWeight)
        {
            return new LootResult(true, new Drink(10, "수상한 탄산음료", "마시면 머리가 맑아지는 느낌이다.", 2));
        }

        roll -= drinkWeight;
        if (roll < holyWaterWeight)
        {
            return new LootResult(true, new HolyWater(20, "성수", "한 번, 광기의 손을 떼어낸다."));
        }

        roll -= holyWaterWeight;
        if (roll < crossWeight)
        {
            return new LootResult(true, new Cross(21, "십자가", "한 번, 죽음을 되돌린다."));
        }

        // 정화용 성물
        return new LootResult(true, new HolyRelicPiece(100, "파편 성물", "정화에 필요한 성물 조각이다."));
    }
}
