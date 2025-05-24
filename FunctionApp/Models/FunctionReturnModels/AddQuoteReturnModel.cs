using FunctionApp.Models.Storage;
using Microsoft.AspNetCore.Mvc;

namespace FunctionApp.Models.FunctionReturnModels;

public record AddQuoteReturnModel(IActionResult actionResult, TableData tableData);
