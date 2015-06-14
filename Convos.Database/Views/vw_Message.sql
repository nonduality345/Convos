CREATE VIEW [dbo].[vw_Message] AS 
	SELECT 
		[Id], 
		[Body], 
		[ConvoId],
		[DateCreated], 
		[DateUpdated],
		[IsRead], 
		[Parent],
		[Recipient], 
		[Sender]
	FROM 
		[Message] 
	WHERE
		[IsDeleted] = 0