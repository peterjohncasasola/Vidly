CREATE TABLE [dbo].[MovieGenres]
(
    [intMovieGenreId] INT NOT NULL IDENTITY, 
    [intMovieId] INT NOT NULL, 
    [intGenreId] INT NOT NULL, 
    CONSTRAINT [FK_MovieGenres_dbo.Movies_intMovieId] 
    FOREIGN KEY ([intMovieId]) REFERENCES [dbo].[Movies]([intMovieId])
    ON DELETE CASCADE,
    CONSTRAINT [FK_MovieGenres_dbo.Movies_intGenreId] 
    FOREIGN KEY ([intGenreId]) REFERENCES [dbo].[Genres]([intGenreId])
    ON DELETE CASCADE, 
    CONSTRAINT [PK_MovieGenres] PRIMARY KEY ([intMovieGenreId])
)

GO

CREATE INDEX [IX_MovieGenres_intMovieId] ON [dbo].[MovieGenres] ([intMovieId])

GO

CREATE INDEX [IX_MovieGenres_intGenreId] ON [dbo].[MovieGenres] ([intGenreId])
