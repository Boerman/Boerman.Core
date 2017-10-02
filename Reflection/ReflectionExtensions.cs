using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace Boerman.Core.Reflection
{
    public static class ReflectionExtensions
    {
        public static IEnumerable<Type> FindImplementations<T>()
        {
            return
                AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(q => q.GetTypes())
                    .Where(q => q.IsAssignableFrom(typeof(T)))
                    .Where(q => !q.IsInterface);
        }

        public static IEnumerable<Type> FindImplementations(this Type type)
        {
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(q => q.GetTypes())
                .Where(q => q.IsAssignableFrom(type))
                .Where(q => !q.IsInterface);
        }

        public static IEnumerable<Type> FindExportedDerivedTypes(this Type type)
        {
            return FindDerivedTypes(type, true);
        }

        public static IEnumerable<Type> FindDerivedTypes(this Type type)
        {
            return FindDerivedTypes(type, false);
        }

        private static IEnumerable<Type> FindDerivedTypes(Type type, bool isExportedType)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            
            var types = isExportedType
                ? assemblies.SelectMany(q => q.ExportedTypes)
                : assemblies.SelectMany(q => q.DefinedTypes);

            return types.Where(q => 
                q.IsSubclassOf(type) 
                && q != type 
                && !q.IsAbstract);
        }

        [Obsolete("Use the `FindDerivedTypes` extension method because it searches through the AppDomain")]
        public static IEnumerable<Type> FindSubclasses(this Type type)
        {
            return
                Assembly.GetAssembly(type)
                    .GetTypes()
                    .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(type));
        }

        #region Instantiation
        /// <summary>
        /// Creates a new instance of the provided type and casts it to the generic type provided.
        /// </summary>
        /// <typeparam name="T">The (generic) return type</typeparam>
        /// <param name="type">The type to create an instance of</param>
        /// <returns>An instantiated type</returns>
        public static T CreateInstance<T>(this Type type)
        {
            // ToDo: Add checks to see whether we can cast this type correctly
            return (T) type.CreateInstance();
        }

        /// <summary>
        /// Creates a new instance of the provided type and returns the object.
        /// </summary>
        /// <param name="type">The type to create an instance of</param>
        /// <returns>An instantiated type</returns>
        public static object CreateInstance(this Type type)
        {
            return Activator.CreateInstance(type);
        }

        public static bool HasPArameterlessConstructor(this Type type)
        {
            ConstructorInfo cInfo = type.GetConstructor(null);

            return cInfo != null;
        }

        /// <summary>
        /// Creates a new instance of the provided type with the provided parameters as constructor parameters and returns the object.
        /// </summary>
        /// <param name="type">The type to create an instance of</param>
        /// <param name="parameters">The parameter to use in the constructor of the provided type</param>
        /// <returns>An instantiated type</returns>
        public static object CreateInstance(this Type type, params object[] parameters)
        {
            ConstructorInfo cInfo = type.GetConstructor(parameters.Select(x => x.GetType()).ToArray());

            if (cInfo == null) throw new InvalidOperationException("Constructor does not exist");

            var returnValue = cInfo.Invoke(parameters);

            return returnValue;
        }
        
        /// <summary>
        /// Creates a new instance of the provided type with the provided parameters as constructor parameters and returns the object which is casted to the generic type provided.
        /// </summary>
        /// <typeparam name="T">The (generic) return type</typeparam>
        /// <param name="type">The type to create an instance of</param>
        /// <param name="parameters">The parameter to use in the constructor of the provided type</param>
        /// <returns>An instantiated type</returns>
        public static T CreateInstance<T>(this Type type, params object[] parameters) where T : class
        {
            // ToDo: Add checks to see whether we can cast this type correctly
            return type.CreateInstance(parameters) as T;
        }

        /// <summary>
        /// Creates a new instance of the provided generic type and uses the provied parameters in the constructor.
        /// </summary>
        /// <typeparam name="T">The type you want instantiated</typeparam>
        /// <param name="parameters">The parameters to use in the constructor</param>
        /// <returns>An instantiated object of the provided (generic) type</returns>
        public static T CreateInstance<T>(params object[] parameters)
        {
            ConstructorInfo cInfo = typeof(T).GetConstructor(parameters.Select(x => x.GetType()).ToArray());

            if (cInfo == null) throw new InvalidOperationException("Constructor does not exist");

            var returnValue = (T)cInfo.Invoke(parameters);

            return returnValue;
        }
        #endregion

        public static bool IsValueType(this object obj)
        {
            return obj != null && obj.GetType().IsValueType;
        }

        public static bool IsPrimitiveType(this object obj)
        {
            return obj.GetType().IsPrimitive;
        }

        public static bool IsPrimitiveType(this PropertyInfo propertyInfo)
        {
            return propertyInfo.PropertyType.IsPrimitive;
        }

        public static bool Implements(this Type t1, Type t2)
        {
            return t2.IsAssignableFrom(t1);
        }

        public static IEnumerable<Type> GetTypesWithAttribute(this IEnumerable<Type> types, Type attribute)
        {
            if (!attribute.Implements(typeof(Attribute))) throw new Exception("provided type is not assignable to `Attribute`");
            
            foreach (var type in types)
            {
                if (type.GetCustomAttributes(attribute, true).Length > 0)
                {
                    yield return type;
                }
            }
        }

        public static IEnumerable<Type> GetTypesWithAttribute(Type attribute)
        {
            if (!attribute.Implements(typeof(Attribute))) throw new Exception("provided type is not assignable to `Attribute`");

            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(q => q.GetTypes())
                .GetTypesWithAttribute(attribute);
        }

        public static ExpandoObject ToExpandoObject(this object obj)
        {
            var ed = new ExpandoObject() as IDictionary<string, Object>;

            var properties = obj.GetType().GetProperties();

            foreach (var property in properties)
            {
                ed[property.Name] = property.GetValue(obj);
            }

            return (ExpandoObject)ed;
        }

        public static void AddAtribute<T>(this T obj, Attribute attributeType)
        {
            TypeDescriptor.AddAttributes(obj, attributeType);
        }
    }
}
