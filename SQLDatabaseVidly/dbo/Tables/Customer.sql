CREATE TABLE [dbo].[Customer] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [FirstName]       NVARCHAR (50)  NOT NULL,
    [MiddleName]      NVARCHAR (50)  NULL,
    [LastName]        NVARCHAR (50)  NOT NULL,
    [CompleteAddress] NVARCHAR (MAX) NULL,
    [BirthDate]       DATE           NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


