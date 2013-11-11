using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interlook.Components;
using Xunit;
using Xunit.Extensions;

public class MaybeTests
{
	[Fact]
	public void TwoNothingInstancesOfSameClosedTypeAreEqual_Test()
	{
		var n1 = new Nothing<int>();
		var n2 = Nothing<int>.Instance;

		Assert.Equal<Nothing<int>>(n1, n2);
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

	private static Maybe<int> IncrementInt(int value)
	{
		return (value + 1).ToMaybe();
	}

	private static Maybe<T> TerminateIt<T>(T value)
	{
		return Nothing<T>.Instance;
	}
}