CREATE TABLE [dbo].[Rentals] (
    [intRentalId]         INT      IDENTITY (1, 1) NOT NULL,
    [strTransactionCode] NVARCHAR(30) NULL,
    [intCustomerId] INT      NOT NULL,
    [dtmDateRented] DATETIME NOT NULL,
    [ysnCompleted] BIT NULL , 
    [dtmDateCompleted] DATETIME NULL, 
    CONSTRAINT [PK_dbo.Rentals] PRIMARY KEY CLUSTERED ([intRentalId] ASC),
    CONSTRAINT [FK_dbo.Rentals_dbo.Customers_CustomerId] FOREIGN KEY ([intCustomerId]) REFERENCES [dbo].[tblCustomers] ([intCustomerId]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_CustomerId]
    ON [dbo].[Rentals]([intCustomerId] ASC);

