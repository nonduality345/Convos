CREATE TABLE [dbo].[Convo]
(
	[Id] BIGINT IDENTITY(1, 1) NOT NULL, 
	[Creator] BIGINT NOT NULL, 
	[DateCreated] DATETIME NOT NULL,
	[DateDeleted] DATETIME NULL,
	[DateOfLastMessage] DATETIME NULL,
    [DateUpdated] DATETIME NOT NULL,
	[IsDeleted] BIT CONSTRAINT [DF_Thread_Deleted] DEFAULT (0) NOT NULL,
	[Participant] BIGINT NOT NULL, 
    [Subject] NVARCHAR(140) NULL, 
	CONSTRAINT [PK_Thread] PRIMARY KEY CLUSTERED ([Id] ASC), 
    CONSTRAINT [FK_Convo_User] FOREIGN KEY ([Creator]) REFERENCES [User]([user_id]), 
    CONSTRAINT [FK_Convo_Participant] FOREIGN KEY ([Participant]) REFERENCES [User]([user_id])
)


GO

CREATE INDEX [IX_Convo_Participant_Creator] ON [dbo].[Convo] ([Participant], [Creator])

GO

CREATE INDEX [IX_Convo_DateCreated] ON [dbo].[Convo] ([DateCreated] DESC)
