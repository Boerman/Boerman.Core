using System;
using System.Collections.Concurrent;
using Boerman.Core.Extensions;
using Boerman.Core.Reflection;

namespace Boerman.Core
{
    internal class SingletonManager
    {
        private static readonly Lazy<SingletonManager> Lazy = new Lazy<SingletonManager>(() => new SingletonManager());
        public static SingletonManager Instance => Lazy.Value;
        

        readonly ConcurrentDictionary<Type, object> _objects = new ConcurrentDictionary<Type, object>();

        public T GetInstance<T>() where T : class
        {
            if (_objects.ContainsKey(typeof(T)))
                return _objects[typeof(T)] as T;

            var instance = typeof(T).CreateInstance<T>();
            if (instance != null && _objects.TryAdd(typeof(T), instance)) return instance;

            return null;
        }
    }

    // Please note you can shoot yer foot off with this thing
    public static class Singleton<T> where T : class
    {
        public static T Instance => SingletonManager.Instance.GetInstance<T>();
    }
}
