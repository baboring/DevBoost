﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using System.ComponentModel;
using System;
using System.Reflection;


namespace DevBoost
{
    public static class TransformExtention
    {
        public static Transform Clear(this Transform transform)
        {
            foreach (Transform child in transform)
            {
                if (child.gameObject.activeSelf)
                    GameObject.Destroy(child.gameObject);
            }
            return transform;
        }
    }

    public static class VectorExtention
    {
        public static string ToStringFull(this Vector3 target)
        {
            return string.Format("{0},{1},{2}", target.x, target.y, target.z);
        }

        #region Vector3
        public static Vector3 RandomX(this Vector3 target, float min, float max)
        {
            target.x = UnityEngine.Random.Range(min, max);
            return target;
        }
        public static Vector3 RandomY(this Vector3 target, float min, float max)
        {
            target.y = UnityEngine.Random.Range(min, max);
            return target;
        }
        public static Vector3 RandomZ(this Vector3 target, float min, float max)
        {
            target.z = UnityEngine.Random.Range(min, max);
            return target;
        }
        public static Vector3 RandomXZ(this Vector3 target, float min, float max)
        {
            target.x = UnityEngine.Random.Range(min, max);
            target.z = UnityEngine.Random.Range(min, max);
            return target;
        }
        #endregion
    }

    public static class RectTransformExtensions
    {
        public static void SetDefaultScale(this RectTransform trans)
        {
            trans.localScale = new Vector3(1, 1, 1);
        }
        public static void SetPivotAndAnchors(this RectTransform trans, Vector2 aVec)
        {
            trans.pivot = aVec;
            trans.anchorMin = aVec;
            trans.anchorMax = aVec;
        }

        public static Vector2 GetSize(this RectTransform trans)
        {
            return trans.rect.size;
        }
        public static float GetWidth(this RectTransform trans)
        {
            return trans.rect.width;
        }
        public static float GetHeight(this RectTransform trans)
        {
            return trans.rect.height;
        }

        public static void SetPositionOfPivot(this RectTransform trans, Vector2 newPos)
        {
            trans.localPosition = new Vector3(newPos.x, newPos.y, trans.localPosition.z);
        }

        public static void SetLeftBottomPosition(this RectTransform trans, Vector2 newPos)
        {
            trans.localPosition = new Vector3(newPos.x + (trans.pivot.x * trans.rect.width), newPos.y + (trans.pivot.y * trans.rect.height), trans.localPosition.z);
        }
        public static void SetLeftTopPosition(this RectTransform trans, Vector2 newPos)
        {
            trans.localPosition = new Vector3(newPos.x + (trans.pivot.x * trans.rect.width), newPos.y - ((1f - trans.pivot.y) * trans.rect.height), trans.localPosition.z);
        }
        public static void SetRightBottomPosition(this RectTransform trans, Vector2 newPos)
        {
            trans.localPosition = new Vector3(newPos.x - ((1f - trans.pivot.x) * trans.rect.width), newPos.y + (trans.pivot.y * trans.rect.height), trans.localPosition.z);
        }
        public static void SetRightTopPosition(this RectTransform trans, Vector2 newPos)
        {
            trans.localPosition = new Vector3(newPos.x - ((1f - trans.pivot.x) * trans.rect.width), newPos.y - ((1f - trans.pivot.y) * trans.rect.height), trans.localPosition.z);
        }

        public static void SetSize(this RectTransform trans, Vector2 newSize)
        {
            Vector2 oldSize = trans.rect.size;
            Vector2 deltaSize = newSize - oldSize;
            trans.offsetMin = trans.offsetMin - new Vector2(deltaSize.x * trans.pivot.x, deltaSize.y * trans.pivot.y);
            trans.offsetMax = trans.offsetMax + new Vector2(deltaSize.x * (1f - trans.pivot.x), deltaSize.y * (1f - trans.pivot.y));
        }
        public static void SetWidth(this RectTransform trans, float newSize)
        {
            SetSize(trans, new Vector2(newSize, trans.rect.size.y));
        }
        public static void SetHeight(this RectTransform trans, float newSize)
        {
            SetSize(trans, new Vector2(trans.rect.size.x, newSize));
        }

    }

    public static class StringExtensions
    {
        public static bool StartsWithAny(this string s, IEnumerable<string> items)
        {
            return items.Any(i => s.StartsWith(i));
        }
    }

    public static class GuidExtensions
    {
        #region Guid
        public static ulong ToUInt64(this System.Guid target)
        {
            byte[] guidAsBytes = target.ToByteArray();
            return System.BitConverter.ToUInt64(guidAsBytes, 0);
        }

        #endregion
    }

    public class IosCompatible
    {
        public static Int32Converter i32 = new Int32Converter();
        public static Int64Converter i64 = new Int64Converter();
        public static BooleanConverter booleanConv = new BooleanConverter();
        public static StringConverter stringConv = new StringConverter();
        public static SingleConverter singleConv = new SingleConverter();
    }


    /// <summary>
    /// [ <c>public static object GetDefault(this Type type)</c> ]
    /// <para></para>
    /// Retrieves the default value for a given Type
    /// </summary>
    /// <param name="type">The Type for which to get the default value</param>
    /// <returns>The default value for <paramref name="type"/></returns>
    /// <remarks>
    /// If a null Type, a reference Type, or a System.Void Type is supplied, this method always returns null.  If a value type 
    /// is supplied which is not publicly visible or which contains generic parameters, this method will fail with an 
    /// exception.
    /// </remarks>
    /// <example>
    /// To use this method in its native, non-extension form, make a call like:
    /// <code>
    ///     object Default = DefaultValue.GetDefault(someType);
    /// </code>
    /// To use this method in its Type-extension form, make a call like:
    /// <code>
    ///     object Default = someType.GetDefault();
    /// </code>
    /// </example>
    /// <seealso cref="GetDefault();"/>

    public static class DefaultExtention
    {

        public static object Default(Type maybeNullable)
        {
            Type underlying = Nullable.GetUnderlyingType(maybeNullable);
            if (underlying != null)
                return Activator.CreateInstance(underlying);
            return Activator.CreateInstance(maybeNullable);
        }

        public static object GetDefault(this Type type)
        {
            // If no Type was supplied, if the Type was a reference type, or if the Type was a System.Void, return null
            if (type == null || !type.IsValueType || type == typeof(void))
                return null;

            // If the supplied Type has generic parameters, its default value cannot be determined
            if (type.ContainsGenericParameters)
                throw new ArgumentException(
                    "{" + MethodInfo.GetCurrentMethod() + "} Error:\n\nThe supplied value type <" + type +
                    "> contains generic parameters, so the default value cannot be retrieved");

            // If the Type is a primitive type, or if it is another publicly-visible value type (i.e. struct/enum), return a 
            //  default instance of the value type
            if (type.IsPrimitive || !type.IsNotPublic)
            {
                try
                {
                    return Activator.CreateInstance(type);
                }
                catch (Exception e)
                {
                    throw new ArgumentException(
                        "{" + MethodInfo.GetCurrentMethod() + "} Error:\n\nThe Activator.CreateInstance method could not " +
                        "create a default instance of the supplied value type <" + type +
                        "> (Inner Exception message: \"" + e.Message + "\")", e);
                }
            }

            // Fail with exception
            throw new ArgumentException("{" + MethodInfo.GetCurrentMethod() + "} Error:\n\nThe supplied value type <" + type +
                "> is not a publicly-visible type, so the default value cannot be retrieved");
        }
    }

	public enum Colors
    {
        aqua,
        black,
        blue,
        brown,
        cyan,
        darkblue,
        fuchsia,
        green,
        grey,
        lightblue,
        lime,
        magenta,
        maroon,
        navy,
        olive,
        purple,
        red,
        silver,
        teal,
        white,
        yellow
    }

	public static class StringExtensionMethods
    {
        /// Sets the color of the text according to the parameter value.
        public static string ColorFormat(this string message, Colors color, string format, params object[] args)
        {
            return string.Format("<color={0}>{1}</color>", color.ToString(), string.Format(format, args));
        }
        public static string Colored(this string message, Colors color)
        {
            return string.Format("<color={0}>{1}</color>", color.ToString(), message);
        }

        /// Sets the color of the text according to the traditional HTML format parameter value.

        public static string Colored(this string message, string colorCode)
        {
            return string.Format("<color={0}>{1}</color>", colorCode, message);
        }

        /// Sets the size of the text according to the parameter value, given in pixels.

        public static string Sized(this string message, int size)
        {
            return string.Format("<size={0}>{1}</size>", size, message);
        }

        /// Renders the text in boldface.

        public static string Bold(this string message)
        {
            return string.Format("<b>{0}</b>", message);
        }


        /// Renders the text in italics.

        public static string Italics(this string message)
        {
            return string.Format("<i>{0}</i>", message);
        }
    }


	public enum ColorType
    {
        black,
        white,
        red,
        yellow,
        blue,
        green,
        gray,
        cyan,
        magenta,
    }

    public static class LogExtensionMethods
    {
        static public void LogFormatColor(this Debug target, ColorType color, string format, params object[] args)
        {
            Debug.Log(string.Format("<color={0}>{1}</color>", color.ToString(), string.Format(format, args)));
        }
        static public void LogFormat(this Debug target, ColorType color, string format, params object[] args)
        {
            Debug.Log(string.Format("<color={0}>{1}</color>", color.ToString(), string.Format(format, args)));
        }
        static public void Log(this Debug target, ColorType color, string arg)
        {
            Debug.Log(string.Format("<color={0}>{1}</color>", color.ToString(), arg));
        }
        static public Transform Search(this Transform target, string name)
        {
            if (target.name == name) return target;

            for (int i = 0; i < target.childCount; ++i) {
                var result = Search(target.GetChild(i), name);

                if (result != null) return result;
            }

            return null;
        }
        
        // Get or Add component one time
		static public T GetOrAddComponent<T>(this UnityEngine.Component child) where T : UnityEngine.Component
        {
            T result = child.GetComponent<T>();
            if (result == null) {
                result = child.gameObject.AddComponent<T>();
            }
            return result;
        }

        // transforms
        static public void SetLocalPosX(this Transform t, float newX)
        {
            t.localPosition = new Vector3(newX, t.localPosition.y, t.localPosition.z);
        }
        static public void SetLocalPosY(this Transform t, float newY)
        {
            t.localPosition = new Vector3(t.localPosition.x, newY, t.localPosition.z);
        }
        static public void SetLocalPosZ(this Transform t, float newZ)
        {
            t.localPosition = new Vector3(t.localPosition.x, t.localPosition.y, newZ);
        }
        static public void SetLocalPosXY(this Transform t, float newX, float newY)
        {
            t.localPosition = new Vector3(newX, newY, t.localPosition.z);
        }
        static public void SetLocalPosYZ(this Transform t, float newY, float newZ)
        {
            t.localPosition = new Vector3(t.localPosition.x, newY, newZ);
        }
        static public void SetLocalPosXZ(this Transform t, float newX, float newZ)
        {
            t.localPosition = new Vector3(newX, t.localPosition.y, newZ);
        }
    }


    public static class ReflectionExtensions
    {
        private static PropertyInfo GetPropertyInfo(Type type, string propertyName)
        {
            PropertyInfo propInfo = null;
            do
            {
                propInfo = type.GetProperty(propertyName,
                       BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                type = type.BaseType;
            }
            while (propInfo == null && type != null);
            return propInfo;
        }

        public static object GetPropertyValue(this object obj, string propertyName)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            Type objType = obj.GetType();
            PropertyInfo propInfo = GetPropertyInfo(objType, propertyName);
            if (propInfo == null)
                throw new ArgumentOutOfRangeException("propertyName",
                  string.Format("Couldn't find property {0} in type {1}", propertyName, objType.FullName));
            return propInfo.GetValue(obj, null);
        }

        public static void SetPropertyValue(this object obj, string propertyName, object val)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            Type objType = obj.GetType();
            PropertyInfo propInfo = GetPropertyInfo(objType, propertyName);
            if (propInfo == null)
                throw new ArgumentOutOfRangeException("propertyName",
                  string.Format("Couldn't find property {0} in type {1}", propertyName, objType.FullName));
            propInfo.SetValue(obj, val, null);
        }
    }



}