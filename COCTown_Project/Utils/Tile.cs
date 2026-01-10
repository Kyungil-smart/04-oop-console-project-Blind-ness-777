public class Tile
{
    public GameObject OnTileObject { get; set; }
    public Vector Position { get; private set; }
    public Item ItemOnTile { get; set; }

	// 입구/정문/특수 구역 등 "한 글자"로 표시하고 싶은 타일이 있을 때 사용.
	// '\0'이면 사용하지 않는다.
	public char SpecialSymbol { get; set; }

	// "조사" 가능한 루팅 포인트 (바닥에 아이템이 놓여있는 것과 별개)
	public bool IsLootSpot { get; set; }

	// 이동 불가(벽/건물 외벽 등)
	public bool IsBlocked { get; set; }

	// 문(입구) 타일일 때, Enter 상호작용 시 이동할 씬 이름(예: "BrokenHouse")
	public string DoorTargetScene { get; set; }

    public bool HasGameObject => OnTileObject != null;

    public Tile(Vector position)
    {
        Position = position;
		SpecialSymbol = '\0';
    }

    public void Print()
    {
        if (HasGameObject)
        {
            OnTileObject.Symbol.Print();
        }
		else if (SpecialSymbol != '\0')
		{
			SpecialSymbol.Print();
		}
		else if (IsBlocked)
		{
			'#'.Print(); // 벽/건물 외벽
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