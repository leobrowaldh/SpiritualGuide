using Azure.Data.Tables;
using FunctionApp.Models.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp.Services;
public interface IDbService
{
    Task AddTableEntitiesAsync(List<TableData> entities);
    Task<List<TableData>> GetAllQuotesAsync();
}
