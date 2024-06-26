﻿using System;
using NUnit.Framework;

#if LIGHT_EXPRESSION
using static FastExpressionCompiler.LightExpression.Expression;
namespace FastExpressionCompiler.LightExpression.IssueTests
#else
using static System.Linq.Expressions.Expression;
namespace FastExpressionCompiler.IssueTests
#endif
{
    [TestFixture]
    public class Issue127_Switch_is_supported : ITest
    {
        public int Run()
        {
            Switch_nullable_enum_value_equals_via_comparison_method_with_non_nullable_parameters();
            Switch_nullable_enum_value_equals_to_non_nullable_cases_via_comparison_method_Impossible_both_should_be_nullable();
            SwitchIsSupported_string_with_comparison_method();

            Switch_nullable_enum_value_equals_to_nullable_cases();
            SwitchIsSupported_nullable_enum_comparing_with_null();
            SwitchIsSupported_bool_value();
            SwitchIsSupported1();
            SwitchIsSupported31();
            SwitchIsSupported11();
            SwitchIsSupported12();
            SwitchIsSupported2();
            SwitchIsSupported30();
            SwitchIsSupported33();
            SwitchIsSupported3();
            SwitchIsSupported_string();
            SwitchIsSupported6();

            return 16;
        }

        [Test]
        public void SwitchIsSupported1()
        {
            var eVar = Parameter(typeof(int));
            var blockExpr =
                Switch(eVar,
                    Constant("Z"),
                    SwitchCase(Constant("A"), Constant(1)),
                    SwitchCase(Constant("B"), Constant(2)),
                    SwitchCase(Constant("C"), Constant(5))  //Difference of 3 creates empty branches, more creates Conditions
                );

            var expr = Lambda<Func<int, string>>(blockExpr, eVar);
            expr.PrintCSharp();

            var fs = expr.CompileSys();
            fs.PrintIL();
            Assert.AreEqual("A", fs(1));
            Assert.AreEqual("B", fs(2));
            Assert.AreEqual("C", fs(5));
            Assert.AreEqual("Z", fs(45));

            var ff = expr.CompileFast(true);
            ff.PrintIL();
            Assert.AreEqual("A", ff(1));
            Assert.AreEqual("B", ff(2));
            Assert.AreEqual("C", ff(5));
            Assert.AreEqual("Z", ff(45));
        }

        public enum MyEnum
        {
            A, B, C
        }

        [Test]
        public void Switch_nullable_enum_value_equals_to_nullable_cases()
        {
            var eVar = Parameter(typeof(MyEnum?));
            var blockExpr =
                Switch(eVar,
                    Constant("Z"),
                    SwitchCase(Constant("A"), Constant(MyEnum.A, typeof(MyEnum?))),
                    SwitchCase(Constant("B"), Constant(MyEnum.B, typeof(MyEnum?))),
                    SwitchCase(Constant("C"), Constant(MyEnum.C, typeof(MyEnum?)))
                );

            var expr = Lambda<Func<MyEnum?, string>>(blockExpr, eVar);
            expr.PrintCSharp();

            var ff = expr.CompileSys();
            ff.PrintIL();
            Assert.AreEqual("A", ff(MyEnum.A));
            Assert.AreEqual("B", ff(MyEnum.B));
            Assert.AreEqual("C", ff(MyEnum.C));
            Assert.AreEqual("Z", ff(null));

            var fs = expr.CompileFast(true);
            fs.PrintIL();
            Assert.AreEqual("A", fs(MyEnum.A));
            Assert.AreEqual("B", fs(MyEnum.B));
            Assert.AreEqual("C", fs(MyEnum.C));
            Assert.AreEqual("Z", fs(null));
        }

        public static bool MyEnumEquals(MyEnum? y, MyEnum? x) =>
            x.GetValueOrDefault() == y.GetValueOrDefault() && x.HasValue == y.HasValue;

        [Test]
        public void Switch_nullable_enum_value_equals_to_non_nullable_cases_via_comparison_method_Impossible_both_should_be_nullable()
        {
            var eVar = Parameter(typeof(MyEnum?));
            var blockExpr =
                Switch(eVar,
                    Constant("Z"),
                    GetType().GetMethod(nameof(MyEnumEquals)),
                    SwitchCase(Constant("A"), Constant(MyEnum.A, typeof(MyEnum?))),
                    SwitchCase(Constant("B"), Constant(MyEnum.B, typeof(MyEnum?))),
                    SwitchCase(Constant("C"), Constant(MyEnum.C, typeof(MyEnum?)))
                );

            var expr = Lambda<Func<MyEnum?, string>>(blockExpr, eVar);
            expr.PrintCSharp();

            var ff = expr.CompileSys();
            ff.PrintIL();
            Assert.AreEqual("A", ff(MyEnum.A));
            Assert.AreEqual("B", ff(MyEnum.B));
            Assert.AreEqual("C", ff(MyEnum.C));
            Assert.AreEqual("Z", ff(null));

            var fs = expr.CompileFast(true);
            fs.PrintIL();
            Assert.AreEqual("A", fs(MyEnum.A));
            Assert.AreEqual("B", fs(MyEnum.B));
            Assert.AreEqual("C", fs(MyEnum.C));
            Assert.AreEqual("Z", fs(null));
        }

        public static bool MyEnumEqualsNonNullable(MyEnum y, MyEnum x) => y == x;

        [Test]
        public void Switch_nullable_enum_value_equals_via_comparison_method_with_non_nullable_parameters()
        {
            var eVar = Parameter(typeof(MyEnum?));
            var blockExpr =
                Switch(eVar,
                    Constant("Z"),
                    GetType().GetMethod(nameof(MyEnumEqualsNonNullable)),
                    SwitchCase(Constant("A"), Constant(MyEnum.A, typeof(MyEnum?))),
                    SwitchCase(Constant("B"), Constant(MyEnum.B, typeof(MyEnum?))),
                    SwitchCase(Constant("C"), Constant(MyEnum.C, typeof(MyEnum?)))
                );

            var expr = Lambda<Func<MyEnum?, string>>(blockExpr, eVar);
            expr.PrintCSharp();

            var ff = expr.CompileSys();
            ff.PrintIL();
            Assert.AreEqual("A", ff(MyEnum.A));
            Assert.AreEqual("B", ff(MyEnum.B));
            Assert.AreEqual("C", ff(MyEnum.C));
            Assert.AreEqual("Z", ff(null));

            var fs = expr.CompileFast(true);
            fs.PrintIL();
            Assert.AreEqual("A", fs(MyEnum.A));
            Assert.AreEqual("B", fs(MyEnum.B));
            Assert.AreEqual("C", fs(MyEnum.C));
            Assert.AreEqual("Z", fs(null));
        }

        [Test]
        public void SwitchIsSupported_nullable_enum_comparing_with_null()
        {
            var eVar = Parameter(typeof(MyEnum));
            var blockExpr =
                Switch(eVar,
                    Constant(null, typeof(long?)),
                    SwitchCase(Constant(1L, typeof(long?)), Constant(MyEnum.A)),
                    SwitchCase(Constant(2L, typeof(long?)), Constant(MyEnum.B))
                );

            var e = Lambda<Func<MyEnum, long?>>(blockExpr, eVar);

            var fs = e.CompileSys();
            fs.PrintIL();

            var f = e.CompileFast(true);
            f.PrintIL();

            Assert.NotNull(f);
            Assert.AreEqual(2L, f(MyEnum.B));
            Assert.AreEqual(null, f(MyEnum.C));
        }

        [Test]
        public void SwitchIsSupported11()
        {
            var eVar = Parameter(typeof(int));
            var blockExpr =
                Switch(eVar,
                    Constant("Z"),
                    SwitchCase(Constant("A"), Constant(1)),
                    SwitchCase(Constant("B"), Constant(2)),
                    SwitchCase(Constant("C"), Constant(7)),  //Difference of 3 creates empty branches, more creates Conditions
                    SwitchCase(Constant("C"), Constant(8))  //Difference of 3 creates empty branches, more creates Conditions
                );

            var lambda = Lambda<Func<int, string>>(blockExpr, eVar);
            var f = lambda.CompileFast(true);

            Assert.NotNull(f);
            Assert.AreEqual("B", f(2));
        }

        [Test]
        public void SwitchIsSupported12()
        {
            var eVar = Parameter(typeof(int));
            var blockExpr =
                Switch(eVar,
                    Constant("Z"),
                    SwitchCase(Constant("A"), Constant(1)),
                    SwitchCase(Constant("B"), Constant(2)),
                    SwitchCase(Constant("C"), Constant(7))  //Difference of 3 creates empty branches, more creates Conditions
                );

            var lambda = Lambda<Func<int, string>>(blockExpr, eVar);
            var fastCompiled = lambda.CompileFast(true);
            Assert.NotNull(fastCompiled);
            Assert.AreEqual("B", fastCompiled(2));
        }


        [Test]
        public void SwitchIsSupported2()
        {
            var eVar = Parameter(typeof(long));
            var blockExpr =
                Switch(eVar,
                    Constant("Z"),
                    SwitchCase(Constant("A"), Constant(1L)),
                    SwitchCase(Constant("B"), Constant(2L)),
                    SwitchCase(Constant("C"), Constant(3L))
                );

            var lambda = Lambda<Func<long, string>>(blockExpr, eVar);
            var fastCompiled = lambda.CompileFast(true);
            Assert.NotNull(fastCompiled);
            Assert.AreEqual("B", fastCompiled(2));
        }

        [Test]
        public void SwitchIsSupported30()
        {
            var eVar = Parameter(typeof(int?));
            var blockExpr = Equal(eVar, Constant(94, typeof(int?)));

            var lambda = Lambda<Func<int?, bool>>(blockExpr, eVar);
            var fastCompiled = lambda.CompileFast(true);
            Assert.NotNull(fastCompiled);
            Assert.AreEqual(true, fastCompiled(94));
        }

        [Test]
        public void SwitchIsSupported33()
        {
            var blockExpr = Convert(Constant(94), typeof(int?));

            var lambda = Lambda<Func<int?>>(blockExpr);
            var fastCompiled = lambda.CompileFast(true);
            Assert.NotNull(fastCompiled);
            Assert.AreEqual(94, fastCompiled());
        }

        [Test]
        public void SwitchIsSupported3()
        {
            var eVar = Parameter(typeof(int?));
            var blockExpr =
                Switch(eVar,
                    Constant("Z"),
                    SwitchCase(Constant("A"), Constant((int?)94, typeof(int?))),
                    SwitchCase(Constant("B"), Constant((int?)96, typeof(int?))),
                    SwitchCase(Constant("C"), Constant((int?)98, typeof(int?)))
                );

            var lambda = Lambda<Func<int?, string>>(blockExpr, eVar);
            var fastCompiled = lambda.CompileFast(true);
            Assert.NotNull(fastCompiled);
            Assert.AreEqual("B", fastCompiled(96));
        }

        [Test]
        public void SwitchIsSupported31()
        {
            var eVar = Parameter(typeof(int));
            var blockExpr =
                Switch(eVar,
                    Constant("Z"),
                    SwitchCase(Constant("A"), Constant(1))
                );

            var e = Lambda<Func<int, string>>(blockExpr, eVar);
            e.PrintCSharp();

            var fs = e.CompileSys();
            fs.PrintIL();
            Assert.AreEqual("A", fs(1));
            Assert.AreEqual("Z", fs(42));

            var ff = e.CompileFast(true);
            ff.PrintIL();
            Assert.AreEqual("A", ff(1));
            Assert.AreEqual("Z", ff(42));
        }

        [Test]
        public void SwitchIsSupported_bool_value()
        {
            var eVar = Parameter(typeof(bool));
            var blockExpr =
                Switch(eVar,
                    Constant("Z"),
                    SwitchCase(Constant("A"), Constant(true)),
                    SwitchCase(Constant("B"), Constant(false))
                );

            var expr = Lambda<Func<bool, string>>(blockExpr, eVar);
            expr.PrintCSharp();

            var fs = expr.CompileSys();
            fs.PrintIL();
            Assert.AreEqual("A", fs(true));
            Assert.AreEqual("B", fs(false));

            var ff = expr.CompileFast(true);
            ff.PrintIL();
            Assert.AreEqual("A", ff(true));
            Assert.AreEqual("B", ff(false));
        }

        [Test]
        public void SwitchIsSupported_string()
        {
            var eVar = Parameter(typeof(string));
            var blockExpr =
                Switch(eVar,
                    Constant("C"),
                    SwitchCase(Constant("A"), Constant("A")),
                    SwitchCase(Constant("B"), Constant("B"))
                );

            var expr = Lambda<Func<string, string>>(blockExpr, eVar);
            expr.PrintCSharp();

            var fs = expr.CompileSys();
            fs.PrintIL();
            Assert.AreEqual("A", fs("A"));
            Assert.AreEqual("B", fs("B"));
            Assert.AreEqual("C", fs("Z"));

            var ff = expr.CompileFast(true);
            ff.PrintIL();
            Assert.AreEqual("A", ff("A"));
            Assert.AreEqual("B", ff("B"));
            Assert.AreEqual("C", ff("Z"));
        }

        public static bool StringCompareOrdinalIgnoreCase(string x, string y) =>
            StringComparer.OrdinalIgnoreCase.Equals(x, y);

        [Test]
        public void SwitchIsSupported_string_with_comparison_method()
        {
            var eVar = Parameter(typeof(string));
            var blockExpr =
                Switch(eVar,
                    Constant("C"),
                    GetType().GetMethod(nameof(StringCompareOrdinalIgnoreCase)),
                    SwitchCase(Constant("A"), Constant("a")),
                    SwitchCase(Constant("B"), Constant("b"))
                );

            var expr = Lambda<Func<string, string>>(blockExpr, eVar);
            expr.PrintCSharp();

            var fs = expr.CompileSys();
            fs.PrintIL();
            Assert.AreEqual("A", fs("A"));
            Assert.AreEqual("B", fs("B"));
            Assert.AreEqual("C", fs("Z"));

            var ff = expr.CompileFast(true);
            ff.PrintIL();
            Assert.AreEqual("A", ff("A"));
            Assert.AreEqual("B", ff("B"));
            Assert.AreEqual("C", ff("Z"));
        }

        class Helper
        {
            public string V { get; set; }
        }

        [Test]
        public void SwitchIsSupported6()
        {
            var eVar = Parameter(typeof(Helper));
            var blockExpr =
                Switch(Property(eVar, "V"),
                    Constant("C"),
                    SwitchCase(Constant("A"), Constant("A")),
                    SwitchCase(Constant("B"), Constant("B")),
                    SwitchCase(Constant("C"), Constant("C")),
                    SwitchCase(Constant("D"), Constant("D")),
                    SwitchCase(Constant("E"), Constant("E")),
                    SwitchCase(Constant("F"), Constant("F")),
                    SwitchCase(Constant("G"), Constant("G")),
                    SwitchCase(Constant("H"), Constant("H")),
                    SwitchCase(Constant("I"), Constant("I"))
                );

            var lambda = Lambda<Func<Helper, string>>(blockExpr, eVar);
            var fastCompiled = lambda.CompileFast(true);
            Assert.NotNull(fastCompiled);
            Assert.AreEqual("A", fastCompiled(new Helper() { V = "A" }));
            Assert.AreEqual("C", fastCompiled(new Helper() { V = "Z" }));
        }
    }
}
