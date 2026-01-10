using System;

public static class EventUtils
{
    // 정신력 감소를 적용하되,
    // 1) 성수가 있으면 1회 -1을 막고 소모
    // 2) 죽을 상황이면 십자가로 1회 죽음을 막고 소모
    public static void ApplySanityDamage(EventContext context, int amount)
    {
        if (context == null || context.Player == null) return;
        if (amount <= 0) return;

        for (int i = 0; i < amount; i++)
        {
            // 성수: -1 1회 방어
            bool blocked = context.Player.Inventory.TryConsumeHolyWater();
            if (blocked)
            {
                Console.WriteLine("[성수]가 스스로 깨져 정신력 감소를 막았다 (-0)");
                continue;
            }

            int before = context.Player.Sanity.Value;
            context.Player.DecreaseSanity(1);

            // 죽음 체크
            if (before > 0 && context.Player.Sanity.Value <= 0)
            {
                bool saved = context.Player.Inventory.TryConsumeCross();
                if (saved)
                {
                    context.Player.Sanity.Value = 1;
                    Console.WriteLine("[십자가]가 부서지며 죽음을 되돌렸다 (정신력 1)");
                }
            }
        }
    }

    public static void WaitForEnter()
    {
        Console.WriteLine("[Enter] 계속");
        Console.ReadLine();
    }
}
