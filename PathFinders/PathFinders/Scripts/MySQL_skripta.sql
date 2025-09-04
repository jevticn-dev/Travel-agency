-- Kreiranje baze podataka TuristickaAgencija ako ne postoji
CREATE DATABASE IF NOT EXISTS turisticka_agencija;

-- Koriscenje baze TuristickaAgencija
USE turisticka_agencija;

-- Tabela Clients (Klijenti)
CREATE TABLE Clients (
    ID INT AUTO_INCREMENT PRIMARY KEY,
    Ime VARCHAR(255) NOT NULL,
    Prezime VARCHAR(255) NOT NULL,
    Broj_pasosa VARCHAR(255) UNIQUE NOT NULL,
    Datum_rodjenja DATE,
    Email_adresa VARCHAR(255) UNIQUE,
    Broj_telefona VARCHAR(50)
);

-- Tabela Packages (Paketi)
CREATE TABLE Packages (
    ID_paketa INT AUTO_INCREMENT PRIMARY KEY,
    Naziv_paketa VARCHAR(255) NOT NULL,
    Cena DECIMAL(10, 2) NOT NULL,
    Vrsta_paketa VARCHAR(50) NOT NULL,
    Detalji_paketa JSON
);

-- Tabela Reservations (Rezervacije) koja povezuje klijente i pakete
CREATE TABLE Reservations (
    ReservationID INT AUTO_INCREMENT PRIMARY KEY,
    ClientID INT,
    PackageID INT,
    Reservation_date DATE NOT NULL,
    FOREIGN KEY (ClientID) REFERENCES Clients(ID),
    FOREIGN KEY (PackageID) REFERENCES Packages(ID_paketa)
);
