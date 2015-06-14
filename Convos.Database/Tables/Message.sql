CREATE TABLE [dbo].[Message]
(
	[Id] BIGINT IDENTITY(1, 1) NOT NULL, 
    [Body] NVARCHAR(MAX) NULL, 
	[ConvoId] BIGINT NOT NULL,
    [DateCreated] DATETIME NOT NULL, 
	[DateDeleted] DATETIME NULL,
	[DateUpdated] DATETIME NOT NULL,
	[IsDeleted] BIT CONSTRAINT [DF_Convo_Deleted] DEFAULT (0) NOT NULL,
    [IsRead] BIT CONSTRAINT [DF_Convo_IsRead] DEFAULT(0) NOT NULL, 
    [Parent] BIGINT NULL,
    [Recipient] BIGINT NOT NULL, 
    [Sender] BIGINT NOT NULL, 
	CONSTRAINT [PK_Message] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_Message_Convo] FOREIGN KEY ([ConvoId]) REFERENCES [Convo]([Id]),
    CONSTRAINT [FK_Message_Parent] FOREIGN KEY ([Parent]) REFERENCES [Message]([Id]),
	CONSTRAINT [FK_Message_Recipient] FOREIGN KEY ([Recipient]) REFERENCES [User]([user_id]),
	CONSTRAINT [FK_Message_Sender] FOREIGN KEY ([Sender]) REFERENCES [User]([user_id])
)
GO

CREATE INDEX [IX_Message_ConvoId] ON [dbo].[Message] ([ConvoId])

GO

CREATE INDEX [IX_Message_DateCreated] ON [dbo].[Message] ([DateCreated] DESC)
