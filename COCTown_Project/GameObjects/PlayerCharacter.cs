using System;

public class PlayerCharacter : GameObject
{
    public ObservableProperty<int> Sanity = new ObservableProperty<int>(5);

    public Tile[,] Field { get; set; }
    private Inventory _inventory = new Inventory();

    public Inventory Inventory
    {
        get { return _inventory; }
    }

    public PlayerCharacter() => Init();

    public void Init()
    {
        // 플레이어 심볼(혼합 모드): ●
        Symbol = '●';
        Sanity.AddListener(SetSanityGauge);
        _sanityGauge = "●●●●●";
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

    private string _sanityGauge;

    public void DrawSanityGauge()
    {
        Console.SetCursorPosition(0, 0);
        "정신력 : ".Print();
        _sanityGauge.Print(ConsoleColor.Cyan);
        Console.Write("   ");
    }

    public void SetSanityGauge(int sanity)
    {
        switch (sanity)
        {
            case 5:
                _sanityGauge = "●●●●●";
                break;
            case 4:
                _sanityGauge = "●●●●○";
                break;
            case 3:
                _sanityGauge = "●●●○○";
                break;
            case 2:
                _sanityGauge = "●●○○○";
                break;
            case 1:
                _sanityGauge = "●○○○○";
                break;
            case 0:
                _sanityGauge = "○○○○○";
                break;
            default:
                if (sanity > 5) _sanityGauge = "●●●●●";
                else _sanityGauge = "○○○○○";
                break;
        }
    }

    public void DecreaseSanity(int amount)
    {
        if (amount <= 0) return;

        int next = Sanity.Value - amount;
        if (next < 0) next = 0;

        Sanity.Value = next;
    }

    public void IncreaseSanity(int amount)
    {
        if (amount <= 0) return;

        int next = Sanity.Value + amount;
        if (next > 5) next = 5;

        Sanity.Value = next;
    }

    public bool IsDead()
    {
        return Sanity.Value <= 0;
    }

}