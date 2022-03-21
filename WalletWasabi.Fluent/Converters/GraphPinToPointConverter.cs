using Avalonia;
using Avalonia.Data.Converters;
using System.Globalization;
using WalletWasabi.Fluent.ViewModels.Wallets.Graph;

namespace WalletWasabi.Fluent.Converters;

public class GraphPinToPointConverter : IValueConverter
{
	public static readonly GraphPinToPointConverter Instance = new();

	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is GraphPin pin)
		{
			var x = pin.X;
			var y = pin.Y;

			if (pin.Parent is { })
			{
				x += pin.Parent.X;
				y += pin.Parent.Y;
			}

			return new Point(x, y + 2);
		}

		return new Point();
	}

	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
