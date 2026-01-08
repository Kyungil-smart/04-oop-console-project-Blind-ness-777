public class PlayerCharacter : GameObject
{
    public ObservableProperty<int> Health = new ObservableProperty<int>(100);

    public Tile[,] Field { get; set; }
    private Inventory _inventory = new Inventory();

    public Inventory Inventory
    {
        get { return _inventory; }
    }

    public PlayerCharacter() => Init();

    public void Init()
    {
        Symbol = 'P';
        Health.AddListener(SetHealthGauge);
        _healthGauge = "■■■■■";
    }

    public void Update()
    {
        HandleHotkeys();
        UpdateWorldMode();
    }

    private void UpdateWorldMode()
    {
        if (InputManager.GetKey(ConsoleKey.UpArrow)) Move(Vector.Up);
        if (InputManager.GetKey(ConsoleKey.DownArrow)) Move(Vector.Down);
        if (InputManager.GetKey(ConsoleKey.LeftArrow)) Move(Vector.Left);
        if (InputManager.GetKey(ConsoleKey.RightArrow)) Move(Vector.Right);

        // 상호작용(Z)은 나중에 여기
        // if (InputManager.GetKey(ConsoleKey.Z)) Interact();
    }

    private void HandleHotkeys()
    {
        if (InputManager.GetKey(ConsoleKey.D1)) { _inventory.UseAt(0); return; }
        if (InputManager.GetKey(ConsoleKey.D2)) { _inventory.UseAt(1); return; }
        if (InputManager.GetKey(ConsoleKey.D3)) { _inventory.UseAt(2); return; }
        if (InputManager.GetKey(ConsoleKey.D4)) { _inventory.UseAt(3); return; }
        if (InputManager.GetKey(ConsoleKey.D5)) { _inventory.UseAt(4); return; }
        if (InputManager.GetKey(ConsoleKey.D6)) { _inventory.UseAt(5); return; }
    }

    private void Move(Vector direction)
    {
        if (Field == null) return;

        Vector nextPos = Position + direction;

        int height = Field.GetLength(0);
        int width = Field.GetLength(1);

        if (nextPos.Y < 0 || nextPos.Y >= height) return;
        if (nextPos.X < 0 || nextPos.X >= width) return;

        Field[Position.Y, Position.X].OnTileObject = null;
        Field[nextPos.Y, nextPos.X].OnTileObject = this;
        Position = nextPos;
    }

    public void Render()
    {
    }

    public void AddItem(Item item)
    {
        _inventory.TryAdd(item);
    }

    private string _healthGauge;

    public void DrawHealthGauge()
    {
        Console.SetCursorPosition(Position.X - 2, Position.Y - 1);
        _healthGauge.Print(ConsoleColor.Red);
    }

    public void SetHealthGauge(int health)
    {
        switch (health)
        {
            case 5:
                _healthGauge = "■■■■■";
                break;
            case 4:
                _healthGauge = "■■■■□";
                break;
            case 3:
                _healthGauge = "■■■□□";
                break;
            case 2:
                _healthGauge = "■■□□□";
                break;
            case 1:
                _healthGauge = "■□□□□";
                break;
        }
    }
}