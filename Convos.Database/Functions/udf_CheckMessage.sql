CREATE FUNCTION [dbo].[udf_CheckMessage]
(
	@Id BIGINT,
	@ConvoId BIGINT,
	@ResultCode INT = null
)
RETURNS INT
AS
BEGIN
	IF @ResultCode = NULL
	BEGIN
		SET @ResultCode = 2;
	END
	IF NOT EXISTS(SELECT * FROM [vw_Message] WHERE [Id] = @Id AND [ConvoId] = @ConvoId)
	BEGIN
		RETURN @ResultCode;
	END
	RETURN 0;
END
