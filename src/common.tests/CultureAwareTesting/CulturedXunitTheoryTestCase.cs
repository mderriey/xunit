﻿using System;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Internal;
using Xunit.Sdk;

namespace Xunit.v3;

[Serializable]
public class CulturedXunitTheoryTestCase : XunitDelayEnumeratedTheoryTestCase
{
	/// <inheritdoc/>
	protected CulturedXunitTheoryTestCase(
		SerializationInfo info,
		StreamingContext context) :
			base(info, context)
	{
		Culture = Guard.NotNull("Could not retrieve Culture from serialization", info.GetValue<string>("Culture"));
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="CulturedXunitTheoryTestCase"/> class.
	/// </summary>
	/// <param name="defaultMethodDisplay">Default method display to use (when not customized).</param>
	/// <param name="defaultMethodDisplayOptions">Default method display options to use (when not customized).</param>
	/// <param name="testMethod">The method under test.</param>
	public CulturedXunitTheoryTestCase(
		TestMethodDisplay defaultMethodDisplay,
		TestMethodDisplayOptions defaultMethodDisplayOptions,
		_ITestMethod testMethod,
		string culture)
			: base(defaultMethodDisplay, defaultMethodDisplayOptions, testMethod)
	{
		Culture = Guard.ArgumentNotNull(culture);

		Traits.Add("Culture", Culture);

		var cultureDisplay = $"[{Culture}]";
		TestCaseDisplayName += cultureDisplay;
		UniqueID += cultureDisplay;
	}

	public string Culture { get; }

	public override void GetObjectData(
		SerializationInfo info,
		StreamingContext context)
	{
		base.GetObjectData(info, context);

		info.AddValue("Culture", Culture);
	}

	public override ValueTask<RunSummary> RunAsync(
		IMessageBus messageBus,
		object?[] constructorArguments,
		ExceptionAggregator aggregator,
		CancellationTokenSource cancellationTokenSource) =>
			new CulturedXunitTheoryTestCaseRunner(Culture).RunAsync(
				this,
				messageBus,
				aggregator,
				cancellationTokenSource,
				TestCaseDisplayName,
				SkipReason,
				constructorArguments,
				TestMethodArguments
			);
}
