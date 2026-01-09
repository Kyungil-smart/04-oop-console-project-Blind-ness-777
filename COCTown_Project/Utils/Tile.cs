public class Tile
{
    public GameObject OnTileObject { get; set; }
    public Vector Position { get; private set; }
    public Item ItemOnTile { get; set; }

    public bool HasGameObject => OnTileObject != null;

    public Tile(Vector position)
    {
        Position = position;
    }

    public void Print()
    {
        if (HasGameObject)
        {
            OnTileObject.Symbol.Print();
        }
        else if (ItemOnTile != null)
        {
            '*'.Print(); // 바닥에 아이템이 있음을 표시
        }
        else
        {
            '.'.Print();
        }
    }
}