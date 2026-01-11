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
			"#####################",
			"#S.................R#",
            "#........#T#........#",
            "#...................#",
            "#?.######...######..#",
            "#...................#",
            "#..######...######..#",
			"#...................#",
            "#..######...######.?#",
            "#...................#",
			"##########+##########"
		});
	}

	protected override void OnSpecialInteract(char symbol)
	{
		if (symbol == 'S')
		{
			Console.Clear();
			Console.WriteLine("지하로 내려간다...");
			Console.WriteLine("[Enter] 계속");
			while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }

            SceneManager.SetNextSpawnSymbol('S'); // 지하 맵의 S 위로 스폰
            SceneManager.Change("ChurchBasement");

            return;
		}

        if (symbol == 'T')
        {
            Console.Clear();
            Console.WriteLine("이 마을은 오염되었다......");
            Console.WriteLine("당장 도망....... 전능하신... 신이시여.....");
            while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }
            return;
        }

        base.OnSpecialInteract(symbol);
	}
}
