public class Tile
{
    public GameObject OnTileObject { get; set; }
    public Vector Position { get; private set; }
    public Item ItemOnTile { get; set; }

	// "조사" 가능한 루팅 포인트 (바닥에 아이템이 놓여있는 것과 별개)
	public bool IsLootSpot { get; set; }

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
		else if (IsLootSpot)
		{
			'?'.Print(); // 조사/루팅 포인트
		}
        else
        {
            '.'.Print();
        }
    }
}