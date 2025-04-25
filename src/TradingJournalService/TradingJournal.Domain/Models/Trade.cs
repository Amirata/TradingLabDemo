namespace TradingJournal.Domain.Models;

public class Trade : Entity<TradeId>
{
    public Symbols Symbol { get; set; }
    public PositionType PositionType { get; set; }
    public double Volume { get; set; }
    public double EntryPrice { get; set; }
    public double ClosePrice { get; set; }
    public double StopLossPrice { get; set; }
    public DateTime EntryDateTime { get; set; }
    public DateTime CloseDateTime { get; set; }
    public double Commission { get; set; }
    public double Swap { get; set; }
    public double Pips { get; set; }
    public double NetProfit { get; set; }
    public double GrossProfit { get; set; }
    public double Balance { get; set; }

    public TradingPlanId TradingPlanId { get; set; } = default!;

    public TradingPlan TradingPlan { get; init; }

    public static Trade Create(
        TradeId id,
        Symbols symbol,
        PositionType positionType,
        double volume,
        double entryPrice,
        double closePrice,
        double stopLossPrice,
        DateTime entryDateTime,
        DateTime closeDateTime,
        double commission,
        double swap,
        double pips,
        double netProfit,
        double grossProfit,
        double balance,
        TradingPlanId tradingPlanId
    )
    {
        ArgumentOutOfRangeException.ThrowIfEqual(entryDateTime,DateTime.MinValue);
        ArgumentOutOfRangeException.ThrowIfEqual(closeDateTime,DateTime.MinValue);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(volume);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryPrice);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(closePrice);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(stopLossPrice);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(commission,0);
        ArgumentOutOfRangeException.ThrowIfNegative(balance);
       
        return new Trade()
        {
            Id = id,
            Symbol = symbol,
            PositionType = positionType,
            Volume = volume,
            EntryPrice = entryPrice,
            ClosePrice = closePrice,
            StopLossPrice = stopLossPrice,
            EntryDateTime = entryDateTime,
            CloseDateTime = closeDateTime,
            Commission = commission,
            Swap = swap,
            Pips = pips,
            NetProfit = netProfit,
            GrossProfit = grossProfit,
            Balance = balance,
            TradingPlanId = tradingPlanId
        };
    }

    public void Update(
        Symbols symbol,
        PositionType positionType,
        double volume,
        double entryPrice,
        double closePrice,
        double stopLossPrice,
        DateTime entryDateTime,
        DateTime closeDateTime,
        double commission,
        double swap,
        double pips,
        double netProfit,
        double grossProfit,
        double balance
    )
    {
        ArgumentOutOfRangeException.ThrowIfEqual(entryDateTime,DateTime.MinValue);
        ArgumentOutOfRangeException.ThrowIfEqual(closeDateTime,DateTime.MinValue);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(volume);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryPrice);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(closePrice);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(stopLossPrice);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(commission,0);
        ArgumentOutOfRangeException.ThrowIfNegative(balance);
        
        Symbol = symbol;
        PositionType = positionType;
        Volume = volume;
        EntryPrice = entryPrice;
        ClosePrice = closePrice;
        StopLossPrice = stopLossPrice;
        EntryDateTime = entryDateTime;
        CloseDateTime = closeDateTime;
        Commission = commission;
        Swap = swap;
        Pips = pips;
        NetProfit = netProfit;
        GrossProfit = grossProfit;
        Balance = balance;
    }
}