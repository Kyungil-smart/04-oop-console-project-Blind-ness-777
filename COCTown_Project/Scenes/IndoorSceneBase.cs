using System;

// 실내(집/성당/회관 등)에서 공통으로 쓰는 아주 단순한 베이스 씬
// - 문자열 배열로 맵을 그린다.
// - '#' : 벽(이동 불가)
// - '.' : 바닥
// - '+' : 출구(Enter로 밖으로)
// - '?' : 조사/루팅 포인트
// - 그 외 문자(예: 'F', 'S') : 특수 상호작용 포인트(일단 텍스트만)
public abstract class IndoorSceneBase : Scene
{
	protected Tile[,] _field;
	protected PlayerCharacter _player;
	protected LocationType _locationType;

	protected Vector _spawn;
	protected string _sceneTitle;

	protected IndoorSceneBase(PlayerCharacter player, LocationType locationType, string sceneTitle)
	{
		_player = player;
		_locationType = locationType;
		_sceneTitle = sceneTitle;
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
		_player.Position = _spawn;
		_field[_player.Position.Y, _player.Position.X].OnTileObject = _player;

		Console.Clear();
		Console.WriteLine(_sceneTitle);
		Console.WriteLine("[Enter] 계속");
		while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }
	}

	public override void Update()
	{
		// 이동
		if (InputManager.GetKey(ConsoleKey.UpArrow)) TryMove(0, -1);
		else if (InputManager.GetKey(ConsoleKey.DownArrow)) TryMove(0, 1);
		else if (InputManager.GetKey(ConsoleKey.LeftArrow)) TryMove(-1, 0);
		else if (InputManager.GetKey(ConsoleKey.RightArrow)) TryMove(1, 0);

		if (InputManager.GetKey(ConsoleKey.Enter))
			Interact();
	}

	public override void Render()
	{
		Console.SetCursorPosition(0, 0);
		for (int y = 0; y < _field.GetLength(0); y++)
		{
			for (int x = 0; x < _field.GetLength(1); x++)	
			{
				_field[y, x].Print();
			}
			Console.WriteLine();
		}
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
			SceneManager.ChangePrevScene();
			return;
		}

		// 루팅
		if (tile.IsLootSpot)
		{
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

	protected virtual void OnSpecialInteract(char symbol)
	{
		Console.Clear();
		Console.WriteLine("(아직 구현 전) " + symbol);
		Console.WriteLine("[Enter] 계속");
		while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }
	}
}
