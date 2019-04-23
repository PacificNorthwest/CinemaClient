using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CinemaServer.Models;
using System.Data.SqlClient;
using System.Security.Cryptography;
using CinemaServer;

namespace CinemaServer.SQL
{
    public class DBManager
    {
        public static IEnumerable<Movie> LoadMovieList()
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = string.Empty;
                connection.Open();

                if (connection.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = "Select ID, Title, EnglishTitle, Director, Country, Length, Genres, Description, Trailer, Is3D, Poster from Movies";
                    SqlDataReader reader = command.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    while (reader.Read())
                        yield return new Movie(Convert.ToInt32(reader["ID"]), reader["Title"].ToString())
                            {
                                Director = reader["Director"].ToString(),
                                Country = reader["Country"].ToString(),
                                Length = Convert.ToInt32(reader["Length"]),
                                Genres = reader["Genres"].ToString(),
                                Description = reader["Description"].ToString(),
                                Trailer = reader["Trailer"].ToString(),
                                Is3D = Convert.ToBoolean(reader["Is3D"]),
                                //IMDbRating = OMDbRequest.GetRating(reader["EnglishTitle"].ToString().Replace(' ', '+')),
                                ShowDays = LoadSessionsCollection(connection, Convert.ToInt32(reader["ID"])),
                                Poster = reader["Poster"] as byte[]
                            };
                }
            }
        }

        public static IEnumerable<int> LoadUserTickets(string token)
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = string.Empty;
                connection.Open();

                if (connection.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = $"Select Id from Users where Token='{token}'";
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    var id = (int)reader["Id"];
                    reader.Close();
                    command.CommandText = $"Select Id from Booking where UserId = {id} and IsRedeemed = 'false'";
                    reader = command.ExecuteReader();
                    while (reader.Read())
                        yield return (int)reader["Id"];
                }
            }
        }

        private static List<Day> LoadSessionsCollection(SqlConnection connection, int id)
        {
            List<Day> showDays = new List<Day>();
            SqlCommand command = connection.CreateCommand();
            command.CommandText = $"Select Day from Sessions where MovieID = {id}";
            SqlDataReader dayReader = command.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
            while (dayReader.Read())
            {
                if (!showDays.Exists(day => day.Date == (DateTime)dayReader["Day"]))
                {
                    Day day = new Day((DateTime)dayReader["Day"]);
                    SqlCommand sessionSelectCommand = connection.CreateCommand();
                    sessionSelectCommand.CommandText = $"Select Id, SessionTime, Hall from Sessions where MovieId ={id} and Day = @date";
                    sessionSelectCommand.Parameters.AddWithValue("@date", day.Date);
                    SqlDataReader sessionReader = sessionSelectCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    while (sessionReader.Read())
                    {
                        day.Sessions.Add(new Session(Convert.ToInt32(sessionReader["Id"]),
                                                     Convert.ToDateTime(sessionReader["SessionTime"].ToString()).ToString("HH:mm"),
                                                     Convert.ToInt32(sessionReader["Hall"]))
                                                     { BookedSeats = LoadBookedSeatsForSession(connection, Convert.ToInt32(sessionReader["Id"])) });
                    }
                    showDays.Add(day);
                }
            }
            return showDays;
        }

        private static List<Seat> LoadBookedSeatsForSession(SqlConnection connection, int id)
        {
            List<Seat> seats = new List<Seat>();
            SqlCommand command = connection.CreateCommand();
            command.CommandText = $"Select SeatId from Booking where SessionID = {id}";
            SqlDataReader sessionSeatsReader = command.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
            while (sessionSeatsReader.Read())
            {
                SqlCommand selectSeatsCommand = connection.CreateCommand();
                selectSeatsCommand.CommandText = $"Select Hall, Row, Number from Seats where Id = {Convert.ToInt32(sessionSeatsReader["SeatId"])}";
                SqlDataReader seatInfoReader = selectSeatsCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                seatInfoReader.Read();
                seats.Add(new Seat(Convert.ToInt32(sessionSeatsReader["SeatId"]),
                                   Convert.ToInt32(seatInfoReader["Hall"]),
                                   Convert.ToInt32(seatInfoReader["Row"]),
                                   Convert.ToInt32(seatInfoReader["Number"])));
            }
            return seats;
        }

        public static bool SignUpNewUser(string email, string hash, string cardNumber, string expDate, string cvv)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection())
                {
                    connection.ConnectionString = string.Empty;
                    connection.Open();

                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        SqlCommand command = connection.CreateCommand();
                        command.CommandText = $"Insert into Users (Email, Token, Hash, CardNumber, ExpDate, CVV) values ('{email}', '{BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(Hex.StringToByteArray(hash))).Replace("-", string.Empty)}', '{hash}', '{cardNumber}', '{expDate}', '{cvv}')";
                        command.ExecuteNonQuery();
                        return true;
                    }
                    else return false;
                }
            }
            catch { return false; }
        }

        public static List<object> GetUserData(string userToken)
        {
            List<object> userData = new List<object>();
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = string.Empty;
                connection.Open();

                if (connection.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = $"Select Id, Hash, CardNumber, ExpDate, CVV from Users where Token = '{userToken}'";
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    for (int i = 0; i < reader.FieldCount; i++)
                        userData.Add(reader.GetFieldValue<object>(i));

                    return userData;

                }
                return new List<object>();
            }
        }

        public static void InsertBooking(string userId, string sessionId, string seatId)
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = string.Empty;
                connection.Open();

                if (connection.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = $"Insert into Booking (SessionId, SeatId, UserId, IsRedeemed) values ('{sessionId}', '{seatId}', '{userId}', 'false')";
                    command.ExecuteNonQuery();
                }
            }
        }

        public static bool VerifyUser(string email, string token)
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = string.Empty;
                connection.Open();

                if (connection.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = "Select count(*) from Users where Email like @email AND Token like @token";
                    command.Parameters.AddWithValue("@email", email);
                    command.Parameters.AddWithValue("@token", token);

                    return ((int)command.ExecuteScalar() == 1) ? true : false;
                }
                return false;
            }
        }

        public static string RedeemTicket(int id)
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = string.Empty;
                connection.Open();

                if (connection.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = $"Select IsRedeemed from Booking where Id = {id}";
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    if ((bool)reader["IsRedeemed"] == true) return "Ticket is already redeemed!";
                    else
                    {
                        reader.Close();
                        command.CommandText = $"Update Booking set IsRedeemed = 'true' where Id = {id}";
                        command.ExecuteNonQuery();
                        return "Ticket redeemed successfully!";
                    }
                }
                else return "Failed to redeem ticket! Server internal error.";
            }
        }
    }
}