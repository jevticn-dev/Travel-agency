using PathFinders.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO.Packaging;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PathFinders.Services
{
    public class SQLiteService : IDatabaseService
    {
        private readonly string _connectionString;

        public SQLiteService(string connectionString)
        {
            _connectionString = connectionString;
            CreateDatabaseAndTables();
        }

        public IDbConnection GetConnection()
        {
            return new SQLiteConnection(_connectionString);
        }

        private void CreateDatabaseAndTables()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string createClientsTable = @"
                CREATE TABLE IF NOT EXISTS Clients (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Ime TEXT NOT NULL,
                    Prezime TEXT NOT NULL,
                    Broj_pasosa TEXT UNIQUE NOT NULL,
                    Datum_rodjenja TEXT,
                    Email_adresa TEXT UNIQUE,
                    Broj_telefona TEXT
                );";

                string createPackagesTable = @"
                CREATE TABLE IF NOT EXISTS Packages (
                    ID_paketa INTEGER PRIMARY KEY AUTOINCREMENT,
                    Naziv_paketa TEXT NOT NULL,
                    Cena REAL NOT NULL,
                    Vrsta_paketa TEXT NOT NULL,
                    Detalji_paketa TEXT
                );";

                string createReservationsTable = @"
                CREATE TABLE IF NOT EXISTS Reservations (
                    ReservationID INTEGER PRIMARY KEY AUTOINCREMENT,
                    ClientID INTEGER,
                    PackageID INTEGER,
                    Reservation_date TEXT NOT NULL,
                    FOREIGN KEY (ClientID) REFERENCES Clients(ID),
                    FOREIGN KEY (PackageID) REFERENCES Packages(ID_paketa)
                );";

                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = createClientsTable;
                    command.ExecuteNonQuery();

                    command.CommandText = createPackagesTable;
                    command.ExecuteNonQuery();

                    command.CommandText = createReservationsTable;
                    command.ExecuteNonQuery();
                }
            }
        }

        public void AddClient(Client client)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var command = new SQLiteCommand("INSERT INTO Clients (Ime, Prezime, Broj_pasosa, Datum_rodjenja, Email_adresa, Broj_telefona) VALUES (@firstName, @lastName, @passportNumber, @dateOfBirth, @email, @phoneNumber)", connection);
                command.Parameters.AddWithValue("@firstName", client.FirstName);
                command.Parameters.AddWithValue("@lastName", client.LastName);
                command.Parameters.AddWithValue("@passportNumber", PasswordHasher.HashPassword(client.PassportNumber));
                command.Parameters.AddWithValue("@dateOfBirth", client.DateOfBirth.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@email", client.Email);
                command.Parameters.AddWithValue("@phoneNumber", client.PhoneNumber);
                command.ExecuteNonQuery();
            }
        }
        public void AddPackage(TravelPackage package)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var command = new SQLiteCommand("INSERT INTO Packages (Name, Price, Type, Details) VALUES (@name, @price, @type, @details)", connection);
                command.Parameters.AddWithValue("@name", package.Name);
                command.Parameters.AddWithValue("@price", package.Price);
                command.Parameters.AddWithValue("@type", package.Type);
                command.Parameters.AddWithValue("@details", package.Details);
                command.ExecuteNonQuery();
            }
        }
        public void AddReservation(Reservation reservation)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var command = new SQLiteCommand("INSERT INTO Reservations (ClientId, PackageId, ReservationDate) VALUES (@clientId, @travelPackageId, @reservationDate)", connection);
                command.Parameters.AddWithValue("@clientId", reservation.ClientId);
                command.Parameters.AddWithValue("@travelPackageId", reservation.TravelPackageId);
                command.Parameters.AddWithValue("@reservationDate", reservation.ReservationDate.ToString("yyyy-MM-dd HH:mm:ss"));
                command.ExecuteNonQuery();
            }
        }
        public DataTable GetClients()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var adapter = new SQLiteDataAdapter("SELECT * FROM Clients", connection);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
        }
        public DataTable GetPackages()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var adapter = new SQLiteDataAdapter("SELECT * FROM Packages", connection);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
        }
        public DataTable GetReservationsForClient(int clientId)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var adapter = new SQLiteDataAdapter("SELECT T.*, R.ReservationDate FROM Reservations R INNER JOIN Packages T ON R.PackageId = T.ID WHERE R.ClientId = @clientId", connection);
                adapter.SelectCommand.Parameters.AddWithValue("@clientId", clientId);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
        }
        public void UpdateClient(Client client)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var command = new SQLiteCommand("UPDATE Clients SET Ime = @firstName, Prezime = @lastName, Broj_pasosa = @passportNumber, Datum_rodjenja = @dateOfBirth, Email_adresa = @email, Broj_telefona = @phoneNumber WHERE ID = @id", connection);
                command.Parameters.AddWithValue("@firstName", client.FirstName);
                command.Parameters.AddWithValue("@lastName", client.LastName);
                command.Parameters.AddWithValue("@passportNumber", PasswordHasher.HashPassword(client.PassportNumber));
                command.Parameters.AddWithValue("@dateOfBirth", client.DateOfBirth.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@email", client.Email);
                command.Parameters.AddWithValue("@phoneNumber", client.PhoneNumber);
                command.Parameters.AddWithValue("@id", client.Id);
                command.ExecuteNonQuery();
            }
        }
        public void UpdateReservation(Reservation reservation)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var command = new SQLiteCommand("UPDATE Reservations SET ClientId = @clientId, PackageId = @travelPackageId, ReservationDate = @reservationDate WHERE ID = @id", connection);
                command.Parameters.AddWithValue("@clientId", reservation.ClientId);
                command.Parameters.AddWithValue("@travelPackageId", reservation.TravelPackageId);
                command.Parameters.AddWithValue("@reservationDate", reservation.ReservationDate.ToString("yyyy-MM-dd HH:mm:ss"));
                command.Parameters.AddWithValue("@id", reservation.Id);
                command.ExecuteNonQuery();
            }
        }
        public void DeleteReservation(int reservationId)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var command = new SQLiteCommand("DELETE FROM Reservations WHERE ID = @id", connection);
                command.Parameters.AddWithValue("@id", reservationId);
                command.ExecuteNonQuery();
            }
        }
        public DataTable GetAvailableDestinations()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var adapter = new SQLiteDataAdapter("SELECT DISTINCT json_extract(Details, '$.Destinacija') AS Destination FROM Packages WHERE json_extract(Details, '$.Destinacija') IS NOT NULL", connection);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
        }
    }
}
