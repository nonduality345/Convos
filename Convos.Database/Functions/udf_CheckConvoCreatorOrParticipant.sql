CREATE FUNCTION [dbo].[udf_CheckConvoCreatorOrParticipant]
(
	@Id BIGINT,
	@UserId BIGINT
)
RETURNS INT
AS
BEGIN
	IF NOT EXISTS(
		SELECT * 
		FROM vw_Convo CONVO
		WHERE CONVO.[Id] = @Id AND (CONVO.[Creator] = @UserId OR CONVO.[Participant] = @UserId)
	)
	BEGIN
		RETURN 4;
	END
	RETURN 0;
END
