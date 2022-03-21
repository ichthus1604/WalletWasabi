using ReactiveUI;
using WalletWasabi.Fluent.Models;
using WalletWasabi.Fluent.ViewModels.Wallets.Send;

namespace WalletWasabi.Fluent.ViewModels.Wallets.Graph;

public partial class GraphConnector : ReactiveObject
{
	[AutoNotify] private TransactionGraphViewModel? _parent;
	[AutoNotify] private GraphConnectorOrientation _orientation;
	[AutoNotify] private GraphPin? _start;
	[AutoNotify] private GraphPin? _end;
	[AutoNotify] private double _offset = 50;

	public void GetControlPoints(ref double p1X, ref double p1Y, ref double p2X, ref double p2Y)
	{
		var p1A = Start?.Alignment ?? GraphPinAlignment.None;
		var p2A = End?.Alignment ?? GraphPinAlignment.None;

		switch (_orientation)
		{
			case GraphConnectorOrientation.Auto:
				switch (p1A)
				{
					case GraphPinAlignment.None:
						{
							switch (p2A)
							{
								case GraphPinAlignment.None:
									break;
								case GraphPinAlignment.Left:
									p2X -= _offset;
									p1X += _offset;
									break;
								case GraphPinAlignment.Right:
									p2X += _offset;
									p1X -= _offset;
									break;
								case GraphPinAlignment.Top:
									p2Y -= _offset;
									p1Y += _offset;
									break;
								case GraphPinAlignment.Bottom:
									p2Y += _offset;
									p1Y -= _offset;
									break;
							}
						}
						break;
					case GraphPinAlignment.Left:
						switch (p2A)
						{
							case GraphPinAlignment.None:
								p1X -= _offset;
								p2X += _offset;
								break;
							case GraphPinAlignment.Left:
								p1X -= _offset;
								p2X -= _offset;
								break;
							case GraphPinAlignment.Right:
								p1X -= _offset;
								p2X += _offset;
								break;
							case GraphPinAlignment.Top:
								p1X -= _offset;
								break;
							case GraphPinAlignment.Bottom:
								p2Y += _offset;
								break;
						}
						break;
					case GraphPinAlignment.Right:
						switch (p2A)
						{
							case GraphPinAlignment.None:
								p1X += _offset;
								p2X -= _offset;
								break;
							case GraphPinAlignment.Left:
								p1X += _offset;
								p2X -= _offset;
								break;
							case GraphPinAlignment.Right:
								p1X += _offset;
								p2X += _offset;
								break;
							case GraphPinAlignment.Top:
								p1X += _offset;
								break;
							case GraphPinAlignment.Bottom:
								p2Y += _offset;
								break;
						}
						break;
					case GraphPinAlignment.Top:
						switch (p2A)
						{
							case GraphPinAlignment.None:
								p1Y -= _offset;
								p2Y += _offset;
								break;
							case GraphPinAlignment.Left:
								p2X -= _offset;
								break;
							case GraphPinAlignment.Right:
								p2X += _offset;
								break;
							case GraphPinAlignment.Top:
								p1Y -= _offset;
								p2Y -= _offset;
								break;
							case GraphPinAlignment.Bottom:
								p1Y -= _offset;
								p2Y += _offset;
								break;
						}
						break;
					case GraphPinAlignment.Bottom:
						switch (p2A)
						{
							case GraphPinAlignment.None:
								p1Y += _offset;
								p2Y -= _offset;
								break;
							case GraphPinAlignment.Left:
								p1Y += _offset;
								break;
							case GraphPinAlignment.Right:
								p1Y += _offset;
								break;
							case GraphPinAlignment.Top:
								p1Y += _offset;
								p2Y -= _offset;
								break;
							case GraphPinAlignment.Bottom:
								p1Y += _offset;
								p2Y += _offset;
								break;
						}
						break;
				}

				break;
			case GraphConnectorOrientation.Horizontal:
				p1X += _offset;
				p2X -= _offset;
				break;
			case GraphConnectorOrientation.Vertical:
				p1Y += _offset;
				p2Y -= _offset;
				break;
		}
	}
}


