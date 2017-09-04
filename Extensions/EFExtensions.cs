using System.Data.Entity;
using System.Reflection;

namespace Boerman.Core.Extensions
{
    public static class EFExtensions
    {
        // See http://stackoverflow.com/a/17712575/1720761
        public static DbContext GetContext<TEntity>(this DbSet<TEntity> dbSet)
        where TEntity : class
        {
            object internalSet = dbSet
                .GetType()
                .GetField("_internalSet", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(dbSet);
            object internalContext = internalSet
                .GetType()
                .BaseType
                .GetField("_internalContext", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(internalSet);
            return (DbContext)internalContext
                .GetType()
                .GetProperty("Owner", BindingFlags.Instance | BindingFlags.Public)
                .GetValue(internalContext, null);
        }
    }
}
