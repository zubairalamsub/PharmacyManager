

CREATE DATABASE PharmacyDB;
GO

USE PharmacyDB;
GO



CREATE TABLE AppUsers (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL,
    PasswordHash NVARCHAR(256) NOT NULL,
    FullName NVARCHAR(100) NOT NULL DEFAULT ''
);

CREATE TABLE Medicines (
    MedicineId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL,
    Category NVARCHAR(100) NOT NULL DEFAULT '',
    Manufacturer NVARCHAR(100) NOT NULL DEFAULT '',
    BatchNumber NVARCHAR(50) NOT NULL DEFAULT '',
    ExpiryDate DATE NOT NULL,
    StockQuantity INT NOT NULL DEFAULT 0,
    UnitPrice DECIMAL(10,2) NOT NULL DEFAULT 0,
    Description NVARCHAR(500) NOT NULL DEFAULT ''
);

CREATE TABLE SalesMasters (
    SalesId INT IDENTITY(1,1) PRIMARY KEY,
    InvoiceNumber NVARCHAR(20) NOT NULL,
    InvoiceDate DATE NOT NULL,
    CustomerName NVARCHAR(100) NOT NULL,
    ContactNumber NVARCHAR(15) NOT NULL DEFAULT '',
    SubTotal DECIMAL(10,2) NOT NULL DEFAULT 0,
    Discount DECIMAL(10,2) NOT NULL DEFAULT 0,
    GrandTotal DECIMAL(10,2) NOT NULL DEFAULT 0
);

CREATE TABLE SalesDetails (
    DetailId INT IDENTITY(1,1) PRIMARY KEY,
    SalesId INT NOT NULL,
    MedicineId INT NOT NULL,
    BatchNumber NVARCHAR(50) NOT NULL DEFAULT '',
    ExpiryDate DATE NOT NULL,
    Quantity INT NOT NULL DEFAULT 0,
    UnitPrice DECIMAL(10,2) NOT NULL DEFAULT 0,
    LineTotal DECIMAL(10,2) NOT NULL DEFAULT 0,
    CONSTRAINT FK_SalesDetails_SalesMasters FOREIGN KEY (SalesId)
        REFERENCES SalesMasters(SalesId) ON DELETE CASCADE,
    CONSTRAINT FK_SalesDetails_Medicines FOREIGN KEY (MedicineId)
        REFERENCES Medicines(MedicineId) ON DELETE NO ACTION
);
GO

-- Seed Data
INSERT INTO AppUsers (Username, PasswordHash, FullName)
VALUES ('admin', '240BE518FABD2724DDB6F04EEB1DA5967448D7E831C08C8FA822809F74C720A9', 'System Administrator');

INSERT INTO Medicines 
(Name, Category, Manufacturer, BatchNumber, ExpiryDate, StockQuantity, UnitPrice, Description)
VALUES
('Paracetamol 500mg', 'Analgesic', 'Square Pharmaceuticals', 'BD-2025-001', '2027-06-30', 500, 2.00, 'Pain reliever and fever reducer'),

('Amoxicillin 250mg', 'Antibiotic', 'Beximco Pharmaceuticals', 'BD-2025-002', '2026-12-31', 300, 5.50, 'Broad-spectrum antibiotic'),

('Omeprazole 20mg', 'Antacid', 'Incepta Pharmaceuticals', 'BD-2025-003', '2027-03-15', 200, 3.00, 'Proton pump inhibitor for acid reflux'),

('Cetirizine 10mg', 'Antihistamine', 'Renata Limited', 'BD-2025-004', '2027-09-20', 400, 1.50, 'Allergy relief medication'),

('Metformin 500mg', 'Antidiabetic', 'ACI Pharmaceuticals', 'BD-2025-005', '2026-08-10', 350, 3.80, 'Oral diabetes medicine for type 2 diabetes'),

('Ibuprofen 400mg', 'Analgesic', 'Opsonin Pharma', 'BD-2025-006', '2027-01-25', 250, 3.20, 'Anti-inflammatory pain reliever'),

('Azithromycin 500mg', 'Antibiotic', 'Eskayef Pharmaceuticals (SK+F)', 'BD-2025-007', '2026-11-05', 180, 8.00, 'Macrolide antibiotic for bacterial infections'),

('Losartan 50mg', 'Antihypertensive', 'Aristopharma Ltd', 'BD-2025-008', '2027-04-18', 220, 5.75, 'Blood pressure medication');
GO


PRINT 'Database setup completed successfully.';
GO
