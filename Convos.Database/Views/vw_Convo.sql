CREATE VIEW [dbo].[vw_Convo] AS 
	SELECT 
		[Id], 
		[Creator], 
		[DateCreated],
		[DateOfLastMessage],
		[DateUpdated],
		[Participant], 
		[Subject]
	FROM 
		[Convo] CONVO
	WHERE 
		CONVO.[IsDeleted] = 0
