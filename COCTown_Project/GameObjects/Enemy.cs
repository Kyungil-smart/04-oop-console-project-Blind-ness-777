using System;

public class SimpleChaser
{
    private int enemyX;
    private int enemyY;

    // 진동 방지용 (바로 직전 위치)
    private int prevX;
    private int prevY;
    private bool hasPrev = false;

    private Random random = new Random();

    public SimpleChaser(int startX, int startY)
    {
        enemyX = startX;
        enemyY = startY;
    }

    public int X { get { return enemyX; } }
    public int Y { get { return enemyY; } }

    public void StepTowardPlayer(int playerX, int playerY, char[,] map)
    {
        // 1) 이동 후보(최대 2개 축) 만들기
        int dx = 0;
        int dy = 0;

        if (playerX > enemyX) dx = 1;
        else if (playerX < enemyX) dx = -1;

        if (playerY > enemyY) dy = 1;
        else if (playerY < enemyY) dy = -1;

        // 축 우선순위: 더 멀리 떨어진 축부터 시도 (체감상 자연스러움)
        bool preferX = Math.Abs(playerX - enemyX) >= Math.Abs(playerY - enemyY);

        // (선택) 약간의 랜덤으로 패턴 깨기
        if (random.Next(0, 100) < 15) preferX = !preferX;

        // 2) 후보 순서대로 이동 시도
        if (preferX)
        {
            if (TryMove(dx, 0, map, playerX, playerY)) return;
            if (TryMove(0, dy, map, playerX, playerY)) return;
        }
        else
        {
            if (TryMove(0, dy, map, playerX, playerY)) return;
            if (TryMove(dx, 0, map, playerX, playerY)) return;
        }

        // 3) 둘 다 실패하면 제자리
    }

    private bool TryMove(int moveX, int moveY, char[,] map, int playerX, int playerY)
    {
        if (moveX == 0 && moveY == 0) return false;

        int nextX = enemyX + moveX;
        int nextY = enemyY + moveY;

        if (!IsInBounds(nextX, nextY, map)) return false;
        if (!IsWalkable(map[nextY, nextX])) return false;

        // 진동 방지: 바로 이전 칸으로 되돌아가려 하면 우선 막아보기
        if (hasPrev && nextX == prevX && nextY == prevY)
        {
            // 단, 플레이어가 바로 거기 있으면 추격이므로 허용해도 됨
            if (!(playerX == nextX && playerY == nextY))
            {
                return false;
            }
        }

        // 이동 확정
        prevX = enemyX;
        prevY = enemyY;
        hasPrev = true;

        enemyX = nextX;
        enemyY = nextY;
        return true;
    }

    private bool IsWalkable(char tile)
    {
        // 벽('#')과 단상/금고 같은 고정물은 통과 금지로 확장 가능
        return tile != '#';
    }

    private bool IsInBounds(int x, int y, char[,] map)
    {
        int height = map.GetLength(0);
        int width = map.GetLength(1);
        return x >= 0 && x < width && y >= 0 && y < height;
    }
}