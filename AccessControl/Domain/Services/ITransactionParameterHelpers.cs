using Microsoft.Data.SqlClient;
using System.Data;

namespace AccessControl.Domain.Services
{
    public interface ITransactionParameterHelpers
    {
        SqlParameter CreateInput(string name, object? value);
        SqlParameter CreateOutput(string name, SqlDbType type, int? size = null);
        SqlParameter CreateNullable(string name, object? value);
    }
}
