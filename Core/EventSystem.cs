using System;
using System.Collections.Generic;
using FarmTycoon.Utils;

namespace FarmTycoon.Core
{
    public class EventSystem
    {
        private static EventSystem _instance;
        public static EventSystem Instance => _instance ??= new EventSystem();

        private readonly Dictionary<Type, List<Delegate>> _subscribers = new();

        private EventSystem() { }

        public void Subscribe<T>(Action<T> handler) where T : GameEvent
        {
            var type = typeof(T);
            if (!_subscribers.ContainsKey(type))
                _subscribers[type] = new List<Delegate>();
            _subscribers[type].Add(handler);
        }

        public void Unsubscribe<T>(Action<T> handler) where T : GameEvent
        {
            var type = typeof(T);
            if (_subscribers.ContainsKey(type))
                _subscribers[type].Remove(handler);
        }

        public void Publish<T>(T eventData) where T : GameEvent
        {
            var type = typeof(T);
            if (_subscribers.ContainsKey(type))
            {
                var handlers = new List<Delegate>(_subscribers[type]);
                foreach (var handler in handlers)
                    (handler as Action<T>)?.Invoke(eventData);
            }
        }

        public void Publish(GameEvent eventData)
        {
            var type = eventData.GetType();
            if (_subscribers.ContainsKey(type))
            {
                var handlers = new List<Delegate>(_subscribers[type]);
                foreach (var handler in handlers)
                    handler.DynamicInvoke(eventData);
            }
        }

        public void ClearAll() => _subscribers.Clear();
    }
}
