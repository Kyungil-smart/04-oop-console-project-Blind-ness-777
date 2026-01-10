using System;
using System.Collections.Generic;

public class TriggerService
{
    // 스텝 = 이동 1회
    private int _stepCounter;

    // 마지막 발동 스텝 기록
    private Dictionary<string, int> _lastTriggeredStepByKey = new Dictionary<string, int>();

    private List<IGameEvent> _roamTownEvents = new List<IGameEvent>();
    private List<IGameEvent> _roamHouseEvents = new List<IGameEvent>();
    private List<IGameEvent> _lootEvents = new List<IGameEvent>();

    public TriggerService()
    {
        // 기본 3종 이벤트 등록
        _lootEvents.Add(new LootCurseEvent());
        _roamTownEvents.Add(new WhisperInAlleyEvent());
        _roamHouseEvents.Add(new FootstepsBehindEvent());
    }

    public void OnStep(EventContext context)
    {
        _stepCounter++;

        // 배회 이벤트는 확률 발동
        if (context == null) return;

        if (context.LocationType == LocationType.Town)
        {
            TryTriggerRoam(context, _roamTownEvents);
        }
        else if (context.LocationType == LocationType.House)
        {
            TryTriggerRoam(context, _roamHouseEvents);
        }
    }

    public void OnLootAttempt(EventContext context)
    {
        if (context == null) return;
        TryTriggerLoot(context);
    }

    private void TryTriggerRoam(EventContext context, List<IGameEvent> events)
    {
        // 성물(정화용) 수가 늘수록 발동 확률이 증가
        // 0개: 8% / 5개: 28% 정도 느낌
        double baseChance = 0.08;
        double extraPerRelic = 0.04;

        double chance = baseChance + (context.HolyRelicCount * extraPerRelic);
        if (chance > 0.35) chance = 0.35;

        if (RandomProvider.NextDouble() > chance)
            return;

        for (int i = 0; i < events.Count; i++)
        {
            IGameEvent e = events[i];
            if (!CanTriggerByCooldown(e)) continue;
            if (!e.CanTrigger(context)) continue;

            MarkTriggered(e);
            e.Execute(context);
            return;
        }
    }

    private void TryTriggerLoot(EventContext context)
    {
        // 루팅 시 랜덤 이벤트 (1종) : 20% + 성물 수에 따라 상승
        double baseChance = 0.20;
        double extraPerRelic = 0.03;

        double chance = baseChance + (context.HolyRelicCount * extraPerRelic);
        if (chance > 0.40) chance = 0.40;

        if (RandomProvider.NextDouble() > chance)
            return;

        for (int i = 0; i < _lootEvents.Count; i++)
        {
            IGameEvent e = _lootEvents[i];
            if (!CanTriggerByCooldown(e)) continue;
            if (!e.CanTrigger(context)) continue;

            MarkTriggered(e);
            e.Execute(context);
            return;
        }
    }

    private bool CanTriggerByCooldown(IGameEvent e)
    {
        if (e == null) return false;

        int lastStep;
        if (!_lastTriggeredStepByKey.TryGetValue(e.Key, out lastStep))
            return true;

        return (_stepCounter - lastStep) >= e.CooldownSteps;
    }

    private void MarkTriggered(IGameEvent e)
    {
        if (e == null) return;
        _lastTriggeredStepByKey[e.Key] = _stepCounter;
    }
}
