-- USE PatientVisitMS6606;



CREATE OR ALTER PROCEDURE stp_AddUser
    @Email NVARCHAR(100),
    @Password NVARCHAR(255),
    @UserRole NVARCHAR(20),
    @NewUserId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
   
    BEGIN TRY
        BEGIN TRANSACTION;
       
        IF EXISTS (SELECT 1 FROM Users WHERE Email = @Email)
        BEGIN
            ROLLBACK TRANSACTION;
            RAISERROR('Email already exists', 16, 1);
            RETURN;
        END
       
        INSERT INTO Users (Email, Password, UserRole, CreatedDate)
        VALUES (@Email, @Password, @UserRole, GETUTCDATE());
       
        SET @NewUserId = SCOPE_IDENTITY();
       
        COMMIT TRANSACTION;
       
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
             
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
       
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END



-- =================================== 
-- =================  Doctors ======== 
-- ===================================

    CREATE OR ALTER PROCEDURE stp_AddDoctor
        @DoctorName NVARCHAR(100), 
        @Specialization NVARCHAR(100), 
        @ContactNumber NVARCHAR(15), 
        @Email NVARCHAR(100)
    AS
    BEGIN
        SET NOCOUNT ON;
        BEGIN TRY 
            IF @DoctorName IS NULL OR LTRIM(RTRIM(@DoctorName)) = '' 
                THROW 50006, 'DoctorName is required', 1;
            
            INSERT INTO Doctors (DoctorName, Specialization, ContactNumber, Email) 
            VALUES (@DoctorName, @Specialization, @ContactNumber, @Email);
            
            SELECT SCOPE_IDENTITY() as NewDoctorId;
            
        END TRY 
        BEGIN CATCH  
            DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
            DECLARE @ErrorNumber INT = ERROR_NUMBER();
            THROW @ErrorNumber, @ErrorMessage, 1;
        END CATCH
    END;

    GO 
    CREATE OR ALTER PROCEDURE stp_DeleteDoctor 
        @DoctorId INT
    AS
    BEGIN
        SET NOCOUNT ON; 
        
        DECLARE @RowsAffected INT = 0;
        
        BEGIN TRY  
            IF NOT EXISTS (SELECT 1 FROM Doctors WHERE DoctorId = @DoctorId)
            BEGIN
                THROW 50007, 'Doctor not found', 1;
            END 
            DELETE FROM Doctors WHERE DoctorId = @DoctorId;
            SET @RowsAffected = @@ROWCOUNT;
            
            SELECT @RowsAffected as RowsAffected;
            
        END TRY 
        BEGIN CATCH 
            DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
            DECLARE @ErrorNumber INT = ERROR_NUMBER();
            THROW @ErrorNumber, @ErrorMessage, 1;
        END CATCH
    END;
    GO
    
    CREATE OR ALTER PROCEDURE stp_UpdateDoctor
        @DoctorId INT, 
        @DoctorName NVARCHAR(100), 
        @Specialization NVARCHAR(100), 
        @ContactNumber NVARCHAR(15), 
        @Email NVARCHAR(100)
    AS
    BEGIN
        SET NOCOUNT ON;
        
        DECLARE @RowsAffected INT = 0;
        
        BEGIN TRY 
            IF NOT EXISTS (SELECT 1 FROM Doctors WHERE DoctorId = @DoctorId) 
            BEGIN
                THROW 50007, 'Doctor not found', 1;
            END
            
            IF @DoctorName IS NULL OR LTRIM(RTRIM(@DoctorName)) = '' 
            BEGIN
                THROW 50006, 'DoctorName is required', 1;
            END
            
            UPDATE Doctors 
            SET DoctorName = @DoctorName, 
                Specialization = @Specialization, 
                ContactNumber = @ContactNumber, 
                Email = @Email 
            WHERE DoctorId = @DoctorId;
            
            SET @RowsAffected = @@ROWCOUNT;
            
            
            SELECT @RowsAffected as RowsAffected;
            
        END TRY 
        BEGIN CATCH 
            DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
            DECLARE @ErrorNumber INT = ERROR_NUMBER();
            THROW @ErrorNumber, @ErrorMessage, 1;
        END CATCH
    END;
    GO
    
    CREATE OR ALTER PROCEDURE stp_GetDoctorById 
        @DoctorId INT
    AS
    BEGIN
        SET NOCOUNT ON; 
        SELECT * FROM Doctors WHERE DoctorId = @DoctorId;
    END;
    GO
    
    CREATE OR ALTER PROCEDURE stp_GetAllDoctors
    AS
    BEGIN
        SET NOCOUNT ON; 
        SELECT * FROM Doctors ORDER BY CreatedDate DESC;
    END;
    GO



-- =================================== 
-- =================  Patients ======== 
-- ===================================
 
CREATE OR ALTER PROCEDURE stp_AddPatient
    @PatientName NVARCHAR(100), 
    @DateOfBirth DATE, 
    @Gender NVARCHAR(10), 
    @ContactNumber NVARCHAR(15),
    @Email NVARCHAR(100), 
    @Address NVARCHAR(255), 
    @EmergencyContact NVARCHAR(15)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY 
        IF @PatientName IS NULL OR LTRIM(RTRIM(@PatientName)) = '' 
            THROW 50008, 'PatientName is required', 1;
         
        IF @Gender IS NOT NULL AND @Gender NOT IN ('Male', 'Female', 'Other') 
            THROW 50009, 'Invalid Gender. Must be Male, Female, or Other', 1;
         
        INSERT INTO Patients (PatientName, DateOfBirth, Gender, ContactNumber, Email, Address, EmergencyContact)
        VALUES (@PatientName, @DateOfBirth, @Gender, @ContactNumber, @Email, @Address, @EmergencyContact);
         
        SELECT SCOPE_IDENTITY() as NewPatientId;
        
    END TRY 
    BEGIN CATCH 
         
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorNumber INT = ERROR_NUMBER();
        THROW @ErrorNumber, @ErrorMessage, 1;
    END CATCH
END;
GO
 
CREATE OR ALTER PROCEDURE stp_UpdatePatient
    @PatientId INT, 
    @PatientName NVARCHAR(100), 
    @DateOfBirth DATE, 
    @Gender NVARCHAR(10), 
    @ContactNumber NVARCHAR(15),
    @Email NVARCHAR(100), 
    @Address NVARCHAR(255), 
    @EmergencyContact NVARCHAR(15)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY 
        IF NOT EXISTS (SELECT 1 FROM Patients WHERE PatientId = @PatientId) 
            THROW 50010, 'Patient not found', 1;
         
        IF @PatientName IS NULL OR LTRIM(RTRIM(@PatientName)) = '' 
            THROW 50008, 'PatientName is required', 1;
         
        IF @Gender IS NOT NULL AND @Gender NOT IN ('Male', 'Female', 'Other') 
            THROW 50009, 'Invalid Gender. Must be Male, Female, or Other', 1;
         
        UPDATE Patients 
        SET PatientName = @PatientName, 
            DateOfBirth = @DateOfBirth, 
            Gender = @Gender,
            ContactNumber = @ContactNumber, 
            Email = @Email, 
            Address = @Address, 
            EmergencyContact = @EmergencyContact
        WHERE PatientId = @PatientId;
        
    END TRY 
    BEGIN CATCH 
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorNumber INT = ERROR_NUMBER();
        THROW @ErrorNumber, @ErrorMessage, 1;
    END CATCH
END;
GO
 
CREATE OR ALTER PROCEDURE stp_DeletePatient 
    @PatientId INT
AS
BEGIN
    SET NOCOUNT ON; 
    BEGIN TRY  
        IF NOT EXISTS (SELECT 1 FROM Patients WHERE PatientId = @PatientId)
            THROW 50010, 'Patient not found', 1;
            
        DELETE FROM Patients WHERE PatientId = @PatientId; 
    END TRY 
    BEGIN CATCH 
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorNumber INT = ERROR_NUMBER();
        THROW @ErrorNumber, @ErrorMessage, 1;
    END CATCH
END;
GO
 
CREATE OR ALTER PROCEDURE stp_GetPatientById 
    @PatientId INT
AS
BEGIN
    SET NOCOUNT ON; 
    SELECT * FROM Patients WHERE PatientId = @PatientId;
END;
GO
 
CREATE OR ALTER PROCEDURE stp_GetAllPatients
AS
BEGIN
    SET NOCOUNT ON; 
    SELECT * FROM Patients ORDER BY CreatedDate DESC;
END;



-- ============================
-- ======= Visit Type =========
-- ============================

CREATE OR ALTER PROCEDURE stp_AddVisitType
    @VisitTypeName NVARCHAR(50), 
    @BaseRate DECIMAL(10,2), 
    @Description NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY 
        IF @VisitTypeName IS NULL OR LTRIM(RTRIM(@VisitTypeName)) = '' 
            THROW 50011, 'VisitTypeName is required', 1;
            
        IF @BaseRate IS NULL OR @BaseRate < 0
            THROW 50013, 'BaseRate must be a positive value', 1;
            
        IF EXISTS (SELECT 1 FROM VisitTypes WHERE VisitTypeName = @VisitTypeName)
            THROW 50014, 'VisitType with this name already exists', 1;
         
        INSERT INTO VisitTypes (VisitTypeName, BaseRate, Description)
        VALUES (@VisitTypeName, @BaseRate, @Description);
        
        SELECT CAST(SCOPE_IDENTITY() AS INT) AS NewVisitTypeId;
        
    END TRY 
    BEGIN CATCH 
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorNumber INT = ERROR_NUMBER();
        THROW @ErrorNumber, @ErrorMessage, 1;
    END CATCH
END;
GO


CREATE OR ALTER PROCEDURE stp_UpdateVisitType
    @VisitTypeId INT, 
    @VisitTypeName NVARCHAR(50), 
    @BaseRate DECIMAL(10,2), 
    @Description NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @RowsAffected INT = 0;
    
    BEGIN TRY 
        IF NOT EXISTS (SELECT 1 FROM VisitTypes WHERE VisitTypeId = @VisitTypeId) 
        BEGIN
            THROW 50012, 'VisitType not found', 1;
        END
        
        IF @VisitTypeName IS NULL OR LTRIM(RTRIM(@VisitTypeName)) = '' 
        BEGIN
            THROW 50011, 'VisitTypeName is required', 1;
        END
        
        IF @BaseRate IS NULL OR @BaseRate < 0
        BEGIN
            THROW 50013, 'BaseRate must be a positive value', 1;
        END
        
        IF EXISTS (SELECT 1 FROM VisitTypes WHERE VisitTypeName = @VisitTypeName AND VisitTypeId != @VisitTypeId)
        BEGIN
            THROW 50014, 'VisitType with this name already exists', 1;
        END
        
        UPDATE VisitTypes 
        SET VisitTypeName = @VisitTypeName, 
            BaseRate = @BaseRate, 
            Description = @Description 
        WHERE VisitTypeId = @VisitTypeId;
        
        SET @RowsAffected = @@ROWCOUNT;
        
        SELECT @RowsAffected as RowsAffected;
        
    END TRY 
    BEGIN CATCH 
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorNumber INT = ERROR_NUMBER();
        THROW @ErrorNumber, @ErrorMessage, 1;
    END CATCH
END;
GO

CREATE OR ALTER PROCEDURE stp_DeleteVisitType 
    @VisitTypeId INT
AS
BEGIN
    SET NOCOUNT ON; 
    
    DECLARE @RowsAffected INT = 0;
    
    BEGIN TRY  
        IF NOT EXISTS (SELECT 1 FROM VisitTypes WHERE VisitTypeId = @VisitTypeId)
        BEGIN
            THROW 50012, 'VisitType not found', 1;
        END 
        
        DELETE FROM VisitTypes WHERE VisitTypeId = @VisitTypeId;
        SET @RowsAffected = @@ROWCOUNT;
        
        SELECT @RowsAffected as RowsAffected;
        
    END TRY 
    BEGIN CATCH 
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorNumber INT = ERROR_NUMBER();
        THROW @ErrorNumber, @ErrorMessage, 1;
    END CATCH
END;
GO

CREATE OR ALTER PROCEDURE stp_GetVisitTypeById 
    @VisitTypeId INT
AS
BEGIN
    SET NOCOUNT ON; 
    SELECT * FROM VisitTypes WHERE VisitTypeId = @VisitTypeId;
END;
GO

CREATE OR ALTER PROCEDURE stp_GetAllVisitTypes
AS
BEGIN
    SET NOCOUNT ON; 
    SELECT * FROM VisitTypes ORDER BY CreatedDate DESC;
END;

-- =================================== 
-- =================  Patient Visit ======== 
-- ===================================

CREATE OR ALTER PROCEDURE stp_AddPatientVisit
    @PatientId INT, @DoctorId INT, @VisitTypeId INT, @VisitDate DATETIME2, @Note NVARCHAR(500),
    @DurationInMinutes INT, @Fee DECIMAL(10,2), @CreatedBy INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO PatientVisits (PatientId, DoctorId, VisitTypeId, VisitDate, Note, DurationInMinutes, Fee, CreatedBy)
        VALUES (@PatientId, @DoctorId, @VisitTypeId, @VisitDate, @Note, @DurationInMinutes, @Fee, @CreatedBy);
    END TRY BEGIN CATCH THROW; END CATCH
END;
GO

CREATE OR ALTER PROCEDURE stp_UpdatePatientVisit
    @Id INT, @PatientId INT, @DoctorId INT, @VisitTypeId INT, @VisitDate DATETIME2, @Note NVARCHAR(500),
    @DurationInMinutes INT, @Fee DECIMAL(10,2), @ModifiedBy INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        UPDATE PatientVisits SET PatientId=@PatientId, DoctorId=@DoctorId, VisitTypeId=@VisitTypeId,
        VisitDate=@VisitDate, Note=@Note, DurationInMinutes=@DurationInMinutes, Fee=@Fee,
        ModifiedBy=@ModifiedBy, ModifiedDate=GETDATE()
        WHERE Id=@Id;
    END TRY BEGIN CATCH THROW; END CATCH
END;
GO

CREATE OR ALTER PROCEDURE stp_DeletePatientVisit @Id INT
AS
BEGIN
    SET NOCOUNT ON; BEGIN TRY DELETE FROM PatientVisits WHERE Id=@Id; END TRY BEGIN CATCH THROW; END CATCH
END;
GO

CREATE OR ALTER PROCEDURE stp_GetPatientVisitById @Id INT
AS
BEGIN
    SET NOCOUNT ON; SELECT * FROM PatientVisits WHERE Id=@Id;
END;
GO

CREATE OR ALTER PROCEDURE stp_GetAllPatientVisits
AS
BEGIN
    SET NOCOUNT ON; SELECT * FROM PatientVisits;
END;
GO
    
-- =========================
-- Fee Rate Procedures
-- =========================

CREATE OR ALTER PROCEDURE stp_AddFeeRate
    @VisitTypeId INT, 
    @BaseRate DECIMAL(10,2), 
    @ExtraTimeRate DECIMAL(5,4), 
    @ExtraTimeThreshold INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY 
        IF @VisitTypeId IS NULL OR @VisitTypeId <= 0
            THROW 50021, 'Valid VisitTypeId is required', 1;
            
        IF @BaseRate IS NULL OR @BaseRate <= 0
            THROW 50022, 'Valid BaseRate is required', 1;
            
        IF @ExtraTimeRate IS NULL OR @ExtraTimeRate < 0
            THROW 50023, 'Valid ExtraTimeRate is required', 1;
            
        IF @ExtraTimeThreshold IS NULL OR @ExtraTimeThreshold <= 0
            THROW 50024, 'Valid ExtraTimeThreshold is required', 1;
 
        IF NOT EXISTS (SELECT 1 FROM VisitTypes WHERE VisitTypeId = @VisitTypeId)
            THROW 50025, 'VisitType not found', 1;

        INSERT INTO FeeRates (VisitTypeId, BaseRate, ExtraTimeRate, ExtraTimeThreshold)
        VALUES (@VisitTypeId, @BaseRate, @ExtraTimeRate, @ExtraTimeThreshold);
         
        SELECT SCOPE_IDENTITY() AS NewFeeRateId;
    END TRY 
    BEGIN CATCH 
        THROW; 
    END CATCH
END;
GO

CREATE OR ALTER PROCEDURE stp_UpdateFeeRate
    @FeeRateId INT, 
    @VisitTypeId INT, 
    @BaseRate DECIMAL(10,2), 
    @ExtraTimeRate DECIMAL(5,4), 
    @ExtraTimeThreshold INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @RowsAffected INT = 0;
    
    BEGIN TRY 
        IF @FeeRateId IS NULL OR @FeeRateId <= 0
            THROW 50026, 'Valid FeeRateId is required', 1;
            
        IF @VisitTypeId IS NULL OR @VisitTypeId <= 0
            THROW 50021, 'Valid VisitTypeId is required', 1;
            
        IF @BaseRate IS NULL OR @BaseRate <= 0
            THROW 50022, 'Valid BaseRate is required', 1;
            
        IF @ExtraTimeRate IS NULL OR @ExtraTimeRate < 0
            THROW 50023, 'Valid ExtraTimeRate is required', 1;
            
        IF @ExtraTimeThreshold IS NULL OR @ExtraTimeThreshold <= 0
            THROW 50024, 'Valid ExtraTimeThreshold is required', 1;
 
        IF NOT EXISTS (SELECT 1 FROM FeeRates WHERE FeeRateId = @FeeRateId)
            THROW 50027, 'FeeRate not found', 1;
 
        IF NOT EXISTS (SELECT 1 FROM VisitTypes WHERE VisitTypeId = @VisitTypeId)
            THROW 50025, 'VisitType not found', 1;

        UPDATE FeeRates 
        SET VisitTypeId = @VisitTypeId, 
            BaseRate = @BaseRate, 
            ExtraTimeRate = @ExtraTimeRate,
            ExtraTimeThreshold = @ExtraTimeThreshold 
        WHERE FeeRateId = @FeeRateId;
        
        SET @RowsAffected = @@ROWCOUNT;
        SELECT @RowsAffected AS RowsAffected;
    END TRY 
    BEGIN CATCH 
        THROW; 
    END CATCH
END;
GO

CREATE OR ALTER PROCEDURE stp_DeleteFeeRate 
    @FeeRateId INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @RowsAffected INT = 0;
    
    BEGIN TRY
        IF @FeeRateId IS NULL OR @FeeRateId <= 0
            THROW 50026, 'Valid FeeRateId is required', 1;
 
        IF NOT EXISTS (SELECT 1 FROM FeeRates WHERE FeeRateId = @FeeRateId)
            THROW 50027, 'FeeRate not found', 1;

        DELETE FROM FeeRates WHERE FeeRateId = @FeeRateId;
        SET @RowsAffected = @@ROWCOUNT;
        SELECT @RowsAffected AS RowsAffected;
    END TRY 
    BEGIN CATCH 
        THROW; 
    END CATCH
END;
GO

CREATE OR ALTER PROCEDURE stp_GetFeeRateById @FeeRateId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM FeeRates WHERE FeeRateId = @FeeRateId;
END;
GO

CREATE OR ALTER PROCEDURE stp_GetAllFeeRates
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM FeeRates;
END;
GO

-- =========================
-- Activity Log Procedures
-- =========================

CREATE OR ALTER PROCEDURE stp_AddActivityLog
    @Action NVARCHAR(100),
    @Success BIT,
    @Details NVARCHAR(500) = NULL,
    @UserId INT = NULL,
    @VisitId INT = NULL,
    @NewLogId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO ActivityLog (LogDateTime, Action, Success, Details, UserId, VisitId)
        VALUES (GETUTCDATE(), @Action, @Success, @Details, @UserId, @VisitId);
        
        SET @NewLogId = SCOPE_IDENTITY();
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

CREATE OR ALTER PROCEDURE stp_DeleteActivityLog 
    @LogId INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DELETE FROM ActivityLog WHERE LogId = @LogId;
        
        IF @@ROWCOUNT = 0
        BEGIN
            THROW 50001, 'Activity log not found or already deleted', 1;
        END
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

CREATE OR ALTER PROCEDURE stp_GetActivityLogById 
    @LogId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT LogId, LogDateTime, Action, Success, Details, UserId, VisitId 
    FROM ActivityLog 
    WHERE LogId = @LogId;
END;
GO

CREATE OR ALTER PROCEDURE stp_GetAllActivityLogs
AS
BEGIN
    SET NOCOUNT ON;
    SELECT LogId, LogDateTime, Action, Success, Details, UserId, VisitId 
    FROM ActivityLog 
    ORDER BY LogDateTime DESC;
END;
GO

-- Stored Procedure to check if email exists
-- USE PVMS6606
CREATE OR ALTER PROCEDURE stp_CheckEmailExists
    @Email NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM Users WHERE Email = @Email)
        SELECT 1;
    ELSE
        SELECT 0;
END


CREATE OR ALTER PROCEDURE stp_ResetPassword
    @Email NVARCHAR(100),
    @NewPassword NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Users
    SET Password = @NewPassword
    WHERE Email = @Email;

    IF @@ROWCOUNT > 0
        SELECT 1;  
    ELSE
        SELECT 0;   
END
