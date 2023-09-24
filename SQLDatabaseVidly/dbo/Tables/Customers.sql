CREATE TABLE [dbo].[Customers] (
    [DateOfBirth]              DATETIME       NOT NULL,
    [Address]                  NVARCHAR (MAX) NULL,
    [Id]                       INT            IDENTITY (1, 1) NOT NULL,
    [Name]                     NVARCHAR (255) NOT NULL,
    [IsSubscribedToNewsLetter] BIT            NOT NULL,
    [MembershipTypeId]         INT            NOT NULL,
    CONSTRAINT [PK_dbo.Customers] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.Customers_dbo.MembershipTypes_MembershipTypeId] FOREIGN KEY ([MembershipTypeId]) REFERENCES [dbo].[MembershipTypes] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_MembershipTypeId]
    ON [dbo].[Customers]([MembershipTypeId] ASC);

