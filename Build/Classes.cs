﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Linq;

namespace Build
{
    class RuntimeType
    {
        bool _init;
        bool _guard;
        object _instance;
        Func<object> _func;
        RuntimeInstance _runtime;
        Type _type;
        string _id;
        public string Id { get { return _id; } }
        public RuntimeType(string id) => _id = id;
        public void Initialize(RuntimeInstance runtime, string id, Type type, Func<object> func)
        {
            if (_init)
                throw new Exception(string.Format("{0} is amibiguous for initialization, (more than one constructors available)", Id));
            _runtime = runtime;
            _type = type;
            _id = _type.FullName;
            _func = func;
            _init = true;
        }
        public object CreateInstance()
        {
            object Evaluate()
            {
                if (_guard)
                    throw new Exception(string.Format("{0} is evaluated", Id));
                _guard = true;
                object result = _func();
                _guard = false;
                return result;
            }
            switch (_runtime)
            {
                case RuntimeInstance.Singleton:
                    if (!_guard)
                    {
                        _instance = Evaluate();
                        _guard = true;
                    }
                    return _instance;
                case RuntimeInstance.CreateInstance:
                    return Evaluate();
                case RuntimeInstance.None:
                default:
                    if (_func != null)
                        throw new Exception(string.Format("{0} is not allowed to evaluated", Id));
                    return _instance;
            }
        }
    }
    class TypeBuilder
    {
        IDictionary<string, RuntimeType> types = new Dictionary<string, RuntimeType>();
        RuntimeType this[string type]
        {
            get
            {
                if (!types.ContainsKey(type))
                    types.Add(type, new RuntimeType(type));
                return types[type];
            }
        }
        public object CreateInstance(Type type)
        {
            object CreateInstance(string id)
            {
                if (types.ContainsKey(id))
                    return types[id].CreateInstance();
                throw new Exception(string.Format("{0} is not registered as constructible type (no constructors available)", id));
            }
            var classAttribute = type.GetCustomAttribute<DependencyAttribute>();
            if (classAttribute != null)
            {
                if (classAttribute.Id != null)
                    return CreateInstance(classAttribute.Id);
                if (classAttribute.Type != null)
                    return CreateInstance(classAttribute.Type.FullName);
            }
            return CreateInstance(type.FullName);
        }
        public void RegisterType(Type type)
        {
            string GetTypeId(IRuntimeAttribute attribute, string defaultValue)
            {
                if (attribute != null)
                {
                    var instanceType = attribute.Type;
                    if (instanceType != null)
                        return instanceType.FullName;
                    else
                    {
                        if (attribute.Id != null)
                            return attribute.Id;
                        return defaultValue;
                    }
                }
                else
                    return defaultValue;
            }
            void Initialize(ConstructorInfo constructor, List<RuntimeType> args)
            {
                RuntimeInstance runtimeInstance = RuntimeInstance.CreateInstance;
                var attribute = constructor.GetCustomAttribute<DependencyAttribute>();
                if (attribute == null)
                    attribute = type.GetCustomAttribute<DependencyAttribute>();
                string typeId = GetTypeId(attribute, type.FullName);
                Type attributeType = type.Assembly.GetType(typeId);
                if (attributeType != null && !attributeType.IsAssignableFrom(type))
                    throw new Exception(string.Format("{0} is not assignable from {1}", attributeType.FullName, type.FullName));
                if (attribute != null && attribute.Runtime != runtimeInstance)
                    runtimeInstance = attribute.Runtime;
                object init()
                {
                    Debug.WriteLine("{0}({1})", type.FullName, string.Join(",", args.Select(p => p.Id)));
                    return Activator.CreateInstance(type, args.Select(p => p.CreateInstance()).ToArray());
                }
                this[typeId].Initialize(runtimeInstance, typeId, type, init);
            }
            var constructors = type.GetConstructors();
            if (constructors.Length == 0)
            {
                throw new Exception(string.Format("{0} is not registered as constructible type (no constructors available)", type.FullName));
            }
            foreach (var constructor in constructors)
            {
                var parameters = constructor.GetParameters().ToList();
                var args = new List<RuntimeType>();
                Initialize(constructor, args);
                foreach (var parameterInfo in parameters)
                {
                    var attribute = parameterInfo.GetCustomAttribute<InjectionAttribute>();
                    var parameterType = parameterInfo.ParameterType;
                    string typeId = GetTypeId(attribute, parameterType.FullName);
                    Type attributeType = type.Assembly.GetType(typeId);
                    if (attributeType != null && !parameterType.IsAssignableFrom(attributeType))
                        throw new Exception(string.Format("{0} is not assignable from {1}", parameterType.FullName, attributeType.FullName));
                    if (typeId == type.FullName && typeId == parameterType.FullName)
                        throw new Exception(string.Format("{0} is self-referenced from parameter {1}", type.FullName, parameterInfo.Name));
                    args.Add(this[typeId]);
                }
            }
        }
    }
    public interface IContainer
    {
        T CreateInstance<T>();
        void RegisterType<T>();
    }
    public class Container : IContainer
    {
        bool _createFilter(Type type) => type.IsPublic;
        bool _registerFilter(Type type) =>
            !type.IsInterface && !type.IsAbstract && !type.IsValueType && !type.IsGenericType &&
            !typeof(Attribute).IsAssignableFrom(type) && !typeof(MarshalByRefObject).IsAssignableFrom(type) &&
            _createFilter(type);

        TypeBuilder typeBuilder = new TypeBuilder();
        public T CreateInstance<T>()
        {
            if (!_createFilter(typeof(T)))
                throw new Exception(string.Format("{0} is not allowed", typeof(T).FullName));
            return (T)typeBuilder.CreateInstance(typeof(T));
        }
        public void RegisterType<T>()
        {
            if (!_registerFilter(typeof(T)))
                throw new Exception(string.Format("{0} is not allowed", typeof(T).FullName));
            typeBuilder.RegisterType(typeof(T));
        }
        public void RegisterAssemblyTypes(Assembly assembly)
        {
            if (assembly == null)
                throw new Exception(nameof(assembly));
            foreach (var type in assembly.GetTypes())
                if (_registerFilter(type))
                    typeBuilder.RegisterType(type);
        }
    }
}
