CREATE FUNCTION [dbo].[udf_CheckConvoCreator]
(
	@Id BIGINT,
	@UserId BIGINT
)
RETURNS INT
AS
BEGIN
	IF NOT EXISTS(SELECT * FROM [vw_Convo] WHERE [Id] = @Id AND [Creator] = @UserId)
	BEGIN
		RETURN 4;
	END
	RETURN 0;
END
