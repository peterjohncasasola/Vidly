CREATE TABLE [dbo].[tblCustomers] (
    [dtmDateOfBirth]              DATETIME       NOT NULL,
    [strCompleteAddress]          NVARCHAR (MAX) NULL,
    [intCustomerId]                       INT            IDENTITY (1, 1) NOT NULL,
    [strName]                     NVARCHAR (255) NOT NULL,
    [ysnSubscribedToNewsLetter] BIT            NOT NULL,
    [intMembershipTypeId]         INT            NOT NULL,
    CONSTRAINT [PK_dbo.Customers] PRIMARY KEY CLUSTERED ([intCustomerId] ASC),
    CONSTRAINT [FK_dbo.Customers_dbo.MembershipTypes_MembershipTypeId]
    FOREIGN KEY ([intMembershipTypeId]) REFERENCES [dbo].[MembershipTypes] 
    ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_MembershipTypeId]
    ON [dbo].[tblCustomers]([intMembershipTypeId] ASC);

