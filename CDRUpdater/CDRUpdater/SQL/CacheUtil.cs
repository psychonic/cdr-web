using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

namespace SteamKit2.Blob
{
    /// <summary>
    /// Provides caching for the fast construction of types
    /// </summary>
    public class CacheContext
    {
        internal Dictionary<Type, FastPropertyInfo[]> FastPropCache { get; set; }
        internal Dictionary<MemberInfo, object[]> MemberAttribMap { get; set; }

        /// <summary>
        /// Initializes the default state of the CacheContext
        /// </summary>
        public CacheContext()
        {
            FastPropCache = new Dictionary<Type, FastPropertyInfo[]>();
            MemberAttribMap = new Dictionary<MemberInfo, object[]>();
        }
    }

    /// <summary>
    /// Cache utility functions to operate on a CacheContext
    /// </summary>
    public static class CacheUtil
    {
        /// <summary>
        /// Retrieve the cached property info for a class given the CacheContext
        /// </summary>
        public static FastPropertyInfo[] GetCachedPropertyInfo(this Type t, CacheContext context)
        {
            FastPropertyInfo[] fpropinfo;

            if (context.FastPropCache.TryGetValue(t, out fpropinfo))
                return fpropinfo;

            List<FastPropertyInfo> propGen = new List<FastPropertyInfo>();

            foreach (var prop in t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                propGen.Add(new FastPropertyInfo(prop));
            }

            fpropinfo = propGen.ToArray();
            context.FastPropCache.Add(t, fpropinfo);
            return fpropinfo;
        }

        /// <summary>
        /// Retrieve the cached list of custom attributes for a member
        /// </summary>
        public static object[] GetCachedCustomAttribs(CacheContext context, MemberInfo mi, Type x)
        {
            object[] attribs;

            if (context.MemberAttribMap.TryGetValue(mi, out attribs))
                return attribs;

            attribs = mi.GetCustomAttributes(x, false);
            context.MemberAttribMap.Add(mi, attribs);

            return attribs;
        }

        /// <summary>
        /// Retrieve the cached attribute for a member 
        /// </summary>
        public static T GetAttribute<T>(this MemberInfo mi, CacheContext context)
            where T : Attribute
        {
            T[] attribs = (T[])GetCachedCustomAttribs(context, mi, typeof(T));

            if (attribs == null || attribs.Length == 0)
                return null;

            return attribs[0];
        }
    }
}
