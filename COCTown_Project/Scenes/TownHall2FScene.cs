using System;

// 촌장집 / 마을회관 2층
// - 내부 모양과 루팅 포인트(?) 배치는 자유롭게 수정 가능
// - 'D' : 1층으로 내려가는 계단(또는 내려가는 출입구)
public class TownHall2FScene : IndoorSceneBase
{
	public TownHall2FScene(PlayerCharacter player)
		: base(player, LocationType.House, "촌장집 / 마을회관 (2층)")
	{
		BuildFromStrings(new string[]
		{
			"########################",
			"#..D...............T...#",
			"#......................#",
			"#..............K.......#",
			"#......................#",
			"########################",
		});
	}

	protected override void OnSpecialInteract(char symbol)
	{
		if (symbol == 'D')
		{
			Console.Clear();
			Console.WriteLine("1층으로 내려간다...");
			Console.WriteLine("[Enter] 계속");
			while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }

            SceneManager.SetNextSpawnSymbol('U');
            SceneManager.Change("TownHall");

            return;
		}

		if (symbol == 'T')
		{
			Console.Clear();
			Console.WriteLine("전능하신 신이시여. 저희를 굽어 살피소서. 하늘 위에 계시는.....");
			Console.WriteLine("위대한 #@$%%#^시여.....");
            while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }
			return;
        }

		base.OnSpecialInteract(symbol);
	}
}
