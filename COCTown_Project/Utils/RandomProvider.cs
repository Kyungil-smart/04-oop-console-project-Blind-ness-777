using System;

public static class RandomProvider
{
    // 게임 전체에서 하나만 쓰기(연속 호출 시 똑같은 값 나오는 문제 방지)
    private static readonly Random _random = new Random();

    public static int Next(int minInclusive, int maxExclusive)
    {
        return _random.Next(minInclusive, maxExclusive);
    }

    public static int Next(int maxExclusive)
    {
        return _random.Next(maxExclusive);
    }

    public static double NextDouble()
    {
        return _random.NextDouble();
    }
}
