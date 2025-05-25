using FunctionApp.Models.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace FunctionApp.Models.FunctionReturnModels;

public class AddQuoteReturnModel
{
    public required IActionResult ActionResult { get; set; }

    [TableOutput("Quotes")]
    //returning null or emty list writes no data to table.
    public List<TableData>? TableDatas { get; set; }
}
