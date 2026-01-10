public interface IGameEvent
{
    string Key { get; }

    // 이 이벤트가 지금 상황에서 발동 가능한가?
    bool CanTrigger(EventContext context);

    // 실행(메시지 출력/정신력 변동/아이템 소모 등)
    void Execute(EventContext context);

    // 발동 후 재발동까지 필요한 "스텝" 수 (배회 이벤트 쿨다운용)
    int CooldownSteps { get; }
}
