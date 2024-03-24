using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;


namespace Turbine
{
    public class TurbineFunction
    {
        private readonly ILogger<TurbineFunction> _logger;

        const double revenuePerkW = 0.12;
        const double technicianCost = 250;
        const double turbineCost = 100;

        public TurbineFunction(ILogger<TurbineFunction> logger)
        {
            _logger = logger;
        }

        [Function(nameof(FixTurbine))]
        public async Task<IActionResult> FixTurbine([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonSerializer.Deserialize<RequestBodyModel>(requestBody) ?? throw new InvalidOperationException("request is not valid");

            int? capacity = data?.capacity;
            int? hours = data?.hours;

            double? revenueOpportunity = capacity * revenuePerkW * 24;
            double? costToFix = (hours * technicianCost) + turbineCost;
            string repairTurbine;

            if (revenueOpportunity > costToFix)
            {
                repairTurbine = "Yes";
            }
            else
            {
                repairTurbine = "No";
            };

            _logger.LogInformation("C# HTTP trigger function processed a request.");

            return new OkObjectResult(new
            {
                message = repairTurbine,
                revenueOpportunity = "$" + revenueOpportunity,
                costToFix = "$" + costToFix
            });
        }
        public class RequestBodyModel
        {
            public int Hours { get; set; }
            public int Capacity { get; set; }
        }
    }
}
