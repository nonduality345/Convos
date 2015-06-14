CREATE PROCEDURE [dbo].[usp_ConvoEntity_GetById]
	@ConvoId BIGINT,
	@UserId BIGINT,
	@ErrorMsg VARCHAR(255) output
AS
	DECLARE @Ret INT;

	SET @Ret = [dbo].[udf_CheckConvo](@ConvoId);
	IF @Ret <> 0
	BEGIN
		SET @ErrorMsg = 'Convo was not found';
		RETURN @Ret;
	END

	SET @Ret = [dbo].[udf_CheckConvoCreatorOrParticipant](@ConvoId, @UserId);
	IF @Ret <> 0
	BEGIN
		SET @ErrorMsg = 'You are unauthorized to view this data';
		RETURN @Ret;
	END

	SELECT 
		[Id], 
		[Creator], 
		[DateCreated],
		[DateOfLastMessage],
		[DateUpdated],
		[Participant], 
		[Subject],
		[NumMessages] = (SELECT COUNT(*) FROM [vw_Message] WHERE [ConvoId] = @ConvoId)
	FROM vw_Convo 
	WHERE [Id] = @ConvoId

RETURN 0
