using System;

// 1) 루팅 이벤트(랜덤 발생 1종)
public class LootCurseEvent : IGameEvent
{
    public string Key { get { return "LOOT_CURSE"; } }
    public int CooldownSteps { get { return 8; } }

    public bool CanTrigger(EventContext context)
    {
        // 정신력이 이미 0이면 의미 없음
        return context != null && context.Player != null && !context.Player.IsDead();
    }

    public void Execute(EventContext context)
    {
        Console.Clear();
        Console.WriteLine("루팅 도중, 손끝에 차가운 무언가가 스쳤다...");
        Console.WriteLine("등골이 서늘해지며 머리가 멍해진다.");
        Console.WriteLine();

        EventUtils.ApplySanityDamage(context, 1);
        EventUtils.WaitForEnter();
    }
}

// 2) 마을 배회 이벤트
public class WhisperInAlleyEvent : IGameEvent
{
    public string Key { get { return "TOWN_WHISPER"; } }
    public int CooldownSteps { get { return 12; } }

    public bool CanTrigger(EventContext context)
    {
        if (context == null || context.Player == null) return false;
        return context.LocationType == LocationType.Town;
    }

    public void Execute(EventContext context)
    {
        Console.Clear();
        Console.WriteLine("골목 끝에서 누군가 낮게 속삭이는 소리가 들린다.");
        Console.WriteLine("말은 알아들을 수 없는데, 내 이름이 섞여있는 것만 같다.");
        Console.WriteLine();

        EventUtils.ApplySanityDamage(context, 1);
        EventUtils.WaitForEnter();
    }
}

// 3) 집 배회 이벤트
public class FootstepsBehindEvent : IGameEvent
{
    public string Key { get { return "HOUSE_FOOTSTEPS"; } }
    public int CooldownSteps { get { return 12; } }

    public bool CanTrigger(EventContext context)
    {
        if (context == null || context.Player == null) return false;
        return context.LocationType == LocationType.House;
    }

    public void Execute(EventContext context)
    {
        Console.Clear();
        Console.WriteLine("삐걱... 바닥이 울린다.");
        Console.WriteLine("분명 혼자인데, 내 뒤에서 발소리가 한 번 더 따라온다.");
        Console.WriteLine();

        // 집은 더 불안하게: 1~2 랜덤
        int damage = RandomProvider.Next(1, 3);
        EventUtils.ApplySanityDamage(context, damage);
        EventUtils.WaitForEnter();
    }
}
