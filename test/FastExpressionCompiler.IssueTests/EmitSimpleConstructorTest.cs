﻿using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using NUnit.Framework;
#if !LIGHT_EXPRESSION
namespace FastExpressionCompiler.IssueTests
{
    [TestFixture]
    public class EmitSimpleConstructorTest : ITest
    {
        public int Run()
        {
            DynamicMethod_Emit_Newobj();
            // DynamicMethod_Hack_Emit_Newobj();
            return 2;
        }

        [Test]
        public void DynamicMethod_Emit_Newobj()
        {
            var f = Get_DynamicMethod_Emit_Newobj();
            var a = f();
            Assert.IsInstanceOf<A>(a);
        }

        // [Test]
        public void DynamicMethod_Hack_Emit_Newobj()
        {
            var f = Get_DynamicMethod_Hack_Emit_Newobj();
            var a = f();
            Assert.IsInstanceOf<A>(a);
        }

        public static Func<A> Get_DynamicMethod_Emit_Newobj()
        {
            var dynMethod = new DynamicMethod(string.Empty,
                typeof(A), new[] { typeof(ExpressionCompiler.ArrayClosure) },
                typeof(ExpressionCompiler), skipVisibility: true);

            var il = dynMethod.GetILGenerator();

            il.Emit(OpCodes.Newobj, _aCtor);
            il.Emit(OpCodes.Ret);

            return (Func<A>)dynMethod.CreateDelegate(typeof(Func<A>), ExpressionCompiler.EmptyArrayClosure);
        }

        public static Func<A> Get_DynamicMethod_Hack_Emit_Newobj()
        {
            var dynMethod = new DynamicMethod(string.Empty,
                typeof(A), new[] { typeof(ExpressionCompiler.ArrayClosure) },
                typeof(ExpressionCompiler), skipVisibility: true);

            var il = dynMethod.GetILGenerator();
            var ilType = il.GetType();

            il.Emit(OpCodes.Newobj, _aCtor);

            Debug.Assert(_aCtor.DeclaringType != null && !_aCtor.DeclaringType.IsGenericType);
            // var rtConstructor = con as RuntimeConstructorInfo;
            var methodHandle = _aCtor.MethodHandle;

            // m_tokens.Add(rtConstructor.MethodHandle);
            // var tk = m_tokens.Count - 1 | (int)MetadataTokenType.MethodDef;

            var mScopeField = ilType.GetField("m_scope", BindingFlags.Instance | BindingFlags.NonPublic);
            if (mScopeField == null)
                return null;
            var mScope = mScopeField.GetValue(il);

            var mTokensField = mScope.GetType().GetField("m_tokens", BindingFlags.Instance | BindingFlags.NonPublic);
            if (mTokensField == null)
                return null;
            var mTokens = mTokensField.GetValue(mScope);


            il.Emit(OpCodes.Ret);

            return (Func<A>)dynMethod.CreateDelegate(typeof(Func<A>), ExpressionCompiler.EmptyArrayClosure);
        }

        public class A
        {
            public A() { }
        }

        private static readonly ConstructorInfo _aCtor = typeof(A).GetConstructor(Type.EmptyTypes);
    }
}
#endif