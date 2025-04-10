using System;
using System.Collections.Generic;
using System.Reflection;

namespace StealthSpectator.Extensions
{
    [Flags]
    public enum EntityEffects
    {
        None = 0,
        // Adjust the value for EF_NODRAW to match your game's implementation.
        NoDraw = 1 << 11
    }

    public static class EntityExtensions
    {
        // Cache FieldInfo objects to improve performance.
        private static readonly Dictionary<Type, FieldInfo?> FlagFieldCache = new Dictionary<Type, FieldInfo?>();
        private static readonly object CacheLock = new object();

        /// <summary>
        /// Sets or clears the specified effect flag on the given entity.
        /// </summary>
        public static void SetFlag(this object entity, EntityEffects flag, bool enabled)
        {
            if (entity == null)
                return;

            Type type = entity.GetType();
            FieldInfo? field;
            lock (CacheLock)
            {
                if (!FlagFieldCache.TryGetValue(type, out field))
                {
                    field = type.GetField("m_iEffects", BindingFlags.Instance | BindingFlags.NonPublic);
                    FlagFieldCache[type] = field;
                }
            }
            if (field == null)
                return;

            int currentFlags = (int)(field.GetValue(entity) ?? 0);
            if (enabled)
                currentFlags |= (int)flag;
            else
                currentFlags &= ~(int)flag;
            field.SetValue(entity, currentFlags);
        }

        /// <summary>
        /// Convenience method to set or clear the NoDraw flag on the entity.
        /// </summary>
        public static void SetNoDraw(this object entity, bool enabled)
        {
            entity.SetFlag(EntityEffects.NoDraw, enabled);
        }
    }
}
