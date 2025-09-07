using MySql.Data.MySqlClient;
using PathFinders.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.AccessControl;
using System.Runtime.InteropServices;

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

        public List<Client> GetClientByName(string firstName, string lastName)
        {
            var clients = new List<Client>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var query = new StringBuilder("SELECT ID, Ime, Prezime, Broj_pasosa, Datum_rodjenja, Email_adresa, Broj_telefona FROM Clients WHERE ");
                var cmd = new MySqlCommand();
                cmd.Connection = (MySqlConnection)connection;

                if (!string.IsNullOrWhiteSpace(firstName) && !string.IsNullOrWhiteSpace(lastName))
                {
                    query.Append("Ime LIKE @firstName AND Prezime LIKE @lastName");
                    cmd.Parameters.AddWithValue("@firstName", $"%{firstName}%");
                    cmd.Parameters.AddWithValue("@lastName", $"%{lastName}%");
                }
                else if (!string.IsNullOrWhiteSpace(firstName))
                {
                    query.Append("Ime LIKE @first OR Prezime LIKE @first");
                    cmd.Parameters.AddWithValue("@first", $"%{firstName}%");
                }
                else if (!string.IsNullOrWhiteSpace(lastName))
                {
                    query.Append("Ime LIKE @last OR Prezime LIKE @last");
                    cmd.Parameters.AddWithValue("@last", $"%{lastName}%");
                }
                else
                {
                    query.Clear();
                    query.Append("SELECT ID, Ime, Prezime, Broj_pasosa, Datum_rodjenja, Email_adresa, Broj_telefona FROM Clients");
                }

                cmd.CommandText = query.ToString();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string encryptedPassport = reader.GetString("Broj_pasosa");
                        string decryptedPassport = PassportEncryptor.Decrypt(encryptedPassport);

                        clients.Add(new Client
                        {
                            Id = reader.GetInt32("ID"),
                            FirstName = reader.GetString("Ime"),
                            LastName = reader.GetString("Prezime"),
                            PassportNumber = decryptedPassport,
                            DateOfBirth = reader.GetDateTime("Datum_rodjenja"),
                            Email = reader.GetString("Email_adresa"),
                            PhoneNumber = reader.GetString("Broj_telefona")
                        });
                    }
                }
            }
            return clients;
        }

        public void AddClient(Client client)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand("INSERT INTO Clients (Ime, Prezime, Broj_pasosa, Datum_rodjenja, Email_adresa, Broj_telefona) VALUES (@firstName, @lastName, @passportNumber, @dateOfBirth, @email, @phoneNumber)", (MySqlConnection)connection);
                command.Parameters.AddWithValue("@firstName", client.FirstName);
                command.Parameters.AddWithValue("@lastName", client.LastName);
                command.Parameters.AddWithValue("@passportNumber", PassportEncryptor.Encrypt(client.PassportNumber));
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

        public List<Client> GetClients()
        {
            var clients = new List<Client>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT ID, Ime, Prezime, Broj_pasosa, Datum_rodjenja, Email_adresa, Broj_telefona FROM Clients";
                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string encryptedPassport = reader.GetString("Broj_pasosa");
                            string decryptedPassport = PassportEncryptor.Decrypt(encryptedPassport);

                            clients.Add(new Client
                            {
                                Id = reader.GetInt32("ID"),
                                FirstName = reader.GetString("Ime"),
                                LastName = reader.GetString("Prezime"),
                                PassportNumber = decryptedPassport,
                                DateOfBirth = reader.GetDateTime("Datum_rodjenja"),
                                Email = reader.GetString("Email_adresa"),
                                PhoneNumber = reader.GetString("Broj_telefona")
                            });
                        }
                    }
                }
            }
            return clients;
        }

        public List<TravelPackage> GetPackages()
        {
            var packages = new List<TravelPackage>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT p.*, d.Naziv_destinacije FROM Packages p LEFT JOIN Destinations d ON p.DestinacijaID = d.ID";
                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            packages.Add(new TravelPackage
                            {
                                Id = reader.GetInt32("ID"),
                                Name = reader.GetString("Naziv"),
                                Price = reader.GetDecimal("Cena"),
                                Type = reader.GetString("Tip"),
                                DestinationId = reader.GetInt32("DestinacijaID"),
                                DestinationName = reader.GetString("Naziv_destinacije"),
                                Details = reader.GetString("Detalji")
                            });
                        }
                    }
                }
            }
            return packages;
        }

        public TravelPackage GetPackageById(int packageId)
        {
            TravelPackage package = null;
            using (var connection = (MySqlConnection)GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand("SELECT * FROM Packages WHERE ID = @packageId", connection);
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

        public List<TravelPackage> GetTravelPackageByType(string type)
        {
            var packages = new List<TravelPackage>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT TP.ID, TP.Naziv, TP.Cena, TP.Tip, TP.Detalji, D.Naziv_destinacije as DestinationName FROM Packages TP JOIN Destinations D ON TP.DestinacijaID = D.ID WHERE TP.Tip = @type";
                var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@type", type);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        packages.Add(new TravelPackage
                        {
                            Id = reader.GetInt32("ID"),
                            Name = reader.GetString("Naziv"),
                            Price = reader.GetDecimal("Cena"),
                            Type = reader.GetString("Tip"),
                            Details = reader.GetString("Detalji"),
                            DestinationName = reader.GetString("DestinationName")
                        });
                    }
                }
            }
            return packages;
        }

        public List<Reservation> GetReservationsForClient(int clientId)
        {
            var reservations = new List<Reservation>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var command = new MySqlCommand(@"
                    SELECT r.ID, r.ClientID, r.TravelPackageID, r.Reservation_date, r.NumberOfPeople,
                           p.Naziv AS PackageName, p.Cena AS PackagePrice, p.Tip AS PackageType, p.Detalji AS PackageDetails,
                           d.Naziv_destinacije AS DestinationName
                    FROM Reservations r
                    JOIN Packages p ON r.TravelPackageID = p.ID
                    LEFT JOIN Destinations d ON p.DestinacijaID = d.ID
                    WHERE r.ClientID = @clientId
                ", connection);
                command.Parameters.AddWithValue("@clientId", clientId);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        reservations.Add(new Reservation
                        {
                            Id = reader.GetInt32("ID"),
                            ClientId = reader.GetInt32("ClientID"),
                            TravelPackageId = reader.GetInt32("TravelPackageID"),
                            ReservationDate = reader.GetDateTime("Reservation_date"),
                            NumberOfPeople = reader.GetInt32("NumberOfPeople")
                        });
                    }
                }
            }
            return reservations;
        }

        public Reservation GetReservationById(int reservationId)
        {
            Reservation reservation = null;
            using (var connection = (MySqlConnection)GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand("SELECT * FROM Reservations WHERE ID = @reservationId", connection);
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
                            PassportNumber = passportNumber,
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
                var checkCommand = new MySqlCommand("SELECT ID FROM Destinations WHERE Naziv_destinacije = @destinationName", (MySqlConnection)connection);
                checkCommand.Parameters.AddWithValue("@destinationName", destinationName);
                var result = checkCommand.ExecuteScalar();
                if (result != null)
                {
                    return Convert.ToInt32(result);
                }
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
                var command = new MySqlCommand("SELECT * FROM Reservations WHERE TravelPackageID = @packageId", connection);
                command.Parameters.AddWithValue("@packageId", packageId);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        reservations.Add(new Reservation
                        {
                            Id = reader.GetInt32("ID"),
                            ClientId = reader.GetInt32("ClientID"),
                            TravelPackageId = reader.GetInt32("TravelPackageID"),
                            ReservationDate = reader.GetDateTime("Reservation_date"),
                            NumberOfPeople = reader.GetInt32("NumberOfPeople")
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
                command.Parameters.AddWithValue("@passportNumber", PassportEncryptor.Encrypt(client.PassportNumber));
                command.Parameters.AddWithValue("@dateOfBirth", client.DateOfBirth);
                command.Parameters.AddWithValue("@email", client.Email);
                command.Parameters.AddWithValue("@phoneNumber", client.PhoneNumber);
                command.ExecuteNonQuery();
            }
        }

        public int UpdateReservation(Reservation reservation)
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

                return reservation.Id;
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

        public List<string> GetAvailableDestinations()
        {
            var destinations = new List<string>();
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand("SELECT DISTINCT Naziv_destinacije FROM Destinations", (MySqlConnection)connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        destinations.Add(reader.GetString(0));
                    }
                }
            }
            return destinations;
        }

        public List<Service> GetServices()
        {
            var services = new List<Service>();
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand("SELECT * FROM Services", (MySqlConnection)connection);
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