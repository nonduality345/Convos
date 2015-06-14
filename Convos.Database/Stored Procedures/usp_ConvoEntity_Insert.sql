CREATE PROCEDURE [dbo].[usp_ConvoEntity_Insert]
		@Creator BIGINT, 
		@Participant BIGINT,
		@Subject NVARCHAR(140),
		@ErrorMsg VARCHAR(255) output
AS
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY
        BEGIN TRAN T1

		DECLARE @Ret INT;

		SET @Ret = [dbo].[udf_CheckUser](@Creator, 1);
		IF @Ret <> 0
		BEGIN
			SET @ErrorMsg = 'Invalid creator';
			ROLLBACK TRAN;
			RETURN @Ret;
		END

		SET @Ret = [dbo].[udf_CheckUser](@Participant, 1);
		IF @Ret <> 0
		BEGIN
			SET @ErrorMsg = 'Invalid participant';
			ROLLBACK TRAN;
			RETURN @Ret;
		END

		INSERT INTO [Convo]
			([Creator], 
			[DateCreated],
			[DateUpdated],
			[Participant], 
			[Subject])
		VALUES
			(@Creator,
			GETUTCDATE(),
			GETUTCDATE(),
			@Participant,
			@Subject)

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
