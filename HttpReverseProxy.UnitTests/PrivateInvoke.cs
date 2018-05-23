/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/
using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;

namespace Egora.Stammportal.HttpReverseProxy.UnitTests
{
  /// <summary>
  /// Provides utility functions for accessing non-public types and members.
  /// </summary>
  public static class PrivateInvoke
  {
    // static members

    private static MethodInfo GetMethod(Type type, string methodName, BindingFlags bindingFlags, object[] arguments)
    {
      Debug.Assert(methodName != null);
      return (MethodInfo) GetMethodBaseInternal(type, methodName, type.GetMethods(bindingFlags), arguments);
    }

    private static ConstructorInfo GetConstructor(Type type, BindingFlags bindingFlags, object[] arguments)
    {
      return (ConstructorInfo) GetMethodBaseInternal(type, null, type.GetConstructors(bindingFlags), arguments);
    }

    private static MethodBase GetMethodBaseInternal(Type type, string methodName, MethodBase[] methods,
                                                    object[] arguments)
    {
      MethodBase callMethod = null;

      foreach (MethodBase method in methods)
      {
        if (methodName == null || methodName == method.Name)
        {
          ParameterInfo[] parameters = method.GetParameters();
          if (parameters.Length == arguments.Length)
          {
            bool isMatch = true;
            for (int i = 0; i < parameters.Length; ++i)
            {
              object argument = arguments[i];
              Type parameterType = parameters[i].ParameterType;

              if (
                !((argument == null && !parameterType.IsValueType)
                  // null is a valid argument for any reference type
                  || (argument != null && parameterType.IsAssignableFrom(argument.GetType()))))
              {
                isMatch = false;
                break;
              }
            }
            if (isMatch)
            {
              if (callMethod != null)
                throw new AmbiguousMethodNameException(methodName, type);
              callMethod = method;
            }
          }
        }
      }
      if (callMethod == null)
        throw new MethodNotFoundException(methodName, type);

      return callMethod;
    }

    private static PropertyInfo GetPropertyRecursive(Type type, BindingFlags bindingFlags, string propertyName)
    {
      for (PropertyInfo property = null; type != null; type = type.BaseType)
      {
        property = type.GetProperty(propertyName, bindingFlags);
        if (property != null)
          return property;
      }
      return null;
    }

    private static FieldInfo GetFieldRecursive(Type type, BindingFlags bindingFlags, string fieldName)
    {
      for (FieldInfo field = null; type != null; type = type.BaseType)
      {
        field = type.GetField(fieldName, bindingFlags);
        if (field != null)
          return field;
      }
      return null;
    }


    public static object InvokePublicStaticMethod(Type type, string methodName, params object[] arguments)
    {
      ArgumentUtility.CheckNotNull("type", type);
      ArgumentUtility.CheckNotNullOrEmpty("methodName", methodName);

      return InvokeMethodInternal(null, type, BindingFlags.Static | BindingFlags.Public, methodName, arguments);
    }

    public static object InvokeNonPublicMethod(object target, string methodName, params object[] arguments)
    {
      ArgumentUtility.CheckNotNull("target", target);

      return InvokeNonPublicMethod(target, target.GetType(), methodName, arguments);
    }

    public static object InvokeNonPublicMethod(object target, Type definingType, string methodName,
                                               params object[] arguments)
    {
      ArgumentUtility.CheckNotNull("target", target);
      ArgumentUtility.CheckNotNull("definingType", definingType);
      if (target != null && !definingType.IsAssignableFrom(target.GetType()))
        throw new ArgumentException(string.Format("Argument has type {0} when type {1} was expected.",
                                                  target.GetType(), definingType, "target"));
      ArgumentUtility.CheckNotNullOrEmpty("methodName", methodName);

      return InvokeMethodInternal(target, definingType, BindingFlags.Instance | BindingFlags.NonPublic, methodName,
                                  arguments);
    }

    private static object InvokeMethodInternal(object instance, Type type, BindingFlags bindingFlags,
                                               string methodName, object[] arguments)
    {
      if (arguments == null)
        arguments = new object[] {null};

      MethodInfo callMethod = GetMethod(type, methodName, bindingFlags, arguments);

      try
      {
        return callMethod.Invoke(instance, bindingFlags, null, arguments, CultureInfo.InvariantCulture);
      }
      catch (TargetInvocationException e)
      {
        throw e.InnerException;
      }
    }


    public static object CreateInstanceNonPublicCtor(Type type, params object[] arguments)
    {
      if (arguments == null)
        arguments = new object[] {null};

      BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;

      ConstructorInfo ctor = GetConstructor(type, bindingFlags, arguments);

      try
      {
        return ctor.Invoke(BindingFlags.CreateInstance, null, arguments, CultureInfo.InvariantCulture);
      }
      catch (TargetInvocationException e)
      {
        throw e.InnerException;
      }
    }

    public static void SetNonPublicStaticProperty(Type type, string propertyName, object value)
    {
      if (type == null)
        throw new ArgumentNullException("type");

      PropertyInfo property = GetPropertyRecursive(type, BindingFlags.Static | BindingFlags.NonPublic,
                                                   propertyName);
      try
      {
        property.SetValue(null, value, new object[] {});
      }
      catch (TargetInvocationException e)
      {
        throw e.InnerException;
      }
    }

    public static object GetNonPublicStaticField(Type type, string fieldName)
    {
      if (type == null)
        throw new ArgumentNullException("type");

      FieldInfo field = GetFieldRecursive(type, BindingFlags.Static | BindingFlags.NonPublic, fieldName);
      try
      {
        return field.GetValue(null);
      }
      catch (TargetInvocationException e)
      {
        throw e.InnerException;
      }
    }

    public static void SetNonPublicField(object target, string fieldName, object value)
    {
      if (target == null)
        throw new ArgumentNullException("target");

      FieldInfo field = GetFieldRecursive(target.GetType(), BindingFlags.Instance | BindingFlags.NonPublic,
                                          fieldName);
      try
      {
        field.SetValue(target, value);
      }
      catch (TargetInvocationException e)
      {
        throw e.InnerException;
      }
    }
  }


  [Serializable]
  public class AmbiguousMethodNameException : Exception
  {
    private const string c_errorMessage = "Method name \"{0}\" is ambiguous in type {1}.";

    public AmbiguousMethodNameException(string methodName, Type type)
      : this(string.Format(c_errorMessage, methodName, type.FullName))
    {
    }

    public AmbiguousMethodNameException(string message)
      : base(message)
    {
    }

    protected AmbiguousMethodNameException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }

  [Serializable]
  public class MethodNotFoundException : Exception
  {
    private const string c_errorMessage =
      "There is no method \"{0}\" in type {1} that accepts the specified argument types.";

    public MethodNotFoundException(string methodName, Type type)
      : this(string.Format(c_errorMessage, methodName, type.FullName))
    {
    }

    public MethodNotFoundException(string message)
      : base(message)
    {
    }

    protected MethodNotFoundException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}