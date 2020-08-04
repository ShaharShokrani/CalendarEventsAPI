IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

CREATE TABLE [Events] (
    [Id] uniqueidentifier NOT NULL,
    [Start] datetime2 NOT NULL,
    [End] datetime2 NOT NULL,
    [IsAllDay] bit NOT NULL,
    [Title] nvarchar(max) NULL,
    [Description] nvarchar(max) NULL,
    [Details] nvarchar(max) NULL,
    [URL] nvarchar(max) NULL,
    [ImagePath] nvarchar(max) NULL,
    [Base64Id] nvarchar(max) NULL,
    [CreateDate] datetime2 NOT NULL,
    [UpdateDate] datetime2 NOT NULL,
    [OwnerId] nvarchar(max) NULL,
    CONSTRAINT [PK_Events] PRIMARY KEY ([Id])
);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20200728101743_InitialMigration', N'3.1.4');

GO