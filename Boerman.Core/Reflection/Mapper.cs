using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Boerman.Core.Reflection
{
    public static class Mapper<T> where T : class
    {
        public static T Map(object obj)
        {
            dynamic instance = Activator.CreateInstance<T>();

            Map(ref instance, obj);

            return (T)instance;
        }

        private static void Map(ref dynamic instance, object obj)
        {
            var destination = instance.GetType();
            var source = obj.GetType();

            foreach (PropertyInfo property in destination.GetProperties())
            {
                PropertyInfo sourceProp = source.GetProperty(property.Name);
                if (sourceProp != null)
                {
                    // The moment the object cannot be converted, which most of the times means a different data time is used in one of the objects, try to convert child items.

                    try
                    {
                        property.SetValue(instance, source.GetProperty(property.Name).GetValue(obj));
                    }
                    catch
                    {
                        try
                        {
                            var sp = sourceProp.GetValue(obj);
                            Type t = Type.GetType(property.PropertyType.AssemblyQualifiedName);

                            // If the source is a list, do some alternative conversion
                            var sourceList = sp as IEnumerable;
                            if (sourceList != null)
                            {
                                // If the source has the type of an ICollection we can almost be sure the destination also has the type of an ICollection.
                                var destinationList =
                                    Activator.CreateInstance(
                                        typeof(Collection<>).MakeGenericType(
                                            t.GenericTypeArguments[0]));

                                foreach (var sourceItem in sourceList)
                                {
                                    destinationList.GetType().GetMethod("Add").Invoke(destinationList, new[]
                                    {
                                        typeof (Mapper<>).MakeGenericType(t.GenericTypeArguments[0])
                                            .GetMethod("Map")
                                            .Invoke(null, new[] {sourceItem})
                                    });
                                }

                                property.SetValue(instance, destinationList);
                            }
                            else
                            {
                                // sp isn't an ICollection
                                object notherinstance = Activator.CreateInstance(t);

                                Map(ref notherinstance, sp);

                                property.SetValue(instance, notherinstance); // Do some conversion here
                            }
                        }
                        catch
                        {
                            // When some error is catched it maybe means the property value in the source object is null.

                            // Well, there's so much which could've gone wrong when you're at this point...
                            throw;
                        }
                    }
                }
            }
        }
    }
}
