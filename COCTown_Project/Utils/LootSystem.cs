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
    // ✅ 정화용 성물(5개 모으는 성물)은 B안에서 별도 시스템(RelicPlacementSystem)으로만 지급한다.
    // 이 LootSystem에서는 일반 아이템만 뽑는다.
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

        int total = failWeight + drinkWeight + holyWaterWeight + crossWeight;
        int roll = RandomProvider.Next(0, total);

        if (roll < failWeight)
            return new LootResult(false, null);

        roll -= failWeight;
        if (roll < drinkWeight)
        {
            return new LootResult(true, new Drink(10, "드링크", "마시면 정신이 조금 돌아온다.", 1));
        }

        roll -= drinkWeight;
        if (roll < holyWaterWeight)
        {
            return new LootResult(true, new HolyWater(20, "성수", "한 번, 광기의 손을 떼어낸다."));
        }

        // cross
        return new LootResult(true, new Cross(21, "십자가", "한 번, 죽음을 되돌린다."));
    }

	// 부서진 집 전용 루팅 테이블(하이리스크/하이리턴)
	// - 실패는 줄이고
	// - 성수/십자가 같은 "생존용" 아이템 확률을 높인다.
	public static LootResult RollBrokenHouseLoot(EventContext context)
	{
		if (context == null) return new LootResult(false, null);

		// 부서진 집에서는 운이 더 좋다(하지만 실패는 여전히 존재)
		int failWeight = 25;
		int drinkWeight = 35;
		int holyWaterWeight = 25;
		int crossWeight = 15;

		int total = failWeight + drinkWeight + holyWaterWeight + crossWeight;
		int roll = RandomProvider.Next(0, total);

		if (roll < failWeight)
			return new LootResult(false, null);

		roll -= failWeight;
		if (roll < drinkWeight)
		{
			// 부서진 집에서는 회복량이 조금 더 큰 드링크가 나올 수 있다.
			return new LootResult(true, new Drink(11, "드링크", "마시면 정신이 조금 더 또렷해진다.", 2));
		}

		roll -= drinkWeight;
		if (roll < holyWaterWeight)
		{
			return new LootResult(true, new HolyWater(20, "성수", "한 번, 광기의 손을 떼어낸다."));
		}

		return new LootResult(true, new Cross(21, "십자가", "한 번, 죽음을 되돌린다."));
	}
}
