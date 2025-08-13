using api.Models.Storage;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Services;
public interface IDbService
{
    Task AddTableEntitiesAsync(List<TableData> entities);
    Task<List<TableData>> GetAllQuotesAsync();
}
