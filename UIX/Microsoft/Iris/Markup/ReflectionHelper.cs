// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.ReflectionHelper
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Microsoft.Iris.Markup
{
    internal static class ReflectionHelper
    {
        private static System.Reflection.Emit.OpCode[] s_loadInts = new System.Reflection.Emit.OpCode[9]
        {
      OpCodes.Ldc_I4_0,
      OpCodes.Ldc_I4_1,
      OpCodes.Ldc_I4_2,
      OpCodes.Ldc_I4_3,
      OpCodes.Ldc_I4_4,
      OpCodes.Ldc_I4_5,
      OpCodes.Ldc_I4_6,
      OpCodes.Ldc_I4_7,
      OpCodes.Ldc_I4_8
        };
        private static Module s_irisModule = typeof(ReflectionHelper).Module;

        public static FastReflectionInvokeHandler CreateMethodInvoke(
          MethodBase methodBase)
        {
            DynamicMethod dynamicMethod = new DynamicMethod("MethodInvoker", typeof(object), new Type[2]
            {
                typeof (object),
                typeof (object[])
            }, s_irisModule, true);
            ILGenerator ilGenerator = dynamicMethod.GetILGenerator();
            ParameterInfo[] parameters = methodBase.GetParameters();
            Type[] typeArray = new Type[parameters.Length];
            for (int index = 0; index < parameters.Length; ++index)
                typeArray[index] = parameters[index].ParameterType;
            MethodInfo methodInfo = methodBase as MethodInfo;
            if (methodInfo != null && !methodInfo.IsStatic)
            {
                methodInfo = (MethodInfo)methodBase;
                ilGenerator.Emit(OpCodes.Ldarg_0);
                Type declaringType = methodInfo.DeclaringType;
                EmitCastToType(ilGenerator, declaringType);
                if (declaringType.IsValueType)
                {
                    LocalBuilder local = ilGenerator.DeclareLocal(declaringType);
                    ilGenerator.Emit(OpCodes.Stloc_0);
                    ilGenerator.Emit(OpCodes.Ldloca_S, local);
                }
            }
            for (int index = 0; index < typeArray.Length; ++index)
            {
                ilGenerator.Emit(OpCodes.Ldarg_1);
                if (index < s_loadInts.Length)
                    ilGenerator.Emit(s_loadInts[index]);
                else
                    ilGenerator.Emit(OpCodes.Ldc_I4, index);
                ilGenerator.Emit(OpCodes.Ldelem_Ref);
                EmitCastToType(ilGenerator, typeArray[index]);
            }
            Type cls;
            if (methodInfo != null)
            {
                ilGenerator.EmitCall(methodInfo.IsStatic ? OpCodes.Call : OpCodes.Callvirt, methodInfo, null);
                cls = methodInfo.ReturnType;
            }
            else
            {
                ConstructorInfo con = (ConstructorInfo)methodBase;
                ilGenerator.Emit(OpCodes.Newobj, con);
                cls = con.DeclaringType;
            }
            if (cls == typeof(void))
                ilGenerator.Emit(OpCodes.Ldnull);
            else if (cls.IsValueType)
                ilGenerator.Emit(OpCodes.Box, cls);
            ilGenerator.Emit(OpCodes.Ret);
            return (FastReflectionInvokeHandler)dynamicMethod.CreateDelegate(typeof(FastReflectionInvokeHandler));
        }

        private static void EmitCastToType(ILGenerator il, Type type)
        {
            if (type.IsValueType)
                il.Emit(OpCodes.Unbox_Any, type);
            else
                il.Emit(OpCodes.Castclass, type);
        }
    }
}
