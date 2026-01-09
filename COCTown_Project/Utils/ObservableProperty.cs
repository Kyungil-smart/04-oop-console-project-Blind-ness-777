using System;

public class ObservableProperty<T> where T : struct
{
    private T _value;

    public T Value
    {
        get => _value;
        set
        {
            if (_value.Equals(value)) return;

            _value = value;
            if (OnValueChanged != null)
                OnValueChanged(value);
        }
    }

    public event Action<T> OnValueChanged;

    public ObservableProperty(T value = default)
    {
        _value = value;
    }

    public void AddListener(Action<T> action)
    {
        OnValueChanged += action;
    }

    public void RemoveListener(Action<T> action)
    {
        OnValueChanged -= action;
    }
}