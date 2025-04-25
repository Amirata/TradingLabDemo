using TradingJournal.Domain.Enums;

namespace TradingJournal.Application.Trades.Queries.GetTradeById;

public class GetTradeByIdQuery
    : IQuery<GetTradeByIdResult>
{
    public Guid TradeId { get; set; }
}

public class GetTradeByIdResult
{
    public Guid Id { get; set; }
    public Symbols Symbol { get;  set; }
    public PositionType PositionType { get;  set; }
    public double Volume { get;  set; }
    public double EntryPrice { get;  set; }
    public double ClosePrice { get;  set; }
    public double StopLossPrice { get;  set; }
    public DateTime EntryDateTime { get;  set; }
    public DateTime CloseDateTime { get;  set; }
    public double Commission { get;  set; }
    public double Swap { get;  set; }
    public double Pips { get;  set; }
    public double NetProfit { get;  set; }
    public double GrossProfit { get;  set; }
    public double Balance { get;  set; }
    public Guid TradingPlanId { get; set; }

    public string TradingPlanName { get; set; } = default!;
}
