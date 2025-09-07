using MySql.Data.MySqlClient;
using PathFinders.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;

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
            var connection = new SQLiteConnection(_connectionString);
            connection.Open();
            var command = new SQLiteCommand("PRAGMA foreign_keys = ON;", connection);
            command.ExecuteNonQuery();
            return connection; // Vraćanje iste instance
        }

        private void CreateDatabaseAndTables()
        {
            if (!File.Exists(_connectionString.Split('=')[1]))
            {
                SQLiteConnection.CreateFile(_connectionString.Split('=')[1]);
            }

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                // Create Clients table
                string createClientsTable = @"
                CREATE TABLE IF NOT EXISTS Clients (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Ime TEXT NOT NULL,
                    Prezime TEXT NOT NULL,
                    Broj_pasosa BLOB UNIQUE NOT NULL,
                    Datum_rodjenja TEXT,
                    Email_adresa TEXT,
                    Broj_telefona TEXT
                )";
                var command = new SQLiteCommand(createClientsTable, connection);
                command.ExecuteNonQuery();

                // Create Destinations table
                string createDestinationsTable = @"
                CREATE TABLE IF NOT EXISTS Destinations (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Naziv_destinacije TEXT UNIQUE NOT NULL
                )";
                command = new SQLiteCommand(createDestinationsTable, connection);
                command.ExecuteNonQuery();

                // Create TravelPackages table
                string createPackagesTable = @"
                CREATE TABLE IF NOT EXISTS Packages (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Naziv TEXT NOT NULL,
                    Cena REAL NOT NULL,
                    Tip TEXT,
                    DestinacijaID INTEGER,
                    Detalji TEXT,
                    FOREIGN KEY (DestinacijaID) REFERENCES Destinations(ID) ON DELETE SET NULL
                )";
                command = new SQLiteCommand(createPackagesTable, connection);
                command.ExecuteNonQuery();

                // Create Services table
                string createServicesTable = @"
                CREATE TABLE IF NOT EXISTS Services (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    ServiceName TEXT NOT NULL,
                    ServicePrice REAL NOT NULL,
                    ServiceDescription TEXT
                )";
                command = new SQLiteCommand(createServicesTable, connection);
                command.ExecuteNonQuery();

                // Create Reservations table
                string createReservationsTable = @"
                CREATE TABLE IF NOT EXISTS Reservations (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    ClientID INTEGER,
                    TravelPackageID INTEGER,
                    Reservation_date TEXT,
                    NumberOfPeople INTEGER NOT NULL DEFAULT 1,
                    FOREIGN KEY (ClientID) REFERENCES Clients(ID) ON DELETE CASCADE,
                    FOREIGN KEY (TravelPackageID) REFERENCES Packages(ID) ON DELETE CASCADE
                )";
                command = new SQLiteCommand(createReservationsTable, connection);
                command.ExecuteNonQuery();

                // Create ReservationServiceAssociations table
                string createReservationServiceAssociationsTable = @"
                CREATE TABLE IF NOT EXISTS ReservationServiceAssociations (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    ReservationID INTEGER NOT NULL,
                    ServiceID INTEGER NOT NULL,
                    FOREIGN KEY (ReservationID) REFERENCES Reservations(ID) ON DELETE CASCADE,
                    FOREIGN KEY (ServiceID) REFERENCES Services(ID) ON DELETE CASCADE
                )";
                command = new SQLiteCommand(createReservationServiceAssociationsTable, connection);
                command.ExecuteNonQuery();
            }
        }

        public void AddClient(Client client)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = "INSERT INTO Clients (Ime, Prezime, Broj_pasosa, Datum_rodjenja, Email_adresa, Broj_telefona) VALUES (@ime, @prezime, @passos, @datumRodjenja, @email, @telefon)";
                var command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@ime", client.FirstName);
                command.Parameters.AddWithValue("@prezime", client.LastName);
                command.Parameters.AddWithValue("@passos", PasswordHasher.HashPassword(client.PassportNumber));
                command.Parameters.AddWithValue("@datumRodjenja", client.DateOfBirth.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@email", client.Email);
                command.Parameters.AddWithValue("@telefon", client.PhoneNumber);
                command.ExecuteNonQuery();
            }
        }

        public void AddPackage(TravelPackage package)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = "INSERT INTO Packages (Naziv, Cena, Tip, DestinacijaID, Detalji) VALUES (@naziv, @cena, @tip, @destinacijaId, @detalji)";
                var command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@naziv", package.Name);
                command.Parameters.AddWithValue("@cena", package.Price);
                command.Parameters.AddWithValue("@tip", package.Type);
                command.Parameters.AddWithValue("@destinacijaId", package.DestinationId);
                command.Parameters.AddWithValue("@detalji", package.Details);
                command.ExecuteNonQuery();
            }
        }

        public void AddReservation(Reservation reservation)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = "INSERT INTO Reservations (ClientID, TravelPackageID, Reservation_date, NumberOfPeople) VALUES (@clientId, @travelPackageId, @reservationDate, @numberOfPeople)";
                var command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@clientId", reservation.ClientId);
                command.Parameters.AddWithValue("@travelPackageId", reservation.TravelPackageId);
                command.Parameters.AddWithValue("@reservationDate", reservation.ReservationDate.ToString("yyyy-MM-dd HH:mm:ss"));
                command.Parameters.AddWithValue("@numberOfPeople", reservation.NumberOfPeople);
                command.ExecuteNonQuery();
            }
        }

        public void AddService(Service service)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = "INSERT INTO Services (ServiceName, ServicePrice, ServiceDescription) VALUES (@serviceName, @servicePrice, @serviceDescription)";
                var command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@serviceName", service.ServiceName);
                command.Parameters.AddWithValue("@servicePrice", service.ServicePrice);
                command.Parameters.AddWithValue("@serviceDescription", service.ServiceDescription);
                command.ExecuteNonQuery();
            }
        }

        public void AddReservationService(ReservationServiceAssociation reservationService)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = "INSERT INTO ReservationServiceAssociations (ReservationID, ServiceID) VALUES (@reservationId, @serviceId)";
                var command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@reservationId", reservationService.ReservationId);
                command.Parameters.AddWithValue("@serviceId", reservationService.ServiceId);
                command.ExecuteNonQuery();
            }
        }

        public DataTable GetClients()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var adapter = new SQLiteDataAdapter("SELECT ID, Ime, Prezime, Datum_rodjenja, Email_adresa, Broj_telefona FROM Clients", connection);
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
                var adapter = new SQLiteDataAdapter("SELECT p.ID, p.Naziv, p.Cena, p.Tip, p.Detalji, d.Naziv_destinacije FROM Packages p LEFT JOIN Destinations d ON p.DestinacijaID = d.ID", connection);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
        }

        public TravelPackage GetPackageById(int packageId)
        {
            TravelPackage package = null;
            using (var connection = (SQLiteConnection)GetConnection()) // Or SQLiteConnection for SQLiteService
            {
                connection.Open();
                var command = new SQLiteCommand("SELECT * FROM Packages WHERE ID = @packageId", connection); // Or SQLiteCommand
                command.Parameters.AddWithValue("@packageId", packageId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        package = new TravelPackage
                        {
                            Id = reader.GetInt32("ID"),
                            Name = reader.GetString("Naziv"),
                            Price = reader.GetDecimal("Cena"),
                            Type = reader.GetString("Tip"),
                            DestinationId = reader.GetInt32("DestinacijaID"),
                            Details = reader.GetString("Detalji")
                        };
                    }
                }
            }
            return package;
        }

        public DataTable GetReservationsForClient(int clientId)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = @"
                SELECT
                    r.ID,
                    r.ClientID,
                    r.TravelPackageID,
                    r.Reservation_date,
                    p.Naziv,
                    p.Cena,
                    p.Tip,
                    d.Naziv_destinacije
                FROM Reservations r
                JOIN Packages p ON r.TravelPackageID = p.ID
                LEFT JOIN Destinations d ON p.DestinacijaID = d.ID
                WHERE r.ClientID = @clientId";
                var command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@clientId", clientId);
                var adapter = new SQLiteDataAdapter(command);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
        }

        public Reservation GetReservationById(int reservationId)
        {
            Reservation reservation = null;
            using (var connection = (SQLiteConnection)GetConnection()) // Or SQLiteConnection
            {
                connection.Open();
                var command = new SQLiteCommand("SELECT * FROM Reservations WHERE ID = @reservationId", connection); // Or SQLiteCommand
                command.Parameters.AddWithValue("@reservationId", reservationId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        reservation = new Reservation
                        {
                            Id = reader.GetInt32("ID"),
                            ClientId = reader.GetInt32("KlijentID"),
                            TravelPackageId = reader.GetInt32("PaketID"),
                            ReservationDate = reader.GetDateTime("Datum_rezervacije"),
                            NumberOfPeople = reader.GetInt32("Broj_osoba")
                        };
                    }
                }
            }
            return reservation;
        }

        public Client GetClientByPassportNumber(string passportNumber)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var command = new SQLiteCommand("SELECT ID, Ime, Prezime, Broj_pasosa, Datum_rodjenja, Email_adresa, Broj_telefona FROM Clients WHERE Broj_pasosa = @passportHash", connection);
                command.Parameters.AddWithValue("@passportHash", PasswordHasher.HashPassword(passportNumber));

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Client
                        {
                            Id = reader.GetInt32(0),
                            FirstName = reader.GetString(1),
                            LastName = reader.GetString(2),
                            PassportNumber = passportNumber,
                            DateOfBirth = DateTime.Parse(reader.GetString(4)),
                            Email = reader.GetString(5),
                            PhoneNumber = reader.GetString(6)
                        };
                    }
                }
            }
            return null;
        }

        public int GetOrCreateDestination(string destinationName)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                // Check if destination exists
                var checkCommand = new SQLiteCommand("SELECT ID FROM Destinations WHERE Naziv_destinacije = @naziv", connection);
                checkCommand.Parameters.AddWithValue("@naziv", destinationName);
                var result = checkCommand.ExecuteScalar();
                if (result != null)
                {
                    return Convert.ToInt32(result);
                }
                else
                {
                    // If not, insert new destination
                    var insertCommand = new SQLiteCommand("INSERT INTO Destinations (Naziv_destinacije) VALUES (@naziv)", connection);
                    insertCommand.Parameters.AddWithValue("@naziv", destinationName);
                    insertCommand.ExecuteNonQuery();

                    // Return the new ID
                    return (int)connection.LastInsertRowId;
                }
            }
        }

        public List<Reservation> GetReservationsForPackage(int packageId)
        {
            var reservations = new List<Reservation>();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var command = new SQLiteCommand("SELECT * FROM Reservations WHERE PaketID = @packageId", connection);
                command.Parameters.AddWithValue("@packageId", packageId);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        reservations.Add(new Reservation
                        {
                            Id = reader.GetInt32(0),
                            ClientId = reader.GetInt32(1),
                            TravelPackageId = reader.GetInt32(2),
                            ReservationDate = reader.GetDateTime(3),
                            NumberOfPeople = reader.GetInt32(4)
                        });
                    }
                }
            }
            return reservations;
        }

        public void UpdateClient(Client client)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var command = new SQLiteCommand("UPDATE Clients SET Ime = @ime, Prezime = @prezime, Datum_rodjenja = @datumRodjenja, Email_adresa = @email, Broj_telefona = @telefon WHERE ID = @id", connection);
                command.Parameters.AddWithValue("@ime", client.FirstName);
                command.Parameters.AddWithValue("@prezime", client.LastName);
                command.Parameters.AddWithValue("@datumRodjenja", client.DateOfBirth.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@email", client.Email);
                command.Parameters.AddWithValue("@telefon", client.PhoneNumber);
                command.Parameters.AddWithValue("@id", client.Id);
                command.ExecuteNonQuery();
            }
        }

        public void UpdateReservation(Reservation reservation)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var command = new SQLiteCommand("UPDATE Reservations SET ClientID = @clientId, TravelPackageID = @travelPackageId, Reservation_date = @reservationDate, NumberOfPeople = @numberOfPeople WHERE ID = @id", connection);
                command.Parameters.AddWithValue("@clientId", reservation.ClientId);
                command.Parameters.AddWithValue("@travelPackageId", reservation.TravelPackageId);
                command.Parameters.AddWithValue("@reservationDate", reservation.ReservationDate.ToString("yyyy-MM-dd HH:mm:ss"));
                command.Parameters.AddWithValue("@numberOfPeople", reservation.NumberOfPeople);
                command.Parameters.AddWithValue("@id", reservation.Id);
                command.ExecuteNonQuery();
            }
        }

        public void UpdateService(Service service)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var command = new SQLiteCommand("UPDATE Services SET ServiceName = @serviceName, ServicePrice = @servicePrice, ServiceDescription = @serviceDescription WHERE ID = @id", connection);
                command.Parameters.AddWithValue("@serviceName", service.ServiceName);
                command.Parameters.AddWithValue("@servicePrice", service.ServicePrice);
                command.Parameters.AddWithValue("@serviceDescription", service.ServiceDescription);
                command.Parameters.AddWithValue("@id", service.Id);
                command.ExecuteNonQuery();
            }
        }

        // U klasi SQLiteService
        public void UpdatePackage(TravelPackage package)
        {
            using (var connection = (SQLiteConnection)GetConnection())
            {
                connection.Open();
                string query = "UPDATE Packages SET Naziv = @name, Cena = @price, Tip = @type, DestinacijaID = @destinationId, Detalji = @details WHERE ID = @id";
                var command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@name", package.Name);
                command.Parameters.AddWithValue("@price", package.Price);
                command.Parameters.AddWithValue("@type", package.Type);
                command.Parameters.AddWithValue("@destinationId", package.DestinationId);
                command.Parameters.AddWithValue("@details", package.Details);
                command.Parameters.AddWithValue("@id", package.Id);
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

        public void DeleteService(int serviceId)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var command = new SQLiteCommand("DELETE FROM Services WHERE ID = @id", connection);
                command.Parameters.AddWithValue("@id", serviceId);
                command.ExecuteNonQuery();
            }
        }

        public void DeleteReservationService(int reservationServiceId)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var command = new SQLiteCommand("DELETE FROM ReservationServiceAssociations WHERE ID = @id", connection);
                command.Parameters.AddWithValue("@id", reservationServiceId);
                command.ExecuteNonQuery();
            }
        }

        public void DeleteServicesForReservation(int reservationId)
        {
            using (var connection = (SQLiteConnection)GetConnection())
            {
                connection.Open();
                var command = new SQLiteCommand("DELETE FROM ReservationServiceAssociations WHERE ReservationID = @reservationId", connection);
                command.Parameters.AddWithValue("@reservationId", reservationId);
                command.ExecuteNonQuery();
            }
        }

        public void DeletePackage(int packageId)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var command = new SQLiteCommand("DELETE FROM Packages WHERE ID = @packageId", connection);
                command.Parameters.AddWithValue("@packageId", packageId);
                command.ExecuteNonQuery();
            }
        }

        public DataTable GetAvailableDestinations()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var adapter = new SQLiteDataAdapter("SELECT DISTINCT Naziv_destinacije FROM Destinations", connection);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
        }

        public DataTable GetServices()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var adapter = new SQLiteDataAdapter("SELECT ID, ServiceName, ServicePrice, ServiceDescription FROM Services", connection);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
        }

        public List<Service> GetServicesForReservation(int reservationId)
        {
            var services = new List<Service>();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var command = new SQLiteCommand(@"
                    SELECT s.ID, s.ServiceName, s.ServicePrice, s.ServiceDescription 
                    FROM Services s
                    JOIN ReservationServiceAssociations rsa ON s.ID = rsa.ServiceID
                    WHERE rsa.ReservationID = @reservationId
                ", connection);
                command.Parameters.AddWithValue("@reservationId", reservationId);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        services.Add(new Service
                        {
                            Id = reader.GetInt32(0),
                            ServiceName = reader.GetString(1),
                            ServicePrice = reader.GetDecimal(2),
                            ServiceDescription = reader.GetString(3)
                        });
                    }
                }
            }
            return services;
        }
    }
}