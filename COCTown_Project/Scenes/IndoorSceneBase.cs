using System;

// 실내(집/성당/회관 등)에서 공통으로 쓰는 아주 단순한 베이스 씬
// - 문자열 배열로 맵을 그린다.
// - '#' : 벽(이동 불가)
// - '.' : 바닥
// - '+' : 출구(Enter로 밖으로)
// - '?' : 조사/루팅 포인트
// - 'R' : 확정 성물(루팅 포인트)
// - 'K' : 확정 열쇠(잠긴 집 열쇠, 루팅 포인트)
// - '2' : 확정 열쇠(금고 열쇠, 루팅 포인트)
// - 그 외 문자(예: 'F', 'S') : 특수 상호작용 포인트(일단 텍스트만)
public abstract class IndoorSceneBase : Scene
{
	protected Tile[,] _field;
	protected PlayerCharacter _player;
	protected LocationType _locationType;
	private HotKeyBar _hotKeyBar;

	protected Vector _spawn;
	protected string _sceneTitle;

	protected IndoorSceneBase(PlayerCharacter player, LocationType locationType, string sceneTitle)
	{
		_player = player;
		_locationType = locationType;
		_sceneTitle = sceneTitle;
		_hotKeyBar = new HotKeyBar(_player.Inventory);
	}

	protected void BuildFromStrings(string[] lines)
	{
		int height = lines.Length;
		int width = 0;
		for (int i = 0; i < lines.Length; i++)
			if (lines[i].Length > width) width = lines[i].Length;

		_field = new Tile[height, width];

		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				_field[y, x] = new Tile(new Vector(x, y));

				char c = (x < lines[y].Length) ? lines[y][x] : '#';

				if (c == '#')
				{
					_field[y, x].IsBlocked = true;
				}
				else if (c == '+')
				{
					_field[y, x].SpecialSymbol = '+';
					_field[y, x].IsBlocked = false;
					_spawn = new Vector(x, y);
				}
				else if (c == '?')
				{
					_field[y, x].IsLootSpot = true;
				}
				else if (c == 'R' || c == 'K' || c == '2')
				{
					// 고정 루팅 마커: 화면에는 기호를 보여주되, 동작은 루팅 포인트로 처리한다.
					_field[y, x].IsLootSpot = true;
					_field[y, x].SpecialSymbol = c;
					_field[y, x].IsBlocked = false;
				}
				else if (c == '.')
				{
					// 바닥
				}
				else
				{
					// 특수 타일(일단 표시 + Enter로 텍스트)
					_field[y, x].SpecialSymbol = c;
				}
			}
		}
	}

    public override void Enter()
    {
        _player.Field = _field;

        // 다음 스폰 심볼이 지정되어 있으면, 그 심볼 위치로 스폰을 옮긴다.
        if (SceneManager.NextSpawnSymbol != '\0')
        {
            TrySetSpawnOnSymbol(SceneManager.NextSpawnSymbol);
            SceneManager.NextSpawnSymbol = '\0';
        }

        _player.Position = _spawn;
        _field[_player.Position.Y, _player.Position.X].OnTileObject = _player;

        Console.Clear();
        Console.WriteLine(_sceneTitle);
        Console.WriteLine("[Enter] 계속");
        while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }
    }

    public override void Update()
	{
		// 인벤 열기
		if (InputManager.GetKey(ConsoleKey.B))
		{
			_player.Inventory.ShowInventoryScreen();
			return;
		}

		// 단축키 아이템 사용(1~6 / 넘패드 1~6)
		if (HandleHotKeyUse())
			return;

		// 이동
		if (InputManager.GetKey(ConsoleKey.UpArrow)) TryMove(0, -1);
		else if (InputManager.GetKey(ConsoleKey.DownArrow)) TryMove(0, 1);
		else if (InputManager.GetKey(ConsoleKey.LeftArrow)) TryMove(-1, 0);
		else if (InputManager.GetKey(ConsoleKey.RightArrow)) TryMove(1, 0);

		if (InputManager.GetKey(ConsoleKey.Enter))
			Interact();
	}

	private bool HandleHotKeyUse()
	{
		int slotIndex = -1;

		if (InputManager.GetKey(ConsoleKey.D1)) slotIndex = 0;
		else if (InputManager.GetKey(ConsoleKey.D2)) slotIndex = 1;
		else if (InputManager.GetKey(ConsoleKey.D3)) slotIndex = 2;
		else if (InputManager.GetKey(ConsoleKey.D4)) slotIndex = 3;
		else if (InputManager.GetKey(ConsoleKey.D5)) slotIndex = 4;
		else if (InputManager.GetKey(ConsoleKey.D6)) slotIndex = 5;
		else if (InputManager.GetKey(ConsoleKey.NumPad1)) slotIndex = 0;
		else if (InputManager.GetKey(ConsoleKey.NumPad2)) slotIndex = 1;
		else if (InputManager.GetKey(ConsoleKey.NumPad3)) slotIndex = 2;
		else if (InputManager.GetKey(ConsoleKey.NumPad4)) slotIndex = 3;
		else if (InputManager.GetKey(ConsoleKey.NumPad5)) slotIndex = 4;
		else if (InputManager.GetKey(ConsoleKey.NumPad6)) slotIndex = 5;

		if (slotIndex == -1)
			return false;

		Item item = _player.Inventory.GetItem(slotIndex);
		if (item == null)
		{
			Console.Clear();
			Console.WriteLine("그 슬롯은 비어있다.");
			Console.WriteLine("[Enter] 계속");
			Console.ReadLine();
			return true;
		}

		if (item is HolyWater || item is Cross)
		{
			Console.Clear();
			Console.WriteLine(item + " 는(은) 위기 상황에서 자동으로 소모되는 보호 아이템이다.");
			Console.WriteLine("(단축키로 직접 사용할 수 없다)");
			Console.WriteLine("[Enter] 계속");
			Console.ReadLine();
			return true;
		}

		bool used = _player.Inventory.UseAt(slotIndex);
		Console.Clear();
		Console.WriteLine(used ? ("아이템을 사용했다: " + item) : "지금은 사용할 수 없다.");
		Console.WriteLine("[Enter] 계속");
		Console.ReadLine();
		return true;
	}

    public override void Render()
    {
        int uiTopOffsetY = 2;

        Console.SetCursorPosition(0, 0);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, 1);
        Console.Write(new string(' ', Console.WindowWidth));

        Console.SetCursorPosition(0, uiTopOffsetY);
        for (int y = 0; y < _field.GetLength(0); y++)
        {
            for (int x = 0; x < _field.GetLength(1); x++)
            {
                _field[y, x].Print();
            }
            Console.WriteLine();
        }

        int hotkeyY = uiTopOffsetY + _field.GetLength(0) + 2;
        _hotKeyBar.Render(0, hotkeyY);
        _player.DrawSanityGauge();

        int clearFromY = hotkeyY + 6;
        ConsoleErase.ClearLinesFrom(clearFromY);
    }

    public override void Exit()
	{
		_field[_player.Position.Y, _player.Position.X].OnTileObject = null;
		_player.Field = null;
	}

	protected void TryMove(int dx, int dy)
	{
		int newX = _player.Position.X + dx;
		int newY = _player.Position.Y + dy;

		if (newX < 0 || newX >= _field.GetLength(1)) return;
		if (newY < 0 || newY >= _field.GetLength(0)) return;
		if (_field[newY, newX].IsBlocked) return;

		_field[_player.Position.Y, _player.Position.X].OnTileObject = null;
		_player.Position = new Vector(newX, newY);
		_field[newY, newX].OnTileObject = _player;
	}

	protected void Interact()
	{
		Vector pos = _player.Position;
		Tile tile = _field[pos.Y, pos.X];

        // 출구
        if (tile.SpecialSymbol == '+')
        {
            Console.Clear();
            Console.WriteLine("밖으로 나간다...");
            Console.WriteLine("[Enter] 계속");
            while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }

            // 층 이동 때문에 Prev가 꼬일 수 있으므로, 출구는 고정으로 타운으로 나간다.
            SceneManager.Change("Town");
            return;
        }

        // 루팅
        if (tile.IsLootSpot)
		{
			// 확정 루팅(성물/열쇠)
			if (TryHandleFixedLoot(tile))
				return;

			EventContext context = new EventContext(_player, _locationType);
			Console.Clear();
			Console.WriteLine("주변을 조사한다...");
			Console.WriteLine();

			LootResult loot = LootSystem.RollLoot(context);
			if (!loot.FoundSomething)
			{
				Console.WriteLine("아무것도 찾지 못했다.");
			}
			else
			{
				bool added = _player.Inventory.TryAdd(loot.Item);
				if (added)
				{
					Console.WriteLine("무언가를 얻었다: " + loot.Item);
					tile.IsLootSpot = false;
				}
				else
				{
					Console.WriteLine("가방이 가득 찼다. 아무것도 챙기지 못했다.");
				}
			}

			Console.WriteLine();
			Console.WriteLine("[Enter] 계속");
			while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }
			return;
		}

		// 기타 특수 타일
		if (tile.SpecialSymbol != '\0')
		{
			OnSpecialInteract(tile.SpecialSymbol);
			return;
		}

		Console.Clear();
		Console.WriteLine("특별한 건 없다.");
		Console.WriteLine("[Enter] 계속");
		while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }
	}

	private bool TryHandleFixedLoot(Tile tile)
	{
		if (tile == null) return false;

		char symbol = tile.SpecialSymbol;
		if (symbol != 'R' && symbol != 'K' && symbol != '2')
			return false;

		Item fixedItem = null;

		if (symbol == 'R')
		{
			fixedItem = new HolyRelicPiece(100, "파편 성물", "정화에 필요한 성물 조각이다.");
		}
		else if (symbol == 'K')
		{
			fixedItem = new LockedHouseKey(201, "낡은 집 열쇠", "잠긴 집의 문을 열 수 있는 열쇠다.");
		}
		else if (symbol == '2')
		{
			fixedItem = new SafeKey(202, "금고 열쇠", "잠긴 집 내부의 금고를 열 수 있는 열쇠다.");
		}

		Console.Clear();
		Console.WriteLine("주변을 조사한다...");
		Console.WriteLine();

		bool added = _player.Inventory.TryAdd(fixedItem);
		if (added)
		{
			Console.WriteLine("무언가를 얻었다: " + fixedItem);
			tile.IsLootSpot = false;
			tile.SpecialSymbol = '\0';
		}
		else
		{
			Console.WriteLine("가방이 가득 찼다. 아무것도 챙기지 못했다.");
		}

		Console.WriteLine();
		Console.WriteLine("[Enter] 계속");
		while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }
		return true;
	}

	protected virtual void OnSpecialInteract(char symbol)
	{
		Console.Clear();
		Console.WriteLine("(아직 구현 전) " + symbol);
		Console.WriteLine("[Enter] 계속");
		while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }
	}

    protected void TrySetSpawnOnSymbol(char symbol)
    {
        for (int y = 0; y < _field.GetLength(0); y++)
        {
            for (int x = 0; x < _field.GetLength(1); x++)
            {
                if (_field[y, x].SpecialSymbol == symbol)
                {
                    _spawn = new Vector(x, y);
                    return;
                }
            }
        }
    }
}
