using System;

namespace RGSK
{
    public class GameEvent
    {
        event Action _action = delegate { };

        public void Invoke()
        {
            _action?.Invoke();
        }

        public void AddListener(Action a)
        {
            _action += a;
        }

        public void RemoveListener(Action a)
        {
            _action -= a;
        }
    }

    public class GameEvent<T>
    {
        event Action<T> _action = delegate { };

        public void Invoke(T param)
        {
            _action?.Invoke(param);
        }

        public void AddListener(Action<T> a)
        {
            _action += a;
        }

        public void RemoveListener(Action<T> a)
        {
            _action -= a;
        }
    }

    public class GameEvent<T1, T2>
    {
        event Action<T1, T2> _action = delegate { };

        public void Invoke(T1 param1, T2 param2)
        {
            _action?.Invoke(param1, param2);
        }

        public void AddListener(Action<T1, T2> a)
        {
            _action += a;
        }

        public void RemoveListener(Action<T1, T2> a)
        {
            _action -= a;
        }
    }

    public class GameEvent<T1, T2, T3>
    {
        event Action<T1, T2, T3> _action = delegate { };

        public void Invoke(T1 param1, T2 param2, T3 param3)
        {
            _action?.Invoke(param1, param2, param3);
        }

        public void AddListener(Action<T1, T2, T3> a)
        {
            _action += a;
        }

        public void RemoveListener(Action<T1, T2, T3> a)
        {
            _action -= a;
        }
    }

    public class GameEvent<T1, T2, T3, T4>
    {
        event Action<T1, T2, T3, T4> _action = delegate { };

        public void Invoke(T1 param1, T2 param2, T3 param3, T4 param4)
        {
            _action?.Invoke(param1, param2, param3, param4);
        }

        public void AddListener(Action<T1, T2, T3, T4> a)
        {
            _action += a;
        }

        public void RemoveListener(Action<T1, T2, T3, T4> a)
        {
            _action -= a;
        }
    }
}