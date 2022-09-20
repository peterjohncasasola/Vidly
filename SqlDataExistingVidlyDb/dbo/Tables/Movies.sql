CREATE TABLE [dbo].[Movies] (
    [intMovieId]            INT            IDENTITY (1, 1) NOT NULL,
    [strName]               NVARCHAR (255) NOT NULL,
    [strGenre]              NVARCHAR (MAX) NOT NULL,
    [dtmDateRelease]        DATETIME       NOT NULL,
    [intStock]              INT            NOT NULL,
    [intMinimumRequiredAge] INT            DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_dbo.Movies] PRIMARY KEY CLUSTERED ([intMovieId] ASC)
);



