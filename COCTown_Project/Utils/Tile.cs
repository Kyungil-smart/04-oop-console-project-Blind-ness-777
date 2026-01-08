public class Tile
{
    public GameObject OnTileObject { get; set; }
    public Vector Position { get; private set; }

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
        else
        {
            '.'.Print();
        }
    }
}