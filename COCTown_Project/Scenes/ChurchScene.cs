using System;

public class ChurchScene : IndoorSceneBase
{
	public ChurchScene(PlayerCharacter player)
		: base(player, LocationType.Church, "성당 내부")
	{
		// 'F' : 성수대(정화/성당 시스템은 아직 전 단계)
		// 'S' : 지하로 내려가는 문(아직 잠김)
		BuildFromStrings(new string[]
		{
			"####################",
			"#..................#",
			"#......######......#",
			"#......#....#......#",
			"#......#..F.#......#",
			"#......#....#......#",
			"#......######......#",
			"#..................#",
			"#.........S........#",
            "#..................#",
			"##########+#########"
		});
	}

	protected override void OnSpecialInteract(char symbol)
	{
		if (symbol == 'F')
		{
			Console.Clear();
			Console.WriteLine("성수대 앞에 섰다.");
			Console.WriteLine();
			if (_player.Inventory.GetHolyRelicCount() < 5)
			{
				Console.WriteLine("(아직은 부족하다... 성물 5개가 필요하다)");
			}
			else
			{
				Console.WriteLine("성물 5개가 모였다.\n여기서 무언가를 할 수 있을 것 같다...(미구현)");
			}
			Console.WriteLine();
			Console.WriteLine("[Enter] 계속");
			while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }
			return;
		}

		if (symbol == 'S')
		{
			Console.Clear();
			Console.WriteLine("지하로 내려가는 문이다.");
			Console.WriteLine("지금은 열리지 않는다...(미구현)");
			Console.WriteLine("[Enter] 계속");
			while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }
			return;
		}

		base.OnSpecialInteract(symbol);
	}
}
