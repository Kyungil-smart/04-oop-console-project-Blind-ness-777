public class StoryScene : Scene
{
    private string[] storyTexts;
    private int _lineIndex;

    private const string highlightText = "코크 타운(COC Town)";
    private const ConsoleColor highlightColor = ConsoleColor.Red;

    public override void Enter()
    {
        storyTexts = new string[]
        {
            "산 속 한 가운데 있는 오래된 마을에 사람들이 감쪽같이 사라지는 사건이 발생했다.",
            "마을 사람들의 친인척들이 유명한 탐정에게 이 사건을 조사해달라고 의뢰를 한다.",
            "의뢰를 받은 탐정은 사건에 깊은 흥미를 느끼며, 산 중 마을. 코크 타운(COC Town)",
            "사건의 현장. 그 곳으로 향했다."
        };

        _lineIndex = 0;
    }

    public override void Update()
    {
        if (InputManager.GetKey(ConsoleKey.Enter))
        {
            _lineIndex++;

            if (_lineIndex >= storyTexts.Length)
                SceneManager.Change("Town");
        }
    }

    public override void Render()
    {
        Console.Clear();

        Console.WriteLine("=== PROLOGUE ===\n");

        for (int lineNumber = 0; lineNumber <= _lineIndex && lineNumber < storyTexts.Length; lineNumber++)
        {
            PrintLineWithHighlight(storyTexts[lineNumber]);
            Console.WriteLine();
        }

        Console.WriteLine("\n[Enter] 다음");
    }

    private void PrintLineWithHighlight(string fullText)
    {
        int highlightStartIndex = fullText.IndexOf(highlightText);

        if (highlightStartIndex < 0)
        {
            fullText.Print();
            return;
        }

        string beforeText = fullText.Substring(0, highlightStartIndex);
        string afterText = fullText.Substring(highlightStartIndex + highlightText.Length);

        beforeText.Print();
        highlightText.Print(highlightColor);
        afterText.Print();
    }

    public override void Exit()
    {

    }
}