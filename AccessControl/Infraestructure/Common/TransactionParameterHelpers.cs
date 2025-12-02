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
        public SqlParameter CreateInput(string name, object? value)
        {
            return new SqlParameter(name, value ?? DBNull.Value);
        }

        public SqlParameter CreateNullable(string name, object? value)
        {
            return new SqlParameter(name, value ?? DBNull.Value);
        }

        public SqlParameter CreateOutput(string name, SqlDbType type, int? size = null)
        {
            var p = new SqlParameter(name, type)
            {
                Direction = ParameterDirection.Output
            };

            if (size.HasValue)
                p.Size = size.Value;

            return p;
        }
    
    }
}
