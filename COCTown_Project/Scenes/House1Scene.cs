public class House1Scene : IndoorSceneBase
{
	public House1Scene(PlayerCharacter player)
		: base(player, LocationType.House, "낡은 민가(1층)")
	{
		// 임의로 잡은 내부 배치 + 루팅 포인트
		BuildFromStrings(new string[]
		{
			"###############",
			"#..R...#......#",
			"#......##..?...#",
			"#.......#......#",
			"#..#######.....#",
			"#..............#",
			"#....###.##..###",
			"#....#....#....#",
			"#....#....#....#",
			"#....#....#...?#",
			"#######+#######"
		});
	}
}
