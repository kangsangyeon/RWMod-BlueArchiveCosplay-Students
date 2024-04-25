using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;

public class JsonSerializationBinder : ISerializationBinder
{
    internal static readonly JsonSerializationBinder Instance = new JsonSerializationBinder();

    private readonly JsonSerializationBinder.ThreadSafeStore<JsonSerializationBinder.TypeNameKey, Type> typeCache =
        new JsonSerializationBinder.ThreadSafeStore<JsonSerializationBinder.TypeNameKey, Type>(
            new Func<JsonSerializationBinder.TypeNameKey, Type>(JsonSerializationBinder.GetTypeFromTypeNameKey));

    private static Type GetTypeFromTypeNameKey(JsonSerializationBinder.TypeNameKey _typeNameKey)
    {
        string _assemblyName = _typeNameKey.assemblyName;
        string _typeName = _typeNameKey.typeName;
        if (_assemblyName != null)
        {
            //Assembly assembly = Assembly.LoadWithPartialName(assemblyName);
            Assembly _assembly = Assembly.Load(_assemblyName);
            return (_assembly != null
                       ? _assembly.GetType(_typeName)
                       : throw new JsonSerializationException(string.Format("Could not load assembly '{0}'.", (object)_assemblyName))) ??
                   throw new JsonSerializationException(string.Format("Could not find type '{0}' in assembly '{1}'.", (object)_typeName, (object)_assembly.FullName));
        }

        foreach (Assembly _assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (Type _type in _assembly.GetTypes())
            {
                if (_type.Name == _typeName)
                    return _type;
            }
        }

        return (Type)null;
    }

    public Type BindToType(string _assemblyName, string _typeName) => this.typeCache.Get(new JsonSerializationBinder.TypeNameKey(_assemblyName, _typeName));

    public void BindToName(Type _serializedType, out string _assemblyName, out string _typeName)
    {
        _assemblyName = _serializedType.Assembly.FullName;
        _typeName = _serializedType.FullName;
    }

    internal class ThreadSafeStore<TKey, TValue>
    {
        private readonly object @lock = new object();
        private Dictionary<TKey, TValue> store;
        private readonly Func<TKey, TValue> creator;

        public ThreadSafeStore(Func<TKey, TValue> _creator) => this.creator = _creator != null ? _creator : throw new ArgumentNullException(nameof(_creator));

        public TValue Get(TKey _key)
        {
            TValue _obj;
            return this.store == null || !this.store.TryGetValue(_key, out _obj) ? this.AddValue(_key) : _obj;
        }

        private TValue AddValue(TKey _key)
        {
            TValue _obj1 = this.creator(_key);
            lock (this.@lock)
            {
                if (this.store == null)
                {
                    this.store = new Dictionary<TKey, TValue>();
                    this.store[_key] = _obj1;
                }
                else
                {
                    TValue _obj2;
                    if (this.store.TryGetValue(_key, out _obj2))
                        return _obj2;
                    this.store = new Dictionary<TKey, TValue>((IDictionary<TKey, TValue>)this.store)
                    {
                        [_key] = _obj1
                    };
                }

                return _obj1;
            }
        }
    }

    internal struct TypeNameKey
    {
        internal readonly string assemblyName;
        internal readonly string typeName;

        public TypeNameKey(string _assemblyName, string _typeName)
        {
            this.assemblyName = _assemblyName;
            this.typeName = _typeName;
        }

        public override int GetHashCode() => (this.assemblyName != null ? this.assemblyName.GetHashCode() : 0) ^ (this.typeName != null ? this.typeName.GetHashCode() : 0);

        public override bool Equals(object _obj) => _obj is JsonSerializationBinder.TypeNameKey _other && this.Equals(_other);

        public bool Equals(JsonSerializationBinder.TypeNameKey _other) => this.assemblyName == _other.assemblyName && this.typeName == _other.typeName;
    }
}