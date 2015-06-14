CREATE PROCEDURE [dbo].[usp_MessageEntity_GetByIds]
	@ConvoId BIGINT,
	@MessageIds IdTable READONLY,
	@UserId BIGINT,
	@ErrorMsg VARCHAR(255) output
AS
	DECLARE @Ret INT;
	SET @Ret = [dbo].[udf_CheckUser](@UserId, DEFAULT)
	IF @Ret <> 0
	BEGIN
		SET @ErrorMsg = 'You are unauthorized to view this data';
		RETURN @Ret;
	END

	SET @Ret = [dbo].[udf_CheckConvoCreatorOrParticipant](@ConvoId, @UserId);
	IF @Ret <> 0
	BEGIN
		SET @ErrorMsg = 'You are unauthorized to view this data';
		RETURN @Ret;
	END;

	WITH message_cte (
		[Id], 
		[Body], 
		[ConvoId],
		[DateCreated], 
		[DateUpdated],
		[IsRead], 
		[Parent],
		[Recipient],
		[Sender], 
		[Level],
		[Root]
	) AS
	(
		SELECT 
			MSG.[Id], 
			MSG.[Body], 
			MSG.[ConvoId],
			MSG.[DateCreated], 
			MSG.[DateUpdated],
			MSG.[IsRead], 
			MSG.[Parent],
			MSG.[Recipient],
			MSG.[Sender], 
			0,
			MSG.[Id]
		FROM 
			vw_Message MSG INNER JOIN @MessageIds IDS on MSG.[Id] = IDS.[Id]

		UNION ALL

		SELECT 
			MSG.[Id], 
			MSG.[Body], 
			MSG.[ConvoId],
			MSG.[DateCreated], 
			MSG.[DateUpdated],
			MSG.[IsRead], 
			MSG.[Parent],
			MSG.[Recipient],
			MSG.[Sender], 
			[CTE].[Level] + 1,
			[CTE].[Root]
		FROM vw_Message MSG INNER JOIN message_cte CTE
		ON CTE.Parent = MSG.Id
	)

	SELECT 
		[Id], 
		[Body], 
		[ConvoId],
		[DateCreated], 
		[DateUpdated],
		[IsRead], 
		[Parent],
		[Recipient],
		[Sender], 
		[Level],
		[Root]
	FROM message_cte;

	IF @@ROWCOUNT = 0
	BEGIN
		RETURN 3
	END
RETURN 0
