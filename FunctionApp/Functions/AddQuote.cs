using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker.Extensions.Tables;
using FunctionApp.Models.Storage;

namespace FunctionApp.Functions;

public class AddQuote
{
    private readonly ILogger<AddQuote> _logger;

    public AddQuote(ILogger<AddQuote> logger)
    {
        _logger = logger;
    }

    [Function(nameof(AddQuote))]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("Http trigger - Adding Quote");
        TableData tableData = new TableData()
        {
            Quote = "My newly added string quote"
        };
        return new OkObjectResult("Welcome to Azure Functions!");
    }
}