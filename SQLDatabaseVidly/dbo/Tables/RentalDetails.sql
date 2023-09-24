CREATE TABLE [dbo].[RentalDetails] (
    [Id]           INT      IDENTITY (1, 1) NOT NULL,
    [RentalId]     INT      NOT NULL,
    [MovieId]      INT      NOT NULL,
    [IsReturned]   BIT      NOT NULL,
    [DateReturned] DATETIME NULL,
    CONSTRAINT [PK_dbo.RentalDetails] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.RentalDetails_dbo.Movies_MovieId] FOREIGN KEY ([MovieId]) REFERENCES [dbo].[Movies] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.RentalDetails_dbo.Rentals_RentalId] FOREIGN KEY ([RentalId]) REFERENCES [dbo].[Rentals] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_MovieId]
    ON [dbo].[RentalDetails]([MovieId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_RentalId]
    ON [dbo].[RentalDetails]([RentalId] ASC);

