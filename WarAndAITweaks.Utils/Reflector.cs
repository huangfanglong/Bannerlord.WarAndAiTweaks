using System;
using System.Globalization;
using System.Reflection;

namespace WarAndAITweaks.Utils;

public static class Reflector
{
	public const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

	private static PropertyInfo ResolveProperty(Type type, string name)
	{
		Type type2 = type;
		while (type2 != null)
		{
			PropertyInfo property = type2.GetProperty(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			if (property != null)
			{
				return property;
			}
			type2 = type2.BaseType;
		}
		return null;
	}

	private static FieldInfo ResolveField(Type type, string name)
	{
		Type type2 = type;
		while (type2 != null)
		{
			FieldInfo field = type2.GetField(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			if (field != null)
			{
				return field;
			}
			type2 = type2.BaseType;
		}
		return null;
	}

	private static MethodInfo ResolveMethod(Type type, string name, Type[] parameterTypes)
	{
		Type type2 = type;
		while (type2 != null)
		{
			MethodInfo methodInfo = ((parameterTypes != null) ? type2.GetMethod(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy, null, parameterTypes, null) : type2.GetMethod(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy));
			if (methodInfo != null)
			{
				return methodInfo;
			}
			type2 = type2.BaseType;
		}
		return null;
	}

	internal static PropertyInfo P<T>(T instance, string propertyName)
	{
		ref T reference = ref instance;
		T val = default(T);
		Type type;
		if (val == null)
		{
			val = instance;
			reference = ref val;
			if (val == null)
			{
				type = null;
				goto IL_0041;
			}
		}
		type = reference.GetType();
		goto IL_0041;
		IL_0041:
		return ResolveProperty(type ?? typeof(T), propertyName);
	}

	internal static FieldInfo F<T>(T instance, string fieldName)
	{
		ref T reference = ref instance;
		T val = default(T);
		Type type;
		if (val == null)
		{
			val = instance;
			reference = ref val;
			if (val == null)
			{
				type = null;
				goto IL_0041;
			}
		}
		type = reference.GetType();
		goto IL_0041;
		IL_0041:
		return ResolveField(type ?? typeof(T), fieldName);
	}

	internal static MethodInfo M<T>(T instance, string methodName, params Type[] parameterTypes)
	{
		ref T reference = ref instance;
		T val = default(T);
		Type type;
		if (val == null)
		{
			val = instance;
			reference = ref val;
			if (val == null)
			{
				type = null;
				goto IL_0041;
			}
		}
		type = reference.GetType();
		goto IL_0041;
		IL_0041:
		return ResolveMethod(type ?? typeof(T), methodName, parameterTypes);
	}

	public static TReturn GetPropertyValue<TReturn>(object instance, string propertyName)
	{
		if (instance == null)
		{
			throw new ArgumentNullException("instance");
		}
		Type type = instance.GetType();
		PropertyInfo propertyInfo = ResolveProperty(type, propertyName);
		if (propertyInfo == null)
		{
			throw new MissingMemberException(type.FullName, propertyName);
		}
		PropertyInfo propertyInfo2 = propertyInfo;
		MethodInfo getMethod = propertyInfo2.GetGetMethod(nonPublic: true);
		if (getMethod == null)
		{
			throw new MissingMethodException(type.FullName + "." + propertyName + " has no getter.");
		}
		object value = getMethod.Invoke(instance, null);
		return (TReturn)ConvertIfNeeded(value, typeof(TReturn));
	}

	public static object GetPropertyValue(object instance, string propertyName)
	{
		if (instance == null)
		{
			throw new ArgumentNullException("instance");
		}
		Type type = instance.GetType();
		PropertyInfo propertyInfo = ResolveProperty(type, propertyName);
		if (propertyInfo == null)
		{
			throw new MissingMemberException(type.FullName, propertyName);
		}
		PropertyInfo propertyInfo2 = propertyInfo;
		MethodInfo getMethod = propertyInfo2.GetGetMethod(nonPublic: true);
		if (getMethod == null)
		{
			throw new MissingMethodException(type.FullName + "." + propertyName + " has no getter.");
		}
		return getMethod.Invoke(instance, null);
	}

	public static void SetPropertyValue(object instance, string propertyName, object value)
	{
		if (instance == null)
		{
			throw new ArgumentNullException("instance");
		}
		Type type = instance.GetType();
		PropertyInfo propertyInfo = ResolveProperty(type, propertyName);
		if (propertyInfo == null)
		{
			throw new MissingMemberException(type.FullName, propertyName);
		}
		PropertyInfo propertyInfo2 = propertyInfo;
		Type propertyType = propertyInfo2.PropertyType;
		object obj = ConvertIfNeeded(value, propertyType);
		MethodInfo setMethod = propertyInfo2.GetSetMethod(nonPublic: true);
		if (setMethod != null)
		{
			setMethod.Invoke(instance, new object[1] { obj });
			return;
		}
		string text = char.ToLowerInvariant(propertyName[0]) + propertyName.Substring(1);
		string[] array = new string[6]
		{
			"<" + propertyName + ">k__BackingField",
			"_" + text,
			"_" + propertyName,
			"m_" + text,
			text,
			propertyName
		};
		string[] array2 = array;
		foreach (string name in array2)
		{
			FieldInfo fieldInfo = ResolveField(type, name);
			if (fieldInfo != null)
			{
				object value2 = ConvertIfNeeded(obj, fieldInfo.FieldType);
				fieldInfo.SetValue(instance, value2);
				return;
			}
		}
		throw new MissingMethodException(type.FullName + "." + propertyName + " has no setter and no recognizable backing field was found.");
	}

	public static TReturn GetFieldValue<TReturn>(object instance, string fieldName)
	{
		if (instance == null)
		{
			throw new ArgumentNullException("instance");
		}
		FieldInfo fieldInfo = ResolveField(instance.GetType(), fieldName);
		if (fieldInfo == null)
		{
			throw new MissingMemberException(instance.GetType().FullName, fieldName);
		}
		FieldInfo fieldInfo2 = fieldInfo;
		object value = fieldInfo2.GetValue(instance);
		return (TReturn)ConvertIfNeeded(value, typeof(TReturn));
	}

	public static void SetFieldValue(object instance, string fieldName, object value)
	{
		if (instance == null)
		{
			throw new ArgumentNullException("instance");
		}
		FieldInfo fieldInfo = ResolveField(instance.GetType(), fieldName);
		if (fieldInfo == null)
		{
			throw new MissingMemberException(instance.GetType().FullName, fieldName);
		}
		FieldInfo fieldInfo2 = fieldInfo;
		fieldInfo2.SetValue(instance, ConvertIfNeeded(value, fieldInfo2.FieldType));
	}

	public static object InvokeMethod(object instance, string methodName, Type[] parameterTypes, params object[] args)
	{
		if (instance == null)
		{
			throw new ArgumentNullException("instance");
		}
		Type type = instance.GetType();
		MethodInfo methodInfo2;
		if (parameterTypes == null)
		{
			Type[] array = ((args != null && args.Length != 0) ? Array.ConvertAll(args, (object a) => a?.GetType() ?? typeof(object)) : Type.EmptyTypes);
			Type[] parameterTypes2 = array;
			MethodInfo methodInfo;
			if ((methodInfo = ResolveMethod(type, methodName, parameterTypes2)) == null && (methodInfo = ResolveMethod(type, methodName, null)) == null)
			{
				throw new MissingMethodException(type.FullName, methodName);
			}
			methodInfo2 = methodInfo;
		}
		else
		{
			MethodInfo methodInfo3 = ResolveMethod(type, methodName, parameterTypes);
			if (methodInfo3 == null)
			{
				throw new MissingMethodException(type.FullName, methodName);
			}
			methodInfo2 = methodInfo3;
		}
		return methodInfo2.Invoke(instance, args);
	}

	private static object ConvertIfNeeded(object value, Type targetType)
	{
		if (targetType == typeof(void))
		{
			return null;
		}
		if (value == null)
		{
			if (!targetType.IsValueType || Nullable.GetUnderlyingType(targetType) != null)
			{
				return null;
			}
			return Activator.CreateInstance(targetType);
		}
		Type type = value.GetType();
		if (targetType.IsAssignableFrom(type))
		{
			return value;
		}
		Type underlyingType = Nullable.GetUnderlyingType(targetType);
		if (underlyingType != null)
		{
			return ConvertIfNeeded(value, underlyingType);
		}
		if (!targetType.IsEnum)
		{
			return Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
		}
		if (type == typeof(string))
		{
			return Enum.Parse(targetType, (string)value, ignoreCase: true);
		}
		return Enum.ToObject(targetType, Convert.ChangeType(value, Enum.GetUnderlyingType(targetType), CultureInfo.InvariantCulture));
	}
}
