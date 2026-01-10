using System;
using System.Collections.Generic;

// 성물 4개는 "시작 시점에" 특정 루팅 포인트 4곳에만 배정된다.
// - 그 포인트를 조사하면 무조건 성물 획득(인벤에 들어갈 때만 소모 처리)
// - 다른 루팅 포인트에서는 성물이 나오지 않는다(실패/일반아이템만)
public static class RelicPlacementSystem
{
    private static bool _initialized = false;

    // 성물이 배정된 루팅 포인트 좌표들
    private static HashSet<Vector> _relicSpots = new HashSet<Vector>();

    // 이미 성물을 가져간 좌표들(중복 방지)
    private static HashSet<Vector> _collectedSpots = new HashSet<Vector>();

    public static void InitializeIfNeeded(List<Vector> allLootSpots, int relicSpotCount)
    {
        if (_initialized) return;

        _relicSpots.Clear();
        _collectedSpots.Clear();

        if (allLootSpots == null || allLootSpots.Count == 0)
        {
            _initialized = true;
            return;
        }

        // 복사 후 셔플
        List<Vector> copy = new List<Vector>(allLootSpots);
        Shuffle(copy);

        int count = relicSpotCount;
        if (count > copy.Count) count = copy.Count;

        for (int i = 0; i < count; i++)
        {
            _relicSpots.Add(copy[i]);
        }

        _initialized = true;
    }

    public static bool HasRelicAt(Vector position)
    {
        // 성물이 배정된 곳이고, 아직 안 가져갔으면 true
        return _relicSpots.Contains(position) && !_collectedSpots.Contains(position);
    }

    public static void MarkRelicCollected(Vector position)
    {
        if (!_collectedSpots.Contains(position))
            _collectedSpots.Add(position);
    }

    private static void Shuffle(List<Vector> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = RandomProvider.Next(0, i + 1);

            Vector temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}
