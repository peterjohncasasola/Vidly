CREATE TABLE [dbo].[Rentals] (
    [Id]         INT      IDENTITY (1, 1) NOT NULL,
    [CustomerId] INT      NOT NULL,
    [DateRented] DATETIME NOT NULL,
    [IsCompleted] BIT NULL, 
    [DateCompleted] DATETIME NULL, 
    [RentalCode] NCHAR(10) NOT NULL, 
    CONSTRAINT [PK_dbo.Rentals] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.Rentals_dbo.Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_CustomerId]
    ON [dbo].[Rentals]([CustomerId] ASC);

