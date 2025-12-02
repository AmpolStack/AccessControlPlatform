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
        SqlParameter OutBit(string name);

        SqlParameter OutString(string name, int size);

        (bool Success, string Message) ReadOutput(SqlParameter result, SqlParameter message);
    }
}
