using System;

namespace Nomlas.TopazChat
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class UnityEventAttribute : Attribute { }
}