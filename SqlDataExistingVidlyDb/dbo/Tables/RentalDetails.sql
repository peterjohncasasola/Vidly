CREATE TABLE [dbo].[RentalDetails] (
    [intRentalDetailId] INT      IDENTITY (1, 1) NOT NULL,
    [intRentalId]       INT      NOT NULL,
    [intMovieId]        INT      NOT NULL,
    [ysnIsReturned]     BIT      DEFAULT ((0)) NULL,
    [dtmDateReturned]   DATETIME NULL,
    CONSTRAINT [PK_dbo.RentalDetails] PRIMARY KEY CLUSTERED ([intRentalDetailId] ASC),
    CONSTRAINT [FK_dbo.RentalDetails_dbo.Movies_MovieId] FOREIGN KEY ([intMovieId]) REFERENCES [dbo].[Movies] ([intMovieId]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.RentalDetails_dbo.Rentals_RentalId] FOREIGN KEY ([intRentalId]) REFERENCES [dbo].[Rentals] ([intRentalId]) ON DELETE CASCADE
);




GO
CREATE NONCLUSTERED INDEX [IX_RentalId]
    ON [dbo].[RentalDetails]([intRentalId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_MovieId]
    ON [dbo].[RentalDetails]([intMovieId] ASC);

