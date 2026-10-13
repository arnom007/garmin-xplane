using System;

namespace RoarClient.Net.Native;

[AttributeUsage(AttributeTargets.Delegate, AllowMultiple = false)]
internal class NativeClientIgnoreAttribute : Attribute
{
}
