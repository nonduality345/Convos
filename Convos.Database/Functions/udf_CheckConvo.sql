﻿CREATE FUNCTION [dbo].[udf_CheckConvo]
(
	@Id BIGINT
)
RETURNS INT
AS
BEGIN
	IF NOT EXISTS(SELECT * FROM [vw_Convo] WHERE [Id] = @Id)
	BEGIN
		RETURN 2;
	END
	RETURN 0;
END