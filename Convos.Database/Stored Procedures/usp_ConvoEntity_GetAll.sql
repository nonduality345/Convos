CREATE PROCEDURE [dbo].[usp_ConvoEntity_GetAll]
	@Before DATETIME,
	@Count INT,
	@Index INT,
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

	DECLARE @Total INT;
	DECLARE @MaxCreated DATETIME;
	
	SELECT 
		@Total = COUNT(*),
		@MaxCreated = MAX([DateCreated])
	FROM 
		[vw_Convo] 
	WHERE 
		([Creator] = @UserId OR [Participant] = @UserId)
		AND ([DateOfLastMessage] < @Before OR [DateOfLastMessage] IS NULL);

	IF @Total = 0
	BEGIN
		RETURN 3
	END

	DECLARE @offset INT = @Index * @Count;
	
	WITH results_cte AS (
		SELECT 
			[Id], 
			[Creator], 
			[DateCreated],
			[DateOfLastMessage],
			[DateUpdated],
			[Participant], 
			[Subject],
			ROW_NUMBER() OVER (ORDER BY [DateOfLastMessage] DESC, [DateCreated] DESC) AS RowNum
		FROM vw_Convo
		WHERE
			([Creator] = @UserId
			OR 
			[Participant] = @UserId)
			AND ([DateCreated] < @Before OR [DateOfLastMessage] IS NULL)
	)

	SELECT 
		[Id], 
		[Creator], 
		[DateCreated],
		[DateOfLastMessage],
		[DateUpdated],
		[Participant], 
		[Subject],
		[NumMessages] = (SELECT COUNT(*) FROM [vw_Message] WHERE [ConvoId] = CTE.[Id])
	FROM results_cte CTE
	WHERE RowNum > @offset
	AND RowNum <= @offset + @Count;

	SELECT @Total AS Total;
	SELECT @MaxCreated AS MaxCreated;
RETURN 0
GO