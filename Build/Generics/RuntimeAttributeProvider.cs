﻿using System;
using System.Reflection;

namespace Build.Generics
{
    public abstract class RuntimeAttributeProvider<T>: IRuntimeAttributeProvider<T>,
        IRuntimeAttributeProvider<T, Module>,
        IRuntimeAttributeProvider<T, Assembly>,
        IRuntimeAttributeProvider<T, ParameterInfo>,
        IRuntimeAttributeProvider<T, PropertyInfo>,
        IRuntimeAttributeProvider<T, MethodInfo>,
        IRuntimeAttributeProvider<T, ConstructorInfo>,
        IRuntimeAttributeProvider<T, MemberInfo>,
        IRuntimeAttributeProvider<T, ICustomAttributeProvider>
        where T : Attribute, IRuntimeAttribute
    {
        public virtual T GetAttribute(Module p, Type type, bool inherit = false) => p.GetAttribute<T>(type, inherit);
        public virtual T GetAttribute(Assembly p, Type type, bool inherit = false) => p.GetAttribute<T>(type, inherit);
        public virtual T GetAttribute(ParameterInfo p, Type type, bool inherit = false) => p.GetAttribute<T>(type, inherit);
        public virtual T GetAttribute(PropertyInfo p, Type type, bool inherit = false) => p.GetAttribute<T>(type, inherit);
        public virtual T GetAttribute(MethodInfo p, Type type, bool inherit = false) => p.GetAttribute<T>(type, inherit);
        public virtual T GetAttribute(ConstructorInfo p, Type type, bool inherit = false) => p.GetAttribute<T>(type, inherit);
        public virtual T GetAttribute(MemberInfo p, Type type, bool inherit = false) => p.GetAttribute<T>(type, inherit);
        public virtual T GetAttribute(ICustomAttributeProvider p, Type type, bool inherit = false) => p.GetAttribute<T>(typeof(T), inherit);
    }
}