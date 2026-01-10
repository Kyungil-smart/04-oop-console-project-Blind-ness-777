using System;

// 부서진 집(선택 루트): 하이리스크/하이리턴
// - 이 씬 안에서는 "추격"이 항상 활성화
// - 루팅 테이블은 LootSystem.RollBrokenHouseLoot 사용
public class BrokenHouseScene : Scene
{
	private Tile[,] _field = new Tile[13, 25];
	private PlayerCharacter _player;
	private HotKeyBar _hotKeyBar;

	// 추격(적 X, 흔적 :)
	private System.Collections.Generic.List<Vector> _enemies = new System.Collections.Generic.List<Vector>();
	private System.Collections.Generic.Dictionary<Vector, int> _trails = new System.Collections.Generic.Dictionary<Vector, int>();

	private LocationType _locationType = LocationType.BrokenHouse;

	private Random _random = new Random();

	public BrokenHouseScene(PlayerCharacter player)
	{
		_player = player;
		_hotKeyBar = new HotKeyBar(_player.Inventory);

		// 맵 초기화
		for (int y = 0; y < _field.GetLength(0); y++)
		{
			for (int x = 0; x < _field.GetLength(1); x++)
			{
				_field[y, x] = new Tile(new Vector(x, y));
			}
		}

		// ===== 부서진 집 내부 맵(벽 # + 미로) =====
		// - '#' : 벽(이동 불가)
		// - '.' : 바닥(이동 가능)
		// - '+' : 출구(마을로 돌아가기)
		string[] map =
		{
			"#########################",
			"#.?...#....?......#..?..#",
			"#.###.#.#####.###.#.###.#",
			"#.#...#..?..#...#.#...#.#",
			"#.#.#######.###.#.###.#.#",
			"#.#...?...#..?..#.....#.#",
			"#.#######.###########.#.#",
			"#.....#...#..?..#.....#.#",
			"###.#.#.###.###.#.###.#.#",
			"#...#.#.....#...#...#?..#",
			"#.###.#######.#####.###.#",
			"#.?...#.......?.......#.#",
			"###########+#############",
		};

		for (int y = 0; y < _field.GetLength(0); y++)
		{
			for (int x = 0; x < _field.GetLength(1); x++)
			{
				char c = map[y][x];
				if (c == '#')
				{
					_field[y, x].IsBlocked = true;
				}
				else if (c == '+')
				{
					_field[y, x].SpecialSymbol = '+';
					_field[y, x].IsBlocked = false;
				}
				else
				{
					_field[y, x].IsBlocked = false;
				}
			}
		}

		// 루팅 포인트(부서진 집은 밀도가 높다)
		_field[1, 2].IsLootSpot = true;
        _field[1, 11].IsLootSpot = true;
        _field[1, 21].IsLootSpot = true;
        _field[3, 9].IsLootSpot = true;
        _field[5, 6].IsLootSpot = true;
        _field[5, 13].IsLootSpot = true;
        _field[7, 13].IsLootSpot = true;
        _field[9, 21].IsLootSpot = true;
        _field[11, 2].IsLootSpot = true;
        _field[11, 14].IsLootSpot = true;
    }

	public override void Enter()
	{
		_player.Field = _field;
		// 출구 근처에서 시작
		_player.Position = new Vector(2, 1);
		_field[_player.Position.Y, _player.Position.X].OnTileObject = _player;

		// 적 1마리 스폰(멀리)
		_enemies.Clear();
		_trails.Clear();
		_enemies.Add(new Vector(12, 1));

		Console.Clear();
		Console.WriteLine("부서진 집 안은 조용하다...");
		Console.WriteLine("(뒤에서 발소리가 따라온다)");
		Console.WriteLine("[Enter] 계속");
		Console.ReadLine();
	}

	public override void Update()
	{
		// 인벤 열기
		if (InputManager.GetKey(ConsoleKey.B))
		{
			Console.SetCursorPosition(0, Console.WindowHeight - 1);
			Console.Write(new string(' ', Console.WindowWidth));
			Console.SetCursorPosition(0, Console.WindowHeight - 1);
			Console.Write("지금은 소지품을 열 수 없다...");
			return;
		}

		// 단축키 아이템 사용(1~6 / 넘패드 1~6) - TownScene과 동일 컨셉
		if (HandleHotKeyUse())
			return;

		// 이동
		if (InputManager.GetKey(ConsoleKey.UpArrow)) TryMove(0, -1);
		else if (InputManager.GetKey(ConsoleKey.DownArrow)) TryMove(0, 1);
		else if (InputManager.GetKey(ConsoleKey.LeftArrow)) TryMove(-1, 0);
		else if (InputManager.GetKey(ConsoleKey.RightArrow)) TryMove(1, 0);

		// 상호작용
		if (InputManager.GetKey(ConsoleKey.Enter))
		{
			Interact();
		}
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

		if (slotIndex == -1) return false;

		Item item = _player.Inventory.GetItem(slotIndex);
		if (item == null)
		{
			Console.Clear();
			Console.WriteLine("그 슬롯은 비어있다.");
			Console.WriteLine("[Enter] 계속");
			Console.ReadLine();
			return true;
		}

		// 성수/십자가는 자동 소모 컨셉
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
		Console.WriteLine(used ? ("아이템을 사용했다: " + item) : "사용할 수 없다.");
		Console.WriteLine("[Enter] 계속");
		Console.ReadLine();
		return true;
	}

	private void TryMove(int dx, int dy)
	{
		int newX = _player.Position.X + dx;
		int newY = _player.Position.Y + dy;
		if (newX < 0 || newX >= _field.GetLength(1)) return;
		if (newY < 0 || newY >= _field.GetLength(0)) return;

		_field[_player.Position.Y, _player.Position.X].OnTileObject = null;
		_player.Position = new Vector(newX, newY);
		_field[newY, newX].OnTileObject = _player;

		// 이동할 때마다 적도 1칸씩 접근
		UpdateEnemies();

		if (_player.IsDead()) GameOver();
	}

	private void Interact()
	{
		Vector pos = _player.Position;
		Tile tile = _field[pos.Y, pos.X];

		// 출구
		if (tile.SpecialSymbol == '+')
		{
			Console.Clear();
			Console.WriteLine("서둘러 밖으로 빠져나온다...");
			Console.WriteLine("[Enter] 계속");
			Console.ReadLine();
			SceneManager.ChangePrevScene();
			return;
		}

		// 루팅 포인트
		if (tile.IsLootSpot)
		{
			EventContext context = new EventContext(_player, _locationType);
			Console.Clear();
			Console.WriteLine("부서진 잔해를 뒤진다...");
			Console.WriteLine();

			LootResult loot = LootSystem.RollBrokenHouseLoot(context);
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
			Console.ReadLine();
			return;
		}

		Console.Clear();
		Console.WriteLine("아무것도 없다...");
		Console.WriteLine("[Enter] 계속");
		Console.ReadLine();	
	}

	public override void Render()
	{
		Console.SetCursorPosition(0, 0);
		PrintField();
		int hotkeyY = Console.WindowHeight - 2;
		_hotKeyBar.Render(0, hotkeyY);
		_player.DrawSanityGauge();
	}

	private void PrintField()
	{
		for (int y = 0; y < _field.GetLength(0); y++)
		{
			for (int x = 0; x < _field.GetLength(1); x++)
			{
				Vector p = new Vector(x, y);

				if (_field[y, x].HasGameObject)
				{
					_field[y, x].OnTileObject.Symbol.Print();
					continue;
				}

				if (IsEnemyAt(p))
				{
					'X'.Print();
					continue;
				}

				if (_trails.ContainsKey(p))
				{
					':'.Print();
					continue;
				}

				_field[y, x].Print();
			}
			Console.WriteLine();
		}
	}

	private void UpdateEnemies()
	{
		DecayTrails();
		for (int i = 0; i < _enemies.Count; i++)
		{
			Vector from = _enemies[i];
			Vector to = MoveOneStepTowards(from, _player.Position);
			if (to.X != from.X || to.Y != from.Y)
			{
				AddTrail(from, 2);
			}
			_enemies[i] = to;
			if (to.X == _player.Position.X && to.Y == _player.Position.Y)
			{
				HandleEnemyHitPlayer(i, from);
				return;
			}
		}
	}

	private void HandleEnemyHitPlayer(int enemyIndex, Vector enemyPrevPos)
	{
		bool saved = _player.Inventory.TryConsumeCross();
		if (saved)
		{
			Console.Clear();
			Console.WriteLine("십자가가 부서지며, 죽음을 막아냈다!");
			Console.WriteLine("[Enter] 계속");
			Console.ReadLine();
			_enemies[enemyIndex] = enemyPrevPos;
			if (_player.Sanity.Value <= 0) _player.Sanity.Value = 1;
			else _player.Sanity.Value = 1;
			return;
		}
		GameOver();
	}

	private void AddTrail(Vector pos, int life)
	{
		if (_trails.ContainsKey(pos))
		{
			if (_trails[pos] < life) _trails[pos] = life;
			return;
		}
		_trails.Add(pos, life);
	}

	private void DecayTrails()
	{
		if (_trails.Count == 0) return;
		System.Collections.Generic.List<Vector> keys = new System.Collections.Generic.List<Vector>(_trails.Keys);
		for (int i = 0; i < keys.Count; i++)
		{
			Vector k = keys[i];
			_trails[k] = _trails[k] - 1;
			if (_trails[k] <= 0) _trails.Remove(k);
		}
	}

	private bool IsEnemyAt(Vector pos)
	{
		for (int i = 0; i < _enemies.Count; i++)
		{
			Vector e = _enemies[i];
			if (e.X == pos.X && e.Y == pos.Y) return true;
		}
		return false;
	}

	private Vector MoveOneStepTowards(Vector from, Vector target)
	{
		int dx = target.X - from.X;
		int dy = target.Y - from.Y;
		int stepX = 0;
		int stepY = 0;

		if (Math.Abs(dx) >= Math.Abs(dy))
		{
			if (dx > 0) stepX = 1;
			else if (dx < 0) stepX = -1;
		}
		else
		{
			if (dy > 0) stepY = 1;
			else if (dy < 0) stepY = -1;
		}

		Vector next = new Vector(from.X + stepX, from.Y + stepY);
		if (next.X < 0 || next.X >= _field.GetLength(1)) return from;
		if (next.Y < 0 || next.Y >= _field.GetLength(0)) return from;
		return next;
	}

	public override void Exit()
	{
		_field[_player.Position.Y, _player.Position.X].OnTileObject = null;
		_player.Field = null;
	}

	private void GameOver()
	{
		Console.Clear();
		Console.WriteLine("그것은 눈앞까지 다가왔다...");
		Console.WriteLine("사망!");
		Console.WriteLine("아무 키나 누르면 종료합니다...");
		Console.ReadKey(true);
		Environment.Exit(0);
	}
}