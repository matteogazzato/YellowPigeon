-- ─────────────────────────────────────────────────────────────────────────────
-- setup-db.sql — Database initialization script for the YellowPigeon project.
--
-- Run this script on SQL Server (local via Docker or Azure) BEFORE starting the API.
-- The script is idempotent: it uses IF NOT EXISTS to avoid errors when executed multiple times.
--
-- Suggested connection:
--   Server : localhost,1433
--   Login  : sa
--   Password: Your_password123!
-- ─────────────────────────────────────────────────────────────────────────────

-- ── 1. Create the database if it does not exist ──────────────────────────────
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'OrdersLab')
BEGIN
    CREATE DATABASE OrdersLab;
END;
GO

USE OrdersLab;
GO

-- ── 2. Customers table ───────────────────────────────────────────────────────
-- Support table for the FK on Orders.CustomerId.
-- In a real project this would be managed by its own API; here it is pre-populated with test data.
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Customers')
BEGIN
    CREATE TABLE Customers (
        CustomerId INT         NOT NULL PRIMARY KEY,
        Name       NVARCHAR(100) NOT NULL
    );
END;
GO

-- ── 3. Orders table ──────────────────────────────────────────────────────────
-- Order header. OrderId is IDENTITY (auto-increment).
-- CreatedAtUtc is set by the application (no DEFAULT is used so the
-- service can control the timestamp and return it in the response).
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Orders')
BEGIN
    CREATE TABLE Orders (
        OrderId      INT            IDENTITY(1,1) NOT NULL PRIMARY KEY,
        CustomerId   INT            NOT NULL,
        Currency     NVARCHAR(3)    NOT NULL,
        TotalAmount  DECIMAL(18,2)  NOT NULL,
        CreatedAtUtc DATETIME2      NOT NULL,

        CONSTRAINT FK_Orders_Customers
            FOREIGN KEY (CustomerId) REFERENCES Customers(CustomerId)
    );
END;
GO

-- ── 4. OrderItems table ──────────────────────────────────────────────────────
-- Order lines. Each row is linked to an order through the FK on OrderId.
-- Quantity and UnitPrice have CHECK constraints to enforce positive values at DB level too.
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'OrderItems')
BEGIN
    CREATE TABLE OrderItems (
        OrderItemId INT           IDENTITY(1,1) NOT NULL PRIMARY KEY,
        OrderId     INT           NOT NULL,
        ProductCode NVARCHAR(50)  NOT NULL,
        Quantity    INT           NOT NULL,
        UnitPrice   DECIMAL(18,2) NOT NULL,

        CONSTRAINT FK_OrderItems_Orders
            FOREIGN KEY (OrderId) REFERENCES Orders(OrderId),

        -- These CHECK constraints replicate service validations as a final safety layer
        CONSTRAINT CK_OrderItems_Quantity  CHECK (Quantity > 0),
        CONSTRAINT CK_OrderItems_UnitPrice CHECK (UnitPrice > 0)
    );
END;
GO

-- ── 5. Seed test data ────────────────────────────────────────────────────────
-- Inserts two sample customers used in requests.http.
-- customerId=1 -> valid order (happy path)
-- customerId=99999 -> does not exist -> triggers FK violation -> 409 Conflict
IF NOT EXISTS (SELECT * FROM Customers WHERE CustomerId = 1)
BEGIN
    INSERT INTO Customers (CustomerId, Name) VALUES (1, 'Demo Customer');
END;

IF NOT EXISTS (SELECT * FROM Customers WHERE CustomerId = 2)
BEGIN
    INSERT INTO Customers (CustomerId, Name) VALUES (2, 'Internal Test Customer');
END;
GO

-- ── Final verification ───────────────────────────────────────────────────────
SELECT 'Setup completed.' AS Status;
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE';
SELECT * FROM Customers;
GO
