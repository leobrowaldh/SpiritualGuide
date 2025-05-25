using FunctionApp.Enums;

namespace FunctionApp.Models.FunctionRequestModels;
public class AddQuoteRequest
{
    public EnAuthor Author { get; set; }
    public List<string> Quotes { get; set; } = [];
}
