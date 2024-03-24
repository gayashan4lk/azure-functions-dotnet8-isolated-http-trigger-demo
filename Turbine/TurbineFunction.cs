using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Turbine
{
    public class TurbineFunction
    {
        private readonly ILogger<TurbineFunction> _logger;

        public TurbineFunction(ILogger<TurbineFunction> logger)
        {
            _logger = logger;
        }

        [Function("TurbineFunction")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
