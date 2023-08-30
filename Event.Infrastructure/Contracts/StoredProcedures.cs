using MySqlConnector;
using System;
using System.Data;
using System.Threading.Tasks;
using Event.Application.Contracts;

namespace Event.Infrastructure.Contracts
{
    public sealed class StoredProcedures : IStoredProcedures
    {
        private readonly string _connection_string;

        public StoredProcedures(string connection_string)
        {
            _connection_string = connection_string;
        }
        public async Task LogSystemErrorAsync(string error_message, string request_uri)
        {
            try
            {
                using (MySqlConnection mySqlConnection = new(_connection_string))
                {
                    mySqlConnection.Open();

                    MySqlCommand cmd = new("InsertErrorLog", mySqlConnection);

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Id", Guid.NewGuid());
                    cmd.Parameters.AddWithValue("@Message", error_message ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@RequestUri", request_uri ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DateLogged", DateTime.UtcNow.Date);
                    cmd.Parameters.AddWithValue("@TimeLogged", DateTime.UtcNow.TimeOfDay);

                    await cmd.ExecuteNonQueryAsync();

                    mySqlConnection.Close();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
