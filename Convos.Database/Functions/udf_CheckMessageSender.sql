CREATE FUNCTION [dbo].[udf_CheckMessageSender]
(
	@Id BIGINT,
	@SenderId BIGINT
)
RETURNS INT
AS
BEGIN
	IF NOT EXISTS(SELECT * FROM [vw_Message] WHERE [Id] = @Id AND [Sender] = @SenderId)
	BEGIN
		RETURN 4;
	END
	RETURN 0
END
