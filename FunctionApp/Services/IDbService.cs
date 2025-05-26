using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp.Services;
internal interface IDbService
{
    Task AddTableEntitiesAsync(List<ITableEntity> entities);
}
