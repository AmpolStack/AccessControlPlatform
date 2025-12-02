using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.Core.Interfaces.Common
{
    public interface ITransactionParameterHelpers
    {
        SqlParameter CreateInput(string name, object? value);
        SqlParameter CreateOutput(string name, SqlDbType type, int? size = null);
        SqlParameter CreateNullable(string name, object? value);
    }
}
