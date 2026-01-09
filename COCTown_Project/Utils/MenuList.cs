using System;
using System.Collections.Generic;

public class MenuList
{
    private List<(string text, Action action)> _menus;
    private int _currentIndex;
    
    public MenuList(params (string, Action)[] menuTexts)
    {
        _menus = new List<(string, Action)>();

        if (menuTexts != null && menuTexts.Length > 0)
        {
            for (int i = 0; i < menuTexts.Length; i++)
                _menus.Add(menuTexts[i]);
        }
    }

    public void Reset()
    {
        _currentIndex = 0;
    }

    public void Select()
    {
        if (_menus.Count == 0) return;

        _menus[_currentIndex].action?.Invoke();

        if (_menus.Count == 0) _currentIndex = 0;
        else if (_currentIndex >= _menus.Count) _currentIndex = _menus.Count - 1;
    }

    public void Add(string text, Action action)
    {
        _menus.Add((text, action));
    }

    public void SelectUp()
    {
        _currentIndex--;

        if (_currentIndex < 0)
            _currentIndex = 0;
    }

    public void SelectDown()
    {
        _currentIndex++;

        if (_currentIndex >= _menus.Count)
            _currentIndex = _menus.Count - 1;
    }

    public void Render(int x, int y)
    {
        for (int i = 0; i < _menus.Count; i++)
        {
            Console.SetCursorPosition(x, y + i);

            if (i == _currentIndex)
                Console.Write("-> " + _menus[i].text);
            else
                Console.Write("   " + _menus[i].text);
        }
    }
}