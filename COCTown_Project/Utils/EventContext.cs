public class EventContext
{
    public PlayerCharacter Player { get; private set; }
    public LocationType LocationType { get; private set; }

    public EventContext(PlayerCharacter player, LocationType locationType)
    {
        Player = player;
        LocationType = locationType;
    }

    public int HolyRelicCount
    {
        get { return Player == null ? 0 : Player.Inventory.GetHolyRelicCount(); }
    }
}
