using System;

public static class DoorLinker
{
    public static void LinkDoor(Tile[,] field, int doorX, int doorY, string targetScene)
    {
        if (field == null) return;

        int height = field.GetLength(0);
        int width = field.GetLength(1);

        if (doorY < 0 || doorY >= height || doorX < 0 || doorX >= width)
            return;

        Tile tile = field[doorY, doorX];

        // 맵 자산 규칙: '+'가 아닌 곳을 문으로 만들지 않는다.
        if (tile.SpecialSymbol != '+')
            return;

        tile.DoorTargetScene = targetScene;
    }
}