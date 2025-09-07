using MySql.Data.MySqlClient;
using PathFinders.Models;
using System;
using System.Collections.Generic;
using System.Data;
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
            CreateDatabaseAndTables();
        }

        public IDbConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        private void CreateDatabaseAndTables()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                // Create Clients table
                string createClientsTable = @"
        CREATE TABLE IF NOT EXISTS Clients (
            ID INT AUTO_INCREMENT PRIMARY KEY,
            Ime VARCHAR(255) NOT NULL,
            Prezime VARCHAR(255) NOT NULL,
             Broj_pasosa VARCHAR(255) UNIQUE NOT NULL,
            Datum_rodjenja DATE,
            Email_adresa VARCHAR(255),
            Broj_telefona VARCHAR(50)
        )";
                var command = new MySqlCommand(createClientsTable, connection);
                command.ExecuteNonQuery();

                // Create Destinations table
                string createDestinationsTable = @"
        CREATE TABLE IF NOT EXISTS Destinations (
            ID INT AUTO_INCREMENT PRIMARY KEY,
            Naziv_destinacije VARCHAR(255) UNIQUE NOT NULL
        )";
                command = new MySqlCommand(createDestinationsTable, connection);
                command.ExecuteNonQuery();

                // Create TravelPackages table
                string createPackagesTable = @"
        CREATE TABLE IF NOT EXISTS Packages (
            ID INT AUTO_INCREMENT PRIMARY KEY,
            Naziv VARCHAR(255) NOT NULL,
            Cena DECIMAL(10, 2) NOT NULL,
            Tip VARCHAR(50),
            DestinacijaID INT,
            Detalji JSON,
            FOREIGN KEY (DestinacijaID) REFERENCES Destinations(ID) ON DELETE SET NULL
        )";
                command = new MySqlCommand(createPackagesTable, connection);
                command.ExecuteNonQuery();

                // Create Reservations table
                string createReservationsTable = @"
        CREATE TABLE IF NOT EXISTS Reservations (
            ID INT AUTO_INCREMENT PRIMARY KEY,
            ClientID INT,
            TravelPackageID INT,
            Reservation_date DATETIME,
            NumberOfPeople INT NOT NULL DEFAULT 1,
            FOREIGN KEY (ClientID) REFERENCES Clients(ID) ON DELETE CASCADE,
            FOREIGN KEY (TravelPackageID) REFERENCES Packages(ID) ON DELETE CASCADE
        )";
                command = new MySqlCommand(createReservationsTable, connection);
                command.ExecuteNonQuery();

                // Create Services table
                string createServicesTable = @"
        CREATE TABLE IF NOT EXISTS Services (
            ID INT AUTO_INCREMENT PRIMARY KEY,
            ServiceName VARCHAR(255) NOT NULL,
            ServicePrice DECIMAL(10, 2) NOT NULL,
            ServiceDescription TEXT
        )";
                command = new MySqlCommand(createServicesTable, connection);
                command.ExecuteNonQuery();

                // Create ReservationServiceAssociations table
                string createReservationServiceAssociationsTable = @"
        CREATE TABLE IF NOT EXISTS ReservationServiceAssociations (
            ID INT AUTO_INCREMENT PRIMARY KEY,
            ReservationID INT NOT NULL,
            ServiceID INT NOT NULL,
            FOREIGN KEY (ReservationID) REFERENCES Reservations(ID) ON DELETE CASCADE,
            FOREIGN KEY (ServiceID) REFERENCES Services(ID) ON DELETE CASCADE
        )";
                command = new MySqlCommand(createReservationServiceAssociationsTable, connection);
                command.ExecuteNonQuery();
            }
        }

        public DataTable GetClientByName(string firstName, string lastName)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = "SELECT ID, Ime, Prezime, Broj_pasosa, Datum_rodjenja, Email_adresa, Broj_telefona FROM Clients WHERE Ime LIKE @firstName OR Prezime LIKE @lastName";
                var adapter = new MySqlDataAdapter(query, (MySqlConnection)connection);
                adapter.SelectCommand.Parameters.AddWithValue("@firstName", $"%{firstName}%");
                adapter.SelectCommand.Parameters.AddWithValue("@lastName", $"%{lastName}%");
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
        }

        public void AddClient(Client client)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand("INSERT INTO Clients (Ime, Prezime, Broj_pasosa, Datum_rodjenja, Email_adresa, Broj_telefona) VALUES (@firstName, @lastName, @passportNumber, @dateOfBirth, @email, @phoneNumber)", (MySqlConnection)connection);
                command.Parameters.AddWithValue("@firstName", client.FirstName);
                command.Parameters.AddWithValue("@lastName", client.LastName);
                command.Parameters.AddWithValue("@passportNumber", PassportEncryptor.Encrypt(client.PassportNumber)); // Updated to use Encrypt
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
                var command = new MySqlCommand("INSERT INTO Packages (Naziv, Cena, Tip, DestinacijaID, Detalji) VALUES (@name, @price, @type, @destinationId, @details)", (MySqlConnection)connection);
                command.Parameters.AddWithValue("@name", package.Name);
                command.Parameters.AddWithValue("@price", package.Price);
                command.Parameters.AddWithValue("@type", package.Type);
                command.Parameters.AddWithValue("@destinationId", package.DestinationId);
                command.Parameters.AddWithValue("@details", package.Details);
                command.ExecuteNonQuery();
            }
        }

        public int AddReservation(Reservation reservation)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand("INSERT INTO Reservations (ClientID, TravelPackageID, Reservation_date, NumberOfPeople) VALUES (@clientId, @travelPackageId, @reservationDate, @numberOfPeople)", (MySqlConnection)connection);
                command.Parameters.AddWithValue("@clientId", reservation.ClientId);
                command.Parameters.AddWithValue("@travelPackageId", reservation.TravelPackageId);
                command.Parameters.AddWithValue("@reservationDate", reservation.ReservationDate);
                command.Parameters.AddWithValue("@numberOfPeople", reservation.NumberOfPeople);
                command.ExecuteNonQuery();

                command = new MySqlCommand("SELECT LAST_INSERT_ID()", (MySqlConnection)connection);
                long lastId = (long)command.ExecuteScalar();
                return (int)lastId;
            }
        }

        public void AddService(Service service)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand("INSERT INTO Services (ServiceName, ServicePrice, ServiceDescription) VALUES (@serviceName, @servicePrice, @serviceDescription)", (MySqlConnection)connection);
                command.Parameters.AddWithValue("@serviceName", service.ServiceName);
                command.Parameters.AddWithValue("@servicePrice", service.ServicePrice);
                command.Parameters.AddWithValue("@serviceDescription", service.ServiceDescription);
                command.ExecuteNonQuery();
            }
        }

        public void AddReservationService(ReservationServiceAssociation reservationService)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand("INSERT INTO ReservationServiceAssociations (ReservationID, ServiceID) VALUES (@reservationId, @serviceId)", (MySqlConnection)connection);
                command.Parameters.AddWithValue("@reservationId", reservationService.ReservationId);
                command.Parameters.AddWithValue("@serviceId", reservationService.ServiceId);
                command.ExecuteNonQuery();
            }
        }

        public DataTable GetClients()
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var adapter = new MySqlDataAdapter("SELECT ID, Ime, Prezime, Broj_pasosa, Datum_rodjenja, Email_adresa, Broj_telefona FROM Clients", (MySqlConnection)connection);
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
                var adapter = new MySqlDataAdapter("SELECT p.*, d.Naziv_destinacije FROM Packages p LEFT JOIN Destinations d ON p.DestinacijaID = d.ID", (MySqlConnection)connection);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
        }

        public TravelPackage GetPackageById(int packageId)
        {
            TravelPackage package = null;
            using (var connection = (MySqlConnection)GetConnection()) // Or SQLiteConnection for SQLiteService
            {
                connection.Open();
                var command = new MySqlCommand("SELECT * FROM Packages WHERE ID = @packageId", connection); // Or SQLiteCommand
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

        public DataTable GetTravelPackageByType(string type)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = "SELECT TP.ID, TP.Naziv, TP.Cena, TP.Tip, TP.Details, D.Name as DestinationName FROM TravelPackages TP JOIN Destinations D ON TP.DestinationID = D.ID WHERE TP.Tip = @type";
                var adapter = new MySqlDataAdapter(query, (MySqlConnection)connection);
                adapter.SelectCommand.Parameters.AddWithValue("@type", type);
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
                var adapter = new MySqlDataAdapter(@"
                    SELECT r.ID, r.ClientID, r.TravelPackageID, r.Reservation_date, r.NumberOfPeople, 
                           p.Naziv AS PackageName, p.Cena AS PackagePrice, p.Tip AS PackageType, p.Detalji AS PackageDetails,
                           d.Naziv_destinacije AS DestinationName
                    FROM Reservations r
                    JOIN Packages p ON r.TravelPackageID = p.ID
                    LEFT JOIN Destinations d ON p.DestinacijaID = d.ID
                    WHERE r.ClientID = @clientId
                ", (MySqlConnection)connection);
                adapter.SelectCommand.Parameters.AddWithValue("@clientId", clientId);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
        }

        public Reservation GetReservationById(int reservationId)
        {
            Reservation reservation = null;
            using (var connection = (MySqlConnection)GetConnection()) // Or SQLiteConnection
            {
                connection.Open();
                var command = new MySqlCommand("SELECT * FROM Reservations WHERE ID = @reservationId", connection); // Or SQLiteCommand
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
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand("SELECT ID, Ime, Prezime, Broj_pasosa, Datum_rodjenja, Email_adresa, Broj_telefona FROM Clients WHERE Broj_pasosa = @passportHash", (MySqlConnection)connection);
                command.Parameters.AddWithValue("@passportHash", PassportEncryptor.Encrypt(passportNumber));

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Client
                        {
                            Id = reader.GetInt32(0),
                            FirstName = reader.GetString(1),
                            LastName = reader.GetString(2),
                            PassportNumber = passportNumber, // Use the provided, unencrypted passport number
                            DateOfBirth = reader.GetDateTime(4),
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
            using (var connection = GetConnection())
            {
                connection.Open();

                // Check if destination exists
                var checkCommand = new MySqlCommand("SELECT ID FROM Destinations WHERE Naziv_destinacije = @destinationName", (MySqlConnection)connection);
                checkCommand.Parameters.AddWithValue("@destinationName", destinationName);
                var result = checkCommand.ExecuteScalar();
                if (result != null)
                {
                    return Convert.ToInt32(result);
                }

                // If not, create it
                var insertCommand = new MySqlCommand("INSERT INTO Destinations (Naziv_destinacije) VALUES (@destinationName)", (MySqlConnection)connection);
                insertCommand.Parameters.AddWithValue("@destinationName", destinationName);
                insertCommand.ExecuteNonQuery();

                return (int)insertCommand.LastInsertedId;
            }
        }

        public List<Reservation> GetReservationsForPackage(int packageId)
        {
            var reservations = new List<Reservation>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var command = new MySqlCommand("SELECT * FROM Reservations WHERE PaketID = @packageId", connection);
                command.Parameters.AddWithValue("@packageId", packageId);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        reservations.Add(new Reservation
                        {
                            Id = reader.GetInt32("ID"),
                            ClientId = reader.GetInt32("KlijentID"),
                            TravelPackageId = reader.GetInt32("PaketID"),
                            ReservationDate = reader.GetDateTime("Datum_rezervacije"),
                            NumberOfPeople = reader.GetInt32("Broj_osoba")
                        });
                    }
                }
            }
            return reservations;
        }

        public void UpdateClient(Client client)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand("UPDATE Clients SET Ime = @firstName, Prezime = @lastName, Broj_pasosa = @passportNumber, Datum_rodjenja = @dateOfBirth, Email_adresa = @email, Broj_telefona = @phoneNumber WHERE ID = @id", (MySqlConnection)connection);
                command.Parameters.AddWithValue("@id", client.Id);
                command.Parameters.AddWithValue("@firstName", client.FirstName);
                command.Parameters.AddWithValue("@lastName", client.LastName);
                command.Parameters.AddWithValue("@passportNumber", PassportEncryptor.Encrypt(client.PassportNumber)); // Updated to use Encrypt
                command.Parameters.AddWithValue("@dateOfBirth", client.DateOfBirth);
                command.Parameters.AddWithValue("@email", client.Email);
                command.Parameters.AddWithValue("@phoneNumber", client.PhoneNumber);
                command.ExecuteNonQuery();
            }
        }

        public void UpdateReservation(Reservation reservation)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand("UPDATE Reservations SET ClientID = @clientId, TravelPackageID = @travelPackageId, Reservation_date = @reservationDate, NumberOfPeople = @numberOfPeople WHERE ID = @id", (MySqlConnection)connection);
                command.Parameters.AddWithValue("@clientId", reservation.ClientId);
                command.Parameters.AddWithValue("@travelPackageId", reservation.TravelPackageId);
                command.Parameters.AddWithValue("@reservationDate", reservation.ReservationDate);
                command.Parameters.AddWithValue("@numberOfPeople", reservation.NumberOfPeople);
                command.Parameters.AddWithValue("@id", reservation.Id);
                command.ExecuteNonQuery();
            }
        }

        public void UpdateService(Service service)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand("UPDATE Services SET ServiceName = @serviceName, ServicePrice = @servicePrice, ServiceDescription = @serviceDescription WHERE ID = @id", (MySqlConnection)connection);
                command.Parameters.AddWithValue("@id", service.Id);
                command.Parameters.AddWithValue("@serviceName", service.ServiceName);
                command.Parameters.AddWithValue("@servicePrice", service.ServicePrice);
                command.Parameters.AddWithValue("@serviceDescription", service.ServiceDescription);
                command.ExecuteNonQuery();
            }
        }

        // U klasi MySqlService
        public void UpdatePackage(TravelPackage package)
        {
            using (var connection = (MySqlConnection)GetConnection())
            {
                connection.Open();
                string query = "UPDATE Packages SET Naziv = @name, Cena = @price, Tip = @type, DestinacijaID = @destinationId, Detalji = @details WHERE ID = @id";
                var command = new MySqlCommand(query, connection);
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
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand("DELETE FROM Reservations WHERE ID = @id", (MySqlConnection)connection);
                command.Parameters.AddWithValue("@id", reservationId);
                command.ExecuteNonQuery();
            }
        }

        public void DeleteService(int serviceId)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand("DELETE FROM Services WHERE ID = @id", (MySqlConnection)connection);
                command.Parameters.AddWithValue("@id", serviceId);
                command.ExecuteNonQuery();
            }
        }

        public void DeleteReservationService(int reservationServiceId)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand("DELETE FROM ReservationServiceAssociations WHERE ID = @id", (MySqlConnection)connection);
                command.Parameters.AddWithValue("@id", reservationServiceId);
                command.ExecuteNonQuery();
            }
        }

        public void DeleteServicesForReservation(int reservationId)
        {
            using (var connection = (MySqlConnection)GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand("DELETE FROM ReservationServiceAssociations WHERE ReservationID = @reservationId", connection);
                command.Parameters.AddWithValue("@reservationId", reservationId);
                command.ExecuteNonQuery();
            }
        }

        public void DeletePackage(int packageId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var command = new MySqlCommand("DELETE FROM Packages WHERE ID = @packageId", connection);
                command.Parameters.AddWithValue("@packageId", packageId);
                command.ExecuteNonQuery();
            }
        }

        public DataTable GetAvailableDestinations()
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var adapter = new MySqlDataAdapter("SELECT DISTINCT Naziv_destinacije FROM Destinations", (MySqlConnection)connection);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
        }

        public DataTable GetServices()
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var adapter = new MySqlDataAdapter("SELECT * FROM Services", (MySqlConnection)connection);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
        }

        public List<Service> GetServicesForReservation(int reservationId)
        {
            var services = new List<Service>();
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand(@"
                    SELECT s.ID, s.ServiceName, s.ServicePrice, s.ServiceDescription 
                    FROM Services s
                    JOIN ReservationServiceAssociations rsa ON s.ID = rsa.ServiceID
                    WHERE rsa.ReservationID = @reservationId
                ", (MySqlConnection)connection);
                command.Parameters.AddWithValue("@reservationId", reservationId);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        services.Add(new Service
                        {
                            Id = reader.GetInt32("ID"),
                            ServiceName = reader.GetString("ServiceName"),
                            ServicePrice = reader.GetDecimal("ServicePrice"),
                            ServiceDescription = reader.GetString("ServiceDescription")
                        });
                    }
                }
            }
            return services;
        }
    }
}