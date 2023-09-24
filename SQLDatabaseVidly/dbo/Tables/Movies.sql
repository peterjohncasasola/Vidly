CREATE TABLE [dbo].[Movies] (
    [Id]                 INT            IDENTITY (1, 1) NOT NULL,
    [Name]               NVARCHAR (255) NOT NULL,
    [Genre]              NVARCHAR (MAX) NOT NULL,
    [DateRelease]        DATETIME       NOT NULL,
    [Stock]              INT            NOT NULL,
    [MinimumRequiredAge] INT            DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_dbo.Movies] PRIMARY KEY CLUSTERED ([Id] ASC)
);

