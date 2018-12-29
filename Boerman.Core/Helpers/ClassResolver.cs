/*
 * Please note this code isn't really tested or whatever
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Boerman.Core.Extensions;

namespace Boerman.Core.Helpers
{
    public static class ClassResolver
    {
        public static IEnumerable<Type> Resolve(string name)
        {
            return
                ReflectionExtensions
                    .GetTypesWithAttribute(typeof(ClassResolverAttribute))
                    .Select(type => new
                        {
                            type,
                            classResolverAttribute = (ClassResolverAttribute) type.GetCustomAttribute(typeof(ClassResolverAttribute))
                        })
                    .Where(q => q.classResolverAttribute.Name == name)
                    .Select(q => q.type);
        }

        public static IEnumerable<T> Resolve<T>(string name, params object[] parameters) where T : class
        {
            return typeof(T)
                .FindImplementations()
                .GetTypesWithAttribute(typeof(ClassResolverAttribute))
                .Select(type => new
                {
                    type,
                    classResolverAttribute =
                    (ClassResolverAttribute) type.GetCustomAttribute(typeof(ClassResolverAttribute))
                })
                .Where(q => q.classResolverAttribute.Name == name)
                .Select(q => q.type.CreateInstance<T>(parameters));
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ClassResolverAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
