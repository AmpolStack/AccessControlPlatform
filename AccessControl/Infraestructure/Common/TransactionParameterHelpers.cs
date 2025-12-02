using AccessControl.Core.Interfaces.Common;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.Infraestructure.Common
{
    public class TransactionParameterHelpers : ITransactionParameterHelpers
    {
        public SqlParameter OutBit(string name)
        {
            return new SqlParameter(name, SqlDbType.Bit)
            {
                Direction = ParameterDirection.Output
            };
        }

        public SqlParameter OutString(string name, int size)
        {
            return new SqlParameter(name, SqlDbType.NVarChar, size)
            {
                Direction = ParameterDirection.Output
            };
        }

        public (bool Success, string Message) ReadOutput(SqlParameter result, SqlParameter message)
        {
            return (
                Convert.ToBoolean(result.Value),
                Convert.ToString(message.Value) ?? string.Empty
            );
        }
    }
}
