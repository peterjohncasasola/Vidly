CREATE TABLE [dbo].[MembershipTypes] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (MAX) NOT NULL,
    [SignUpFee]        SMALLINT       NOT NULL,
    [DurationInMonths] TINYINT        NOT NULL,
    [DiscountRate]     TINYINT        NOT NULL,
    CONSTRAINT [PK_dbo.MembershipTypes] PRIMARY KEY CLUSTERED ([Id] ASC)
);

