CREATE PROCEDURE [dbo].[usp_MessageEntity_Insert]
		@Body NVARCHAR(MAX),
		@ConvoId BIGINT,
		@Recipient BIGINT, 
		@Parent BIGINT,
		@Sender BIGINT, 
		@ErrorMsg VARCHAR(255) output
AS
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY
        BEGIN TRAN T1

			DECLARE @Ret INT;

			SET @Ret = [dbo].[udf_CheckUser](@Recipient, 1);
			IF @Ret <> 0
			BEGIN
				SET @ErrorMsg = 'Invalid recipient';
				ROLLBACK TRAN;
				RETURN @Ret;
			END

			SET @Ret = [dbo].[udf_CheckUser](@Sender, 1);
			IF @Ret <> 0
			BEGIN
				SET @ErrorMsg = 'Invalid sender';
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
			IF @Parent IS NOT NULL
			BEGIN
				SET @Ret = [dbo].[udf_CheckMessage](@Parent, @ConvoId, 1);
				IF @Ret <> 0
				BEGIN
					SET @ErrorMsg = 'Parent is invalid';
					ROLLBACK TRAN;
					RETURN @Ret;
				END
			END

			SET @Ret = [dbo].[udf_CheckConvoCreatorOrParticipant](@ConvoId, @Sender);
			IF @Ret <> 0
			BEGIN
				SET @ErrorMsg = 'You are unauthorized to post a message to this convo';
				ROLLBACK TRAN;
				RETURN @Ret;
			END

			SET @Ret = [dbo].[udf_CheckConvoCreatorOrParticipant](@ConvoId, @Recipient);
			IF @Ret <> 0
			BEGIN
				SET @ErrorMsg = 'The recipient is not participating in this convo';
				ROLLBACK TRAN;
				RETURN @Ret;
			END

			INSERT INTO [Message]
				([Body], 
				[ConvoId],
				[DateCreated],
				[DateUpdated],
				[Parent],
				[Recipient], 
				[Sender])
			VALUES
				(@Body,
				@ConvoId,
				GETUTCDATE(),
				GETUTCDATE(),
				@Parent,
				@Recipient,
				@Sender)

			UPDATE [Convo]
			SET [DateOfLastMessage] = GETUTCDATE()
			WHERE [Id] = @ConvoId;

			SELECT @@IDENTITY AS Id;
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

RETURN 5
