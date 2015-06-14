CREATE PROCEDURE [dbo].[usp_MessageEntity_Patch]
		@Id BIGINT,
		@Body NVARCHAR(MAX),
		@ConvoId BIGINT,
		@IsRead BIT,
		@UserId BIGINT, 
		@ErrorMsg VARCHAR(255) output
AS
	SET NOCOUNT ON;
	SET XACT_ABORT ON;
	BEGIN TRY
        BEGIN TRAN T1
			DECLARE @Ret INT;

			SET @Ret = [dbo].[udf_CheckUser](@UserId, DEFAULT)
			IF @Ret <> 0
			BEGIN
				SET @ErrorMsg = 'You are unauthorized to perform this action';
				ROLLBACK TRAN;
				RETURN @Ret;
			END

			SET @Ret = [dbo].[udf_CheckConvo](@ConvoId);
			IF @Ret <> 0
			BEGIN
				SET @ErrorMsg = 'Convo was not found';
				ROLLBACK TRAN;
				RETURN @Ret;
			END

			SET @Ret = [dbo].[udf_CheckMessage](@Id, @ConvoId, DEFAULT);
			IF @Ret <> 0
			BEGIN
				SET @ErrorMsg = 'Could not find message';
				ROLLBACK TRAN;
				RETURN @Ret;
			END

			SET @Ret = [dbo].[udf_CheckMessageSender](@Id, @UserId);
			IF @Ret <> 0
			BEGIN
				SET @ErrorMsg = 'You are unauthorized to delete this message';
				ROLLBACK TRAN;
				RETURN @Ret;
			END

			UPDATE [Message]
			SET
				[Body] = COALESCE(@Body, [Body]),
				[IsRead] = COALESCE(@IsRead, [IsRead]),
				[DateUpdated] = GETUTCDATE()
			WHERE
				[Id] = @Id;

		COMMIT TRAN T1
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
        BEGIN
            ROLLBACK TRAN T1
        END

        DECLARE @ErrorMessage NVARCHAR(4000);
        DECLARE @ErrorSeverity INT;
        DECLARE @ErrorState INT;
        SELECT 
            @ErrorMessage = ERROR_MESSAGE(),
            @ErrorSeverity = ERROR_SEVERITY(),
            @ErrorState = ERROR_STATE();

        RAISERROR ( @ErrorMessage, -- Message text.
                    @ErrorSeverity, -- Severity.
                    @ErrorState -- State.
                  );
    END CATCH

RETURN 0
