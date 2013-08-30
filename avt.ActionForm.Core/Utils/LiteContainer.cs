using System;
using System.Collections.Generic;

namespace avt.ActionForm.Utils
{
    public class LiteContainer
    {
        Dictionary<Type, object> _Services = new Dictionary<Type, object>();
        Dictionary<Type, object> _LazyServices = new Dictionary<Type, object>();

        public void RegisterService<T>(T service)
           where T : class
        {
            lock (typeof(LiteContainer)) {
                if (!typeof(T).IsInterface)
                    throw new ArgumentException("Service must be an interface!");

                _Services[typeof(T)] = service;
            }
        }

        public void RegisterService<T>(Func<T> service)
           where T : class
        {
            lock (typeof(LiteContainer)) {
                if (!typeof(T).IsInterface)
                    throw new ArgumentException("Service must be an interface!");

                _LazyServices[typeof(T)] = service;
            }
        }

        public T ResolveService<T>()
            where T :class
        {
            lock (typeof(LiteContainer)) {
                if (!typeof(T).IsInterface)
                    throw new ArgumentException("Service must be an interface!");

                if (_Services.ContainsKey(typeof(T)))
                    return (T)_Services[typeof(T)];

                // also check lazy loaded services
                if (_LazyServices.ContainsKey(typeof(T))) {
                    _Services[typeof(T)] = (_LazyServices[typeof(T)] as Func<T>)();
                    return (T)_Services[typeof(T)];
                }

                // TODO: maybe throw exception
                return null;
            }
        }


        Dictionary<string, object> _Properties = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        public void RegisterProperty(string key, Func<object> property)
        {
            lock (typeof(LiteContainer)) {
                _Properties[key] = property;
            }
        }

        public bool HasProperty(string name)
        {
            lock (typeof(LiteContainer)) {
                return _Properties.ContainsKey(name);
            }
        }

        public object ResolveProperty(string key)
        {
            lock (typeof(LiteContainer)) {
                if (_Properties.ContainsKey(key)) {
                    return (_Properties[key] as Func<object>)();
                }

                // TODO: maybe throw exception
                return null;
            }
        }
    }
}
