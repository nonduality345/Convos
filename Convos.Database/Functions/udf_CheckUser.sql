CREATE FUNCTION [dbo].[udf_CheckUser]
(
	@UserId BIGINT,
	@ResultCode INT = null
)
RETURNS INT
AS
BEGIN
	IF @ResultCode IS NULL
	BEGIN
		SET @ResultCode = 4;
	END
	IF NOT EXISTS(SELECT * FROM [User] WHERE [user_id] = @UserId)
	BEGIN
		RETURN @ResultCode;
	END
	RETURN 0;
END
