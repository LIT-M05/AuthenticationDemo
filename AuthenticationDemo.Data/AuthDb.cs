using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationDemo.Data
{
    public class AuthDb
    {
        private string _connectionString;

        public AuthDb(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddUser(User user, string password)
        {
            string salt = PasswordHelper.GenerateSalt();
            string passwordHash = PasswordHelper.HashPassword(password, salt);
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO Users (Name, Email, PasswordHash, PasswordSalt) " +
                                  "VALUES (@name, @email, @hash, @salt)";
                cmd.Parameters.AddWithValue("@name", user.Name);
                cmd.Parameters.AddWithValue("@email", user.Email);
                cmd.Parameters.AddWithValue("@hash", passwordHash);
                cmd.Parameters.AddWithValue("@salt", salt);
                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public User Login(string email, string password)
        {
            var user = GetByEmail(email);
            if (user == null)
            {
                return null;
            }

            var doesMatch = PasswordHelper.PasswordMatch(password, user.PasswordSalt, user.PasswordHash);
            if (doesMatch)
            {
                return user;
            }

            return null;
        }

        public User GetByEmail(string email)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT TOP 1 * FROM Users WHERE Email = @email";
                cmd.Parameters.AddWithValue("@email", email);
                connection.Open();
                var reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    return null;
                }

                return new User
                {
                    Id = (int) reader["Id"],
                    Name = (string) reader["Name"],
                    Email = (string) reader["Email"],
                    PasswordSalt = (string) reader["PasswordSalt"],
                    PasswordHash = (string) reader["PasswordHash"]
                };
            }
        }
    }
}
