using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Net;
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

        [Function("TurbineFunction")]
        [OpenApiOperation(operationId: "Run")]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name="code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody("application/json", typeof(RequestBodyModel), Description = "JSON request body containing { hours, capacity}")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string),
            Description = "The OK response message containing a JSON result.")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
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
