using AccessControl.Domain.Services;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AccessControl.Infraestructure.Services
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
