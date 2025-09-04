using MySql.Data.MySqlClient;
using PathFinders.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinders.Services
{
    public class MySqlService : IDatabaseService
    {
        private readonly string _connectionString;

        public MySqlService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        public void AddClient(Client client)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand("INSERT INTO Clients (FirstName, LastName, PassportNumber, DateOfBirth, Email, PhoneNumber) VALUES (@firstName, @lastName, @passportNumber, @dateOfBirth, @email, @phoneNumber)", (MySqlConnection)connection);
                command.Parameters.AddWithValue("@firstName", client.FirstName);
                command.Parameters.AddWithValue("@lastName", client.LastName);
                command.Parameters.AddWithValue("@passportNumber", PasswordHasher.HashPassword(client.PassportNumber));
                command.Parameters.AddWithValue("@dateOfBirth", client.DateOfBirth);
                command.Parameters.AddWithValue("@email", client.Email);
                command.Parameters.AddWithValue("@phoneNumber", client.PhoneNumber);
                command.ExecuteNonQuery();
            }
        }
        public void AddPackage(TravelPackage package)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand("INSERT INTO Packages (Name, Price, Type, Details) VALUES (@name, @price, @type, @details)", (MySqlConnection)connection);
                command.Parameters.AddWithValue("@name", package.Name);
                command.Parameters.AddWithValue("@price", package.Price);
                command.Parameters.AddWithValue("@type", package.Type);
                command.Parameters.AddWithValue("@details", package.Details);
                command.ExecuteNonQuery();
            }
        }
        public void AddReservation(Reservation reservation)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand("INSERT INTO Reservations (ClientId, PackageId, ReservationDate) VALUES (@clientId, @travelPackageId, @reservationDate)", (MySqlConnection)connection);
                command.Parameters.AddWithValue("@clientId", reservation.ClientId);
                command.Parameters.AddWithValue("@travelPackageId", reservation.TravelPackageId);
                command.Parameters.AddWithValue("@reservationDate", reservation.ReservationDate);
                command.ExecuteNonQuery();
            }
        }
        public DataTable GetClients()
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var adapter = new MySqlDataAdapter("SELECT * FROM Clients", (MySqlConnection)connection);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
        }
        public DataTable GetPackages()
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var adapter = new MySqlDataAdapter("SELECT * FROM Packages", (MySqlConnection)connection);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
        }
        public DataTable GetReservationsForClient(int clientId)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var adapter = new MySqlDataAdapter("SELECT T.*, R.ReservationDate FROM Reservations R INNER JOIN Packages T ON R.PackageId = T.Id WHERE R.ClientId = @clientId", (MySqlConnection)connection);
                adapter.SelectCommand.Parameters.AddWithValue("@clientId", clientId);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
        }
        public void UpdateClient(Client client)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand("UPDATE Clients SET FirstName = @firstName, LastName = @lastName, PassportNumber = @passportNumber, DateOfBirth = @dateOfBirth, Email = @email, PhoneNumber = @phoneNumber WHERE Id = @id", (MySqlConnection)connection);
                command.Parameters.AddWithValue("@firstName", client.FirstName);
                command.Parameters.AddWithValue("@lastName", client.LastName);
                command.Parameters.AddWithValue("@passportNumber", PasswordHasher.HashPassword(client.PassportNumber));
                command.Parameters.AddWithValue("@dateOfBirth", client.DateOfBirth);
                command.Parameters.AddWithValue("@email", client.Email);
                command.Parameters.AddWithValue("@phoneNumber", client.PhoneNumber);
                command.Parameters.AddWithValue("@id", client.Id);
                command.ExecuteNonQuery();
            }
        }
        public void UpdateReservation(Reservation reservation)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand("UPDATE Reservations SET ClientId = @clientId, PackageId = @travelPackageId, ReservationDate = @reservationDate WHERE Id = @id", (MySqlConnection)connection);
                command.Parameters.AddWithValue("@clientId", reservation.ClientId);
                command.Parameters.AddWithValue("@travelPackageId", reservation.TravelPackageId);
                command.Parameters.AddWithValue("@reservationDate", reservation.ReservationDate);
                command.Parameters.AddWithValue("@id", reservation.Id);
                command.ExecuteNonQuery();
            }
        }
        public void DeleteReservation(int reservationId)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand("DELETE FROM Reservations WHERE Id = @id", (MySqlConnection)connection);
                command.Parameters.AddWithValue("@id", reservationId);
                command.ExecuteNonQuery();
            }
        }
        public DataTable GetAvailableDestinations()
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var adapter = new MySqlDataAdapter("SELECT DISTINCT JSON_UNQUOTE(JSON_EXTRACT(Details, '$.Destinacija')) AS Destination FROM Packages WHERE JSON_EXTRACT(Details, '$.Destinacija') IS NOT NULL", (MySqlConnection)connection);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
        }
    }
}
