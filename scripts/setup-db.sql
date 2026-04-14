-- ─────────────────────────────────────────────────────────────────────────────
-- setup-db.sql — Script di inizializzazione del database per il progetto YellowPigeon.
--
-- Eseguire questo script su SQL Server (locale via Docker o Azure) PRIMA di avviare l'API.
-- Lo script è idempotente: usa IF NOT EXISTS per evitare errori se eseguito più volte.
--
-- Connessione suggerita:
--   Server : localhost,1433
--   Login  : sa
--   Password: Your_password123!
-- ─────────────────────────────────────────────────────────────────────────────

-- ── 1. Crea il database se non esiste ────────────────────────────────────────
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'OrdersLab')
BEGIN
    CREATE DATABASE OrdersLab;
END;
GO

USE OrdersLab;
GO

-- ── 2. Tabella Customers ──────────────────────────────────────────────────────
-- Tabella di supporto per la FK su Orders.CustomerId.
-- In un progetto reale sarebbe gestita da una propria API; qui viene pre-popolata con dati di test.
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Customers')
BEGIN
    CREATE TABLE Customers (
        CustomerId INT         NOT NULL PRIMARY KEY,
        Name       NVARCHAR(100) NOT NULL
    );
END;
GO

-- ── 3. Tabella Orders ─────────────────────────────────────────────────────────
-- Testata dell'ordine. OrderId è IDENTITY (auto-increment).
-- CreatedAtUtc viene impostato lato applicazione (non usa DEFAULT per permettere
-- al service di controllare il timestamp e restituirlo nella risposta).
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

-- ── 4. Tabella OrderItems ─────────────────────────────────────────────────────
-- Righe d'ordine. Ogni riga è collegata a un ordine tramite FK su OrderId.
-- Quantity e UnitPrice hanno CHECK constraint per garantire valori positivi anche a DB level.
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

        -- Questi CHECK replicano le validazioni del service come ultimo livello di difesa
        CONSTRAINT CK_OrderItems_Quantity  CHECK (Quantity > 0),
        CONSTRAINT CK_OrderItems_UnitPrice CHECK (UnitPrice > 0)
    );
END;
GO

-- ── 5. Seed dati di test ──────────────────────────────────────────────────────
-- Inserisce due customer di esempio usati nel file requests.http.
-- customerId=1 → ordine valido (happy path)
-- customerId=99999 → non esiste → provoca FK violation → 409 Conflict
IF NOT EXISTS (SELECT * FROM Customers WHERE CustomerId = 1)
BEGIN
    INSERT INTO Customers (CustomerId, Name) VALUES (1, 'Demo Customer');
END;

IF NOT EXISTS (SELECT * FROM Customers WHERE CustomerId = 2)
BEGIN
    INSERT INTO Customers (CustomerId, Name) VALUES (2, 'Internal Test Customer');
END;
GO

-- ── Verifica finale ───────────────────────────────────────────────────────────
SELECT 'Setup completato.' AS Stato;
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE';
SELECT * FROM Customers;
GO
