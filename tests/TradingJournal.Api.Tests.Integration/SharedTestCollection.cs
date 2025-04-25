namespace TradingJournal.Api.Tests.Integration;

[CollectionDefinition("Test collection")]
[TestCaseOrderer(
    ordererTypeName: "TradingJournal.Api.Tests.Integration.PriorityOrderer",
    ordererAssemblyName: "TradingJournal.Api.Tests.Integration")]
[assembly: CollectionBehavior(DisableTestParallelization = true)]
public class SharedTestCollection : ICollectionFixture<TradingJournalApiFactory>
{
}