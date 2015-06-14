CREATE PROCEDURE [dbo].[usp_MessageEntity_GetAll]
	@Before DATETIME,
	@Count INT,
	@Index INT,
	@ConvoId BIGINT,
	@UserId BIGINT,
	@ErrorMsg VARCHAR(255) OUTPUT
AS
	SET NOCOUNT ON;

	DECLARE @Ret INT;
	SET @Ret = [dbo].[udf_CheckUser](@UserId, DEFAULT)
	IF @Ret <> 0
	BEGIN
		SET @ErrorMsg = 'You are unauthorized to view this data';
		RETURN @Ret;
	END

	SET @Ret = [dbo].[udf_CheckConvo](@ConvoId);

	IF @Ret <> 0
	BEGIN
		SET @ErrorMsg = 'Could not find convo';
		ROLLBACK TRAN;
		RETURN @Ret;
	END

	SET @Ret = [dbo].[udf_CheckConvoCreatorOrParticipant](@ConvoId, @UserId);
	IF @Ret <> 0
	BEGIN
		SET @ErrorMsg = 'You are unauthorized to view this data';
		RETURN @Ret;
	END

	DECLARE @Total INT;
	DECLARE @MaxCreated DATETIME;

	SELECT 
		@Total = COUNT(*),
		@MaxCreated = MAX([DateCreated])
	FROM 
		[vw_Message] 
	WHERE 
		[ConvoId] = @ConvoId AND [DateCreated] < @Before;

	IF @Total = 0
	BEGIN
		RETURN 3
	END

	DECLARE @offset INT = @Index * @Count;

	WITH results_cte AS (
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
			ROW_NUMBER() OVER (ORDER BY [DateCreated] DESC) AS RowNum
		FROM vw_Message
		WHERE [ConvoId] = @ConvoId
			AND [DateCreated] < @Before
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
			[Level] = 0
	FROM results_cte
	WHERE RowNum > @offset
	AND RowNum <= @offset + @Count;

	SELECT @Total AS Total;
	SELECT @MaxCreated AS MaxCreated;
RETURN 0
GO
