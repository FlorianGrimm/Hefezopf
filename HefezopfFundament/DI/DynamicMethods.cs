#region Copyright © 2007 FocusPoint Solutions
// This File is part of the neXus.CORE Project
//
// Copyright © 2007 FocusPoint Solutions
// All rights reserved

// This library is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published
// by the Free Software Foundation; either version 2.1 of the License, or
// (at your option) any later version.
//
// This library is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this library; if not, write to the
// Free Software Foundation, Inc.,
// 51 Franklin Street, Fifth Floor Boston, MA  02110-1301 USA
#endregion

#region Revision history
// 2007.05.21 - Pedro Gomes (pedro.gomes@focuspoint-solutions.com)
//      Initial version.
// 2007.06.13 - Pedro Gomes (pedro.gomes@focuspoint-solutions.com)
//      Fixed major bug. Setters are now casting the input value.
// 2007.12.10 - Pedro Gomes (pedro.gomes@focuspoint-solutions.com)
//      Now supports generic methods invocation.
#endregion

#region Include directives
using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
#endregion

//namespace com.focuspoint.nexus.reflection
namespace Hefezopf.Fundament.DI
{

    public delegate object GenericInvoker(object target, params object[] arguments);

    public static class DynamicMethods
    {
        private static void FindMethod(Type type, string methodName, Type[] typeArguments, Type[] parameterTypes, out MethodInfo methodInfo, out ParameterInfo[] parameters)
        {

            methodInfo = null;
            parameters = null;

            if (null == parameterTypes)
            {
                methodInfo = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);
                methodInfo = methodInfo.MakeGenericMethod(typeArguments);
                parameters = methodInfo.GetParameters();
            }
            else
            {
                // Method is probably overloaded. As far as i know there's no other way to get the MethodInfo instance, we have to
                // search for it in all the type methods
                MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
                foreach (MethodInfo method in methods)
                {
                    if (method.Name == methodName)
                    {
                        // create the generic method
                        MethodInfo genericMethod = method.MakeGenericMethod(typeArguments);
                        parameters = genericMethod.GetParameters();

                        // compare the method parameters
                        if (parameters.Length == parameterTypes.Length)
                        {
                            for (int i = 0; i < parameters.Length; i++)
                            {
                                if (parameters[i].ParameterType != parameterTypes[i])
                                {
                                    continue; // this is not the method we'r looking for
                                }
                            }

                            // if we'r here, we got the rigth method
                            methodInfo = genericMethod;
                            break;
                        }
                    }
                }

                if (null == methodInfo)
                {
                    throw new InvalidOperationException("Method not found");
                }
            }
        }

        public static GenericInvoker GenericMethodInvokerMethod(Type type, string methodName, Type[] typeArguments, Type[] parameterTypes)
        {
            MethodInfo methodInfo;
            ParameterInfo[] parameters;
            // find the method to be invoked
            FindMethod(type, methodName, typeArguments, parameterTypes, out methodInfo, out parameters);
            return GenericMethodInvokerMethod(type, methodInfo, typeArguments, parameterTypes, parameters);
        }
        public static GenericInvoker GenericMethodInvokerMethod(Type type, MethodInfo methodInfo)
        {
            //Type[] typeArguments, Type[] parameterTypes, ParameterInfo[] parameters
            return GenericMethodInvokerMethod(type, methodInfo, null, null, methodInfo.GetParameters());
        }
        public static GenericInvoker GenericMethodInvokerMethod(MethodInfo methodInfo, ParameterInfo[] parameters)
        {
            
            string name = string.Format("__MethodInvoker_{0}_ON_{1}", methodInfo.Name, methodInfo.DeclaringType.Name);
            DynamicMethod dynamicMethod = new DynamicMethod(name, typeof(object), new Type[] { typeof(object), typeof(object[]) },
              methodInfo.DeclaringType);

            ILGenerator generator = dynamicMethod.GetILGenerator();

            // define local vars
            generator.DeclareLocal(typeof(object));

            // load first argument, the instace where the method is to be invoked
            generator.Emit(OpCodes.Ldarg_0);

            // cast to the correct type
            generator.Emit(OpCodes.Castclass, methodInfo.DeclaringType);

            for (int i = 0; i < parameters.Length; i++)
            {
                // load paramters they are passed as an object array
                generator.Emit(OpCodes.Ldarg_1);

                // load array element
                generator.Emit(OpCodes.Ldc_I4, i);
                generator.Emit(OpCodes.Ldelem_Ref);

                // cast or unbox parameter as needed
                Type parameterType = parameters[i].ParameterType;
                if (parameterType.IsClass)
                {
                    generator.Emit(OpCodes.Castclass, parameterType);
                }
                else
                {
                    generator.Emit(OpCodes.Unbox_Any, parameterType);
                }
            }

            // call method
            generator.EmitCall(OpCodes.Callvirt, methodInfo, null);

            // handle method return if needed
            if (methodInfo.ReturnType == typeof(void))
            {
                // return null
                generator.Emit(OpCodes.Ldnull);
            }
            else
            {
                // box value if needed
                if (methodInfo.ReturnType.IsValueType)
                {
                    generator.Emit(OpCodes.Box, methodInfo.ReturnType);
                }
            }

            // store to the local var
            generator.Emit(OpCodes.Stloc_0);

            // load local and return
            generator.Emit(OpCodes.Ldloc_0);
            generator.Emit(OpCodes.Ret);

            // return delegate
            return (GenericInvoker)dynamicMethod.CreateDelegate(typeof(GenericInvoker));
        }

        public static GenericInvoker GenericMethodInvokerMethod(Type type, string methodName, Type[] typeArguments)
        {
            return GenericMethodInvokerMethod(type, methodName, typeArguments, null);
        }
    }
}
