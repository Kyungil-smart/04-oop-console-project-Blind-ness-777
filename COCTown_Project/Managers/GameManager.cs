using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GameManager
{
    public static bool IsGameOver { get; private set; }

    public static void RequestQuit()
    {
        IsGameOver = true;
    }
    
    public void Run()
    {
        Init();

        while (!IsGameOver)
        {
            InputManager.Poll();

            SceneManager.Update();

            Console.Clear();
            SceneManager.Render();
        }
    }

    private void Init()
    {
        IsGameOver = false;

        Console.CursorVisible = false;

        // 버퍼/창 크기 고정 (숫자는 너 취향대로)
        Console.SetBufferSize(80, 25);
        Console.SetWindowSize(80, 25);

        // Scene 등록/첫 씬 Change도 여기서
    }
}

