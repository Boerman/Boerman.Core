using System;
using System.Linq;

namespace Boerman.Core.Reflection
{
    public static class DynamicClassInvocator
    {
        public static object InvocateInterfaceImplementation<I>(string implementationName) where I : class
        {
            // ToDo: Test the code

            var types =
                AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(p => typeof(I).IsAssignableFrom(p) && !p.IsInterface);

            return (from type in types where typeof(I).Name == implementationName select (I)Activator.CreateInstance(type)).FirstOrDefault();
        }
    }
}
