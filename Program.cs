using Amazon;
using Amazon.CostExplorer;
using Amazon.CostExplorer.Model;

// Get date range from the command line

if (args.Length < 2)
{
    Console.WriteLine("Usage: dotnet run -- [start-yyyy-mm-dd] [end-yyyy-mm-dd]");
}

var startDate = args[0];
var endDate = args[1];

// Query the Cost Explorer API

var client = new AmazonCostExplorerClient();
var request = new GetCostAndUsageRequest()
{
    Granularity = "MONTHLY",
    GroupBy = { new GroupDefinition() { Key = "SERVICE", Type = GroupDefinitionType.DIMENSION } },
    TimePeriod = new DateInterval()
    {
        Start = startDate,
        End = endDate
    },
    Metrics = { "BlendedCost" }
};

var response = await client.GetCostAndUsageAsync(request);

// Display results

foreach (var result in response.ResultsByTime)
{
    Console.WriteLine($"{result.TimePeriod.Start}-{result.TimePeriod.End}");
    foreach(var group in result.Groups)
    {
        foreach(var key in group.Keys)
        {
            var amount = group.Metrics.FirstOrDefault().Value.Amount;
            if (amount != "0")
            {
                Console.WriteLine($"    {key} {group.Metrics.FirstOrDefault().Value.Amount} {group.Metrics.FirstOrDefault().Value.Unit}  ");
            }
        }
    }
    Console.WriteLine();
}
