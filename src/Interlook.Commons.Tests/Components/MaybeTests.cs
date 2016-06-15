using Interlook.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Xunit.Extensions;

public class MaybeTests
{
    [Fact]
    public void Bind_Test()
    {
        var justValue = 5;
        var just = justValue.ToMaybe();
        var resultJust = just.Bind(IncrementInt);

        var expectedResult = 6;
        var defaultResult = -1;
        var actualResult = resultJust.GetValue(defaultResult);
        Assert.Equal<int>(expectedResult, actualResult);
    }

    [Fact]
    public void Bind_ToJust_Test()
    {
        var m = 7.ToMaybe();

        var r = m.Bind(p => (p * 6).ToMaybe())
            .GetValue(-1);

        Assert.Equal(42, r);
    }

    [Fact]
    public void Bind_ToNothing_Test()
    {
        var m = 7.ToMaybe();

        var r = m.Bind(p => Nothing<int>.Instance)
            .GetValue(-1);

        Assert.Equal(-1, r);
    }

    [Fact]
    public void BindNotExecutedForNothing_Test()
    {
        var justValue = 5;
        var just = justValue.ToMaybe();
        var resultMaybe = just.Bind(TerminateIt);

        Assert.True(resultMaybe.IsNothing(), "Expected nothing after binding nullifying method to maybe-object.");

        var defaultResult = -1;
        var touched = false;
        var secondResult = resultMaybe.Bind(p =>
            {
                touched = true;
                return defaultResult.ToMaybe();
            });

        Assert.False(touched, "Method bound to maybe-object was executed, although maybe-object was empty.");
        Assert.True(secondResult.IsNothing(), "Result of binding a function to nothing should still be nothing.");
    }

    [Fact]
    public void GetValue_Just_Test()
    {
        Maybe<int> m = new Just<int>(42);
        Assert.Equal(42, m.GetValue(-1));
    }

    [Fact]
    public void GetValue_Nothing_Test()
    {
        Maybe<int> m = Nothing<int>.Instance;
        Assert.Equal(-1, m.GetValue(-1));
    }

    [Fact]
    public void GetValue_Test()
    {
        var justValue = 5;
        var just = new Just<int>(justValue);
        var justNull = new Just<object>(null);
        var nothing = Nothing<int>.Instance;

        var defaultIntValue = 7;
        var defaultObjectValue = "null";

        var currentJustValue = just.GetValue(defaultIntValue);
        var currentJustNullValue = justNull.GetValue(defaultObjectValue);
        var currentNothingValue = nothing.GetValue(defaultIntValue);

        Assert.Equal<int>(justValue, currentJustValue);
        Assert.Equal<object>(null, currentJustNullValue);
        Assert.Equal<int>(defaultIntValue, currentNothingValue);
    }

    [Fact]
    public void HasValue_Just_Test()
    {
        Maybe<int> m = new Just<int>(42);
        Assert.True(m.HasValue());
    }

    [Fact]
    public void HasValue_Nothing_Test()
    {
        Maybe<int> m = Nothing<int>.Instance;
        Assert.False(m.HasValue());
    }

    [Fact]
    public void HasValue_Test()
    {
        var just = new Just<int>(5);
        var justNull = new Just<object>(null);
        var nothing = Nothing<int>.Instance;

        Assert.True(just.HasValue());
        Assert.True(justNull.HasValue(), "Just<object>.HasValue() must return true, even if value is null.");
        Assert.False(nothing.HasValue());
    }

    [Fact]
    public void IsNothing_Just_Test()
    {
        Maybe<int> m = new Just<int>(42);
        Assert.False(m.IsNothing());
    }

    [Fact]
    public void IsNothing_Nothing_Test()
    {
        Maybe<int> m = Nothing<int>.Instance;
        Assert.True(m.IsNothing());
    }

    [Fact]
    public void IsNothing_Test()
    {
        var just = new Just<int>(5);
        var justNull = new Just<object>(null);
        var nothing = Nothing<int>.Instance;

        Assert.False(just.IsNothing());
        Assert.False(justNull.IsNothing());
        Assert.True(nothing.IsNothing());
    }

    [Fact]
    public void Otherwise_Negative_Test()
    {
        var m = Nothing<int>.Instance;
        var alternative = 23.ToMaybe();

        var r = m.Otherwise(alternative);
        Assert.Equal(23, r.GetValue(-1));
    }

    [Fact]
    public void Otherwise_Test()
    {
        var m = 42.ToMaybe();
        var alternative = 23.ToMaybe();

        var r = m.Otherwise(alternative);
        Assert.Equal(42, r.GetValue(-1));
    }

    [Fact]
    public void OtherwiseThrow_Negative_Test()
    {
        var m = Nothing<int>.Instance;
        Maybe<int> r;

        Assert.Throws<InvalidOperationException>(() => r = m.OtherwiseThrow(new InvalidOperationException()));
    }

    [Fact]
    public void OtherwiseThrow_Test()
    {
        var m = 42.ToMaybe();

        var r = m.OtherwiseThrow(new InvalidOperationException());
        Assert.Equal(42, r.GetValue(-1));
    }

    [Fact]
    public void Satisfies_Negative_Test()
    {
        var val = 42;
        var m = val.ToMaybe()
            .Satisfies(p => p > 100);

        Assert.Equal(false, m.GetValue(true));
    }

    [Fact]
    public void Satisfies_Positive_Test()
    {
        var val = 42;
        var m = val.ToMaybe()
            .Satisfies(p => p < 100);

        Assert.Equal(true, m.GetValue(false));
    }

    [Fact]
    public void Satisfies_Test()
    {
        var justString = new Just<string>("Five");
        var justNull = new Just<string>(null);
        var nothing = Nothing<string>.Instance;

        var StartsWithCapitalF = new Func<string, bool>(p => p != null && p.StartsWith("F"));

        var currentResultForJustString = justString.Satisfies(StartsWithCapitalF);
        var currentResultForJustNull = justNull.Satisfies(StartsWithCapitalF);
        var currentResultForNothing = nothing.Satisfies(StartsWithCapitalF);

        var expectedForJustString = true.ToMaybe();
        var expectedForJustNull = false.ToMaybe();
        var expectedForNothing = Nothing<bool>.Instance;

        Assert.Equal<Maybe<bool>>(expectedForJustString, currentResultForJustString);
        Assert.Equal<Maybe<bool>>(expectedForJustNull, currentResultForJustNull);
        Assert.Equal<Maybe<bool>>(expectedForNothing, currentResultForNothing);
    }

    [Fact]
    public void Select_Test()
    {
        var m = 6.ToMaybe();
        var r = from s in m
                select s * 7;

        Assert.Equal(42, r.GetValue(-1));
    }

    [Fact]
    public void SelectMany_Query_ChainedFromsMultiple_Test()
    {
        var m = 4.ToMaybe();

        var z = from four in m
                from sixteen in (four * 4).ToMaybe()
                from o32 in (sixteen * 2).ToMaybe()
                from ulae in (o32 + 10).ToMaybe()
                select ulae;

        Assert.Equal(42, z.GetValue(0));
    }

    [Fact]
    public void SelectMany_Query_ChainedFromsSingle_Test()
    {
        var m = 7.ToMaybe();

        var z = from seven in m
                from forteen in (seven * 2).ToMaybe()
                select forteen;

        Assert.Equal(14, z.GetValue(0));
    }

    [Fact]
    public void SelectMany_Query_CrossedFroms2_Test()
    {
        var s6 = 6.ToMaybe();
        var s7 = 7.ToMaybe();

        var z = from six in s6
                from seven in s7
                select six * seven;

        Assert.Equal(42, z.GetValue(0));
    }

    [Fact]
    public void SelectMany_Query_CrossedFroms3_Test()
    {
        var s2 = 2.ToMaybe();
        var s3 = 3.ToMaybe();
        var s7 = 7.ToMaybe();

        var z = from two in s2
                from three in s3
                from seven in s7
                select two * three * seven;

        Assert.Equal(42, z.GetValue(0));
    }

    [Fact]
    public void ToMaybe_Test()
    {
        var intValue = 5;
        var stringValue = "fünf";

        var jiv = intValue.ToMaybe();
        var jvs = stringValue.ToMaybe();

        var expectedIntMaybe = new Just<int>(intValue);
        var expectedStringMaybe = new Just<string>(stringValue);

        Assert.Equal<Maybe<int>>(expectedIntMaybe, jiv);
        Assert.Equal<Maybe<string>>(expectedStringMaybe, jvs);
    }

    [Fact]
    public void ToMaybe_WithFilter_Negative_Test()
    {
        var val = 42;
        var m = val.ToMaybe(p => p > 100);

        Assert.Equal(-1, m.GetValue(-1));
    }

    [Fact]
    public void ToMaybe_WithFilter_Positive_Test()
    {
        var val = 42;
        var m = val.ToMaybe(p => p < 100);

        Assert.Equal(val, m.GetValue(-1));
    }

    [Fact]
    public void ToMaybeNotNull_NotNull_Test()
    {
        var val = "alpha";
        var m = val.ToMaybe();
        Assert.Equal(val, m.GetValue("falsch"));
    }

    [Fact]
    public void ToMaybeNotNull_Null_Test()
    {
        string val = null;
        var m = val.ToMaybe();
        Assert.Null(m.GetValue("falsch"));
    }

    [Fact]
    public void TwoJustInstancesWithDifferentValuesAintEqual_Test()
    {
        var j1 = new Just<int>(5);
        var j2 = new Just<int>(7);

        var js1 = new Just<string>("fünf");
        var js2 = new Just<string>("sechs");

        Assert.NotEqual<Just<int>>(j1, j2);
        Assert.NotEqual<Just<string>>(js1, js2);

        Assert.NotEqual<Maybe<int>>(j1, j2);
        Assert.NotEqual<Maybe<string>>(js1, js2);

        Assert.False(j1.Equals(j2));
        Assert.False(js1.Equals(js2));
    }

    [Fact]
    public void TwoJustInstancesWithSameValueAreEqual_Test()
    {
        var j1 = new Just<int>(5);
        var j2 = new Just<int>(5);

        var js1 = new Just<string>("fünf");
        var js2 = new Just<string>("fünf");

        Assert.Equal<Just<int>>(j1, j2);
        Assert.Equal<Just<string>>(js1, js2);

        Assert.Equal<Maybe<int>>(j1, j2);
        Assert.Equal<Maybe<string>>(js1, js2);

        Assert.True(j1.Equals(j2));
        Assert.True(js1.Equals(js2));
    }

    [Fact]
    public void TwoNothingInstancesOfDifferentClosedTypesAintEqual_Test()
    {
        var n1 = new Nothing<int>();
        var n2 = Nothing<string>.Instance;

        Assert.False(n1.Equals(n2));
        Assert.False(n2.Equals(n1));
    }

    [Fact]
    public void TwoNothingInstancesOfSameClosedTypeAreEqual_Test()
    {
        var n1 = new Nothing<int>();
        var n2 = Nothing<int>.Instance;

        Assert.Equal<Nothing<int>>(n1, n2);
    }

    [Fact]
    public void Where_Negative_Test()
    {
        var val = 42;
        var m = val.ToMaybe()
            .Where(p => p > 100);

        Assert.Equal(-1, m.GetValue(-1));
    }

    [Fact]
    public void Where_Positive_Test()
    {
        var val = 42;
        var m = val.ToMaybe()
            .Where(p => p < 100);

        Assert.Equal(val, m.GetValue(-1));
    }

    private static Maybe<int> IncrementInt(int value)
    {
        return (value + 1).ToMaybe();
    }

    private static Maybe<T> TerminateIt<T>(T value)
    {
        return Nothing<T>.Instance;
    }
}