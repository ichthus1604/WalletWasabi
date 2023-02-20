using System.Collections.Generic;
using DynamicData;
using FluentAssertions;
using NBitcoin;
using WalletWasabi.Blockchain.Transactions;
using WalletWasabi.Fluent.Models.Wallets;
using WalletWasabi.Fluent.ViewModels.Wallets.Labels;
using Xunit;

namespace WalletWasabi.Tests.Gui;

public class SuggestionLabelsViewModelTests
{
	[InlineData(1, 1)]
	[InlineData(2, 2)]
	[InlineData(3, 3)]
	[InlineData(100, 5)]
	[Theory]
	public void Top_suggestions_should_match(int maxSuggestions, int expected)
	{
		var wallet = new TestWallet(
			new List<(string Label, int Score)>
			{
				("Label 1", 1),
				("Label 2", 2),
				("Label 3", 3),
				("Label 4", 5),
				("Label 5", 4)
			});
		var sut = new SuggestionLabelsViewModel(wallet, Intent.Send, maxSuggestions);

		Assert.Equal(expected, sut.TopSuggestions.Count);
	}
	
	[Fact]
	public void Suggestions_should_be_in_correct_order()
	{
		var mostUsedLabels = new List<(string Label, int Score)>
		{
			("Label 1", 1),
			("Label 2", 3),
			("Label 3", 2),
		};
		var wallet = new TestWallet(mostUsedLabels);
		var sut = new SuggestionLabelsViewModel(wallet, Intent.Send, 100);

		sut.Suggestions.Should().ContainInOrder("Label 2", "Label 3", "Label 1");
	}

	[Fact]
	public void Suggestions_should_not_contain_labels_already_chosen()
	{
		var mostUsedLabels = new List<(string Label, int Score)>
		{
			("Label 1", 1),
			("Label 2", 3),
			("Label 3", 2),
		};
		var wallet = new TestWallet(mostUsedLabels);
		var sut = new SuggestionLabelsViewModel(wallet, Intent.Send, 100);

		sut.Labels.Add("Label 3");

		sut.Suggestions.Should().NotContain("Label 3");
	}

	[Fact]
	public void Top_suggestions_should_not_contain_labels_already_chosen()
	{
		var mostUsedLabels = new List<(string Label, int Score)>
		{
			("Label 1", 1),
			("Label 2", 3),
			("Label 3", 2),
		};
		var wallet = new TestWallet(mostUsedLabels);
		var sut = new SuggestionLabelsViewModel(wallet, Intent.Send, 100);

		sut.Labels.Add("Label 3");
		sut.Labels.Add("Label 1");

		sut.TopSuggestions.Should().NotContain(new[] { "Label 3", "Label 1" });
	}

	[Fact]
	public void Top_suggestions_should_be_empty_when_all_labels_are_chosen()
	{
		var mostUsedLabels = new List<(string Label, int Score)>
		{
			("Label 1", 1),
			("Label 2", 3),
			("Label 3", 2),
		};
		var wallet = new TestWallet(mostUsedLabels);
		var sut = new SuggestionLabelsViewModel(wallet, Intent.Send, 1);

		sut.Labels.Add("Label 1");
		sut.Labels.Add("Label 2");
		sut.Labels.Add("Label 3");

		sut.TopSuggestions.Should().BeEmpty();
	}

	private class TestWallet : IWalletModel
	{
		private readonly List<(string Label, int Score)> _mostUsedLabels;

		public TestWallet(List<(string Label, int Score)> mostUsedLabels)
		{
			_mostUsedLabels = mostUsedLabels;
		}

		public string Name { get; }
		public IObservable<IChangeSet<TransactionSummary, uint256>> Transactions { get; }

		public IAddress CreateReceiveAddress(IEnumerable<string> destinationLabels)
		{
			throw new NotImplementedException();
		}

		public IObservable<Money> Balance { get; }
		public IObservable<IChangeSet<IAddress, string>> Addresses { get; }

		public IEnumerable<(string Label, int Score)> GetMostUsedLabels(Intent intent)
		{
			return _mostUsedLabels;
		}

		public bool IsHardwareWallet()
		{
			throw new NotImplementedException();
		}
	}
}
