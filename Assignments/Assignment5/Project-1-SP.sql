USE PatientVisitMS6606;



GO
CREATE OR ALTER PROCEDURE stp_AddUser
    @Username NVARCHAR(50),
    @Password NVARCHAR(255),
    @UserRole NVARCHAR(20),
    @NewUserId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        IF EXISTS (SELECT 1 FROM Users WHERE Username = @Username)
        BEGIN
            ROLLBACK TRANSACTION;
            RAISERROR('Username already exists', 16, 1);
            RETURN;
        END
        
        INSERT INTO Users (Username, Password, UserRole, CreatedDate)
        VALUES (@Username, @Password, @UserRole, GETUTCDATE());
        
        SET @NewUserId = SCOPE_IDENTITY();
        
        COMMIT TRANSACTION;
        
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
            
        -- Re-throw the error
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END





-- ===== FIXED PATIENT STORED PROCEDURES =====

-- Fixed Update Patient Procedure
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
    
    DECLARE @RowsAffected INT = 0;
    
    BEGIN TRY
        -- Check if patient exists
        IF NOT EXISTS (SELECT 1 FROM Patients WHERE PatientId = @PatientId) 
        BEGIN
            THROW 50010, 'Patient not found', 1;
        END
        
        -- Validate required fields
        IF @PatientName IS NULL OR LTRIM(RTRIM(@PatientName)) = '' 
        BEGIN
            THROW 50008, 'PatientName is required', 1;
        END
        
        -- Validate Gender only if it's provided (not NULL)
        IF @Gender IS NOT NULL AND @Gender NOT IN ('Male', 'Female', 'Other') 
        BEGIN
            THROW 50009, 'Invalid Gender. Must be Male, Female, or Other', 1;
        END
        
        -- Update patient
        UPDATE Patients 
        SET PatientName = @PatientName, 
            DateOfBirth = @DateOfBirth, 
            Gender = @Gender,
            ContactNumber = @ContactNumber, 
            Email = @Email, 
            Address = @Address, 
            EmergencyContact = @EmergencyContact
        WHERE PatientId = @PatientId;
        
        SET @RowsAffected = @@ROWCOUNT;
        
        -- Return success indicator
        SELECT @RowsAffected as RowsAffected;
        
    END TRY 
    BEGIN CATCH 
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorNumber INT = ERROR_NUMBER();
        THROW @ErrorNumber, @ErrorMessage, 1;
    END CATCH
END;
GO

-- Fixed Delete Patient Procedure
CREATE OR ALTER PROCEDURE stp_DeletePatient 
    @PatientId INT
AS
BEGIN
    SET NOCOUNT ON; 
    
    DECLARE @RowsAffected INT = 0;
    
    BEGIN TRY  
        IF NOT EXISTS (SELECT 1 FROM Patients WHERE PatientId = @PatientId)
        BEGIN
            THROW 50010, 'Patient not found', 1;
        END
        
        DELETE FROM Patients WHERE PatientId = @PatientId;
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

















-- ===== FIXED DOCTORS STORED PROCEDURES =====

-- Fixed Add Doctor Procedure
CREATE OR ALTER PROCEDURE stp_AddDoctor
    @DoctorName NVARCHAR(100), 
    @Specialization NVARCHAR(100), 
    @ContactNumber NVARCHAR(15), 
    @Email NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        -- Validate required fields
        IF @DoctorName IS NULL OR LTRIM(RTRIM(@DoctorName)) = '' 
            THROW 50006, 'DoctorName is required', 1;
        
        -- Insert doctor and return the new ID
        INSERT INTO Doctors (DoctorName, Specialization, ContactNumber, Email) 
        VALUES (@DoctorName, @Specialization, @ContactNumber, @Email);
        
        -- Return the new doctor ID
        SELECT SCOPE_IDENTITY() as NewDoctorId;
        
    END TRY 
    BEGIN CATCH 
        -- Re-throw the error with details
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorNumber INT = ERROR_NUMBER();
        THROW @ErrorNumber, @ErrorMessage, 1;
    END CATCH
END;
GO
-- ===== FIXED DELETE AND UPDATE PROCEDURES =====

-- Fixed Delete Doctor Procedure
CREATE OR ALTER PROCEDURE stp_DeleteDoctor 
    @DoctorId INT
AS
BEGIN
    SET NOCOUNT ON; 
    
    DECLARE @RowsAffected INT = 0;
    
    BEGIN TRY 
        -- Check if doctor exists before deletion
        IF NOT EXISTS (SELECT 1 FROM Doctors WHERE DoctorId = @DoctorId)
        BEGIN
            THROW 50007, 'Doctor not found', 1;
        END
        -- Perform the deletion
        DELETE FROM Doctors WHERE DoctorId = @DoctorId;
        SET @RowsAffected = @@ROWCOUNT;
        
        -- Return success indicator
        SELECT @RowsAffected as RowsAffected;
        
    END TRY 
    BEGIN CATCH 
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorNumber INT = ERROR_NUMBER();
        THROW @ErrorNumber, @ErrorMessage, 1;
    END CATCH
END;
GO

-- Fixed Update Doctor Procedure  
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
        -- Check if doctor exists
        IF NOT EXISTS (SELECT 1 FROM Doctors WHERE DoctorId = @DoctorId) 
        BEGIN
            THROW 50007, 'Doctor not found', 1;
        END
        
        -- Validate required fields
        IF @DoctorName IS NULL OR LTRIM(RTRIM(@DoctorName)) = '' 
        BEGIN
            THROW 50006, 'DoctorName is required', 1;
        END
        
        -- Update doctor
        UPDATE Doctors 
        SET DoctorName = @DoctorName, 
            Specialization = @Specialization, 
            ContactNumber = @ContactNumber, 
            Email = @Email 
        WHERE DoctorId = @DoctorId;
        
        SET @RowsAffected = @@ROWCOUNT;
        
        -- Return success indicator
        SELECT @RowsAffected as RowsAffected;
        
    END TRY 
    BEGIN CATCH 
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorNumber INT = ERROR_NUMBER();
        THROW @ErrorNumber, @ErrorMessage, 1;
    END CATCH
END;
GO

-- Get Doctor by ID (no changes needed)
CREATE OR ALTER PROCEDURE stp_GetDoctorById 
    @DoctorId INT
AS
BEGIN
    SET NOCOUNT ON; 
    SELECT * FROM Doctors WHERE DoctorId = @DoctorId;
END;
GO

-- Get All Doctors (no changes needed)
CREATE OR ALTER PROCEDURE stp_GetAllDoctors
AS
BEGIN
    SET NOCOUNT ON; 
    SELECT * FROM Doctors ORDER BY CreatedDate DESC;
END;
GO



 

-- Fixed Add Patient Procedure
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
        -- Validate required fields
        IF @PatientName IS NULL OR LTRIM(RTRIM(@PatientName)) = '' 
            THROW 50008, 'PatientName is required', 1;
        
        -- Validate Gender only if it's provided (not NULL)
        IF @Gender IS NOT NULL AND @Gender NOT IN ('Male', 'Female', 'Other') 
            THROW 50009, 'Invalid Gender. Must be Male, Female, or Other', 1;
        
        -- Insert patient and return the new ID
        INSERT INTO Patients (PatientName, DateOfBirth, Gender, ContactNumber, Email, Address, EmergencyContact)
        VALUES (@PatientName, @DateOfBirth, @Gender, @ContactNumber, @Email, @Address, @EmergencyContact);
        
        -- Return the new patient ID
        SELECT SCOPE_IDENTITY() as NewPatientId;
        
    END TRY 
    BEGIN CATCH 
        -- Re-throw the error with details
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorNumber INT = ERROR_NUMBER();
        THROW @ErrorNumber, @ErrorMessage, 1;
    END CATCH
END;
GO

-- Fixed Update Patient Procedure
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
        -- Check if patient exists
        IF NOT EXISTS (SELECT 1 FROM Patients WHERE PatientId = @PatientId) 
            THROW 50010, 'Patient not found', 1;
        
        -- Validate required fields
        IF @PatientName IS NULL OR LTRIM(RTRIM(@PatientName)) = '' 
            THROW 50008, 'PatientName is required', 1;
        
        -- Validate Gender only if it's provided (not NULL)
        IF @Gender IS NOT NULL AND @Gender NOT IN ('Male', 'Female', 'Other') 
            THROW 50009, 'Invalid Gender. Must be Male, Female, or Other', 1;
        
        -- Update patient
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

-- Fixed Delete Patient Procedure
CREATE OR ALTER PROCEDURE stp_DeletePatient 
    @PatientId INT
AS
BEGIN
    SET NOCOUNT ON; 
    BEGIN TRY 
        -- Check if patient exists before deletion
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

-- Get Patient by ID (no changes needed)
CREATE OR ALTER PROCEDURE stp_GetPatientById 
    @PatientId INT
AS
BEGIN
    SET NOCOUNT ON; 
    SELECT * FROM Patients WHERE PatientId = @PatientId;
END;
GO

-- Get All Patients (no changes needed)
CREATE OR ALTER PROCEDURE stp_GetAllPatients
AS
BEGIN
    SET NOCOUNT ON; 
    SELECT * FROM Patients ORDER BY CreatedDate DESC;
END;
GO






-- ===== VISITTYPES =====
CREATE PROCEDURE stp_AddVisitType
    @VisitTypeName NVARCHAR(50), @BaseRate DECIMAL(10,2), @Description NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF @VisitTypeName IS NULL OR LTRIM(RTRIM(@VisitTypeName))='' THROW 50011, 'VisitTypeName is required', 1;
        INSERT INTO VisitTypes (VisitTypeName, BaseRate, Description) VALUES (@VisitTypeName, @BaseRate, @Description);
    END TRY BEGIN CATCH THROW; END CATCH
END;
GO

CREATE PROCEDURE stp_UpdateVisitType
    @VisitTypeId INT, @VisitTypeName NVARCHAR(50), @BaseRate DECIMAL(10,2), @Description NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM VisitTypes WHERE VisitTypeId=@VisitTypeId) THROW 50012, 'VisitType not found', 1;
        UPDATE VisitTypes SET VisitTypeName=@VisitTypeName, BaseRate=@BaseRate, Description=@Description WHERE VisitTypeId=@VisitTypeId;
    END TRY BEGIN CATCH THROW; END CATCH
END;
GO

CREATE PROCEDURE stp_DeleteVisitType @VisitTypeId INT
AS
BEGIN
    SET NOCOUNT ON; BEGIN TRY DELETE FROM VisitTypes WHERE VisitTypeId=@VisitTypeId; END TRY BEGIN CATCH THROW; END CATCH
END;
GO

CREATE PROCEDURE stp_GetVisitTypeById @VisitTypeId INT
AS
BEGIN
    SET NOCOUNT ON; SELECT * FROM VisitTypes WHERE VisitTypeId=@VisitTypeId;
END;
GO

CREATE PROCEDURE stp_GetAllVisitTypes
AS
BEGIN
    SET NOCOUNT ON; SELECT * FROM VisitTypes;
END;
GO






-- ===== PATIENTVISITS =====
CREATE PROCEDURE stp_AddPatientVisit
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

CREATE PROCEDURE stp_UpdatePatientVisit
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

CREATE PROCEDURE stp_DeletePatientVisit @Id INT
AS
BEGIN
    SET NOCOUNT ON; BEGIN TRY DELETE FROM PatientVisits WHERE Id=@Id; END TRY BEGIN CATCH THROW; END CATCH
END;
GO

CREATE PROCEDURE stp_GetPatientVisitById @Id INT
AS
BEGIN
    SET NOCOUNT ON; SELECT * FROM PatientVisits WHERE Id=@Id;
END;
GO

CREATE PROCEDURE stp_GetAllPatientVisits
AS
BEGIN
    SET NOCOUNT ON; SELECT * FROM PatientVisits;
END;
GO







-- ===== FEERATES =====
CREATE PROCEDURE stp_AddFeeRate
    @VisitTypeId INT, @BaseRate DECIMAL(10,2), @ExtraTimeRate DECIMAL(5,4), @ExtraTimeThreshold INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO FeeRates (VisitTypeId, BaseRate, ExtraTimeRate, ExtraTimeThreshold)
        VALUES (@VisitTypeId, @BaseRate, @ExtraTimeRate, @ExtraTimeThreshold);
    END TRY BEGIN CATCH THROW; END CATCH
END;
GO

CREATE PROCEDURE stp_UpdateFeeRate
    @FeeRateId INT, @VisitTypeId INT, @BaseRate DECIMAL(10,2), @ExtraTimeRate DECIMAL(5,4), @ExtraTimeThreshold INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        UPDATE FeeRates SET VisitTypeId=@VisitTypeId, BaseRate=@BaseRate, ExtraTimeRate=@ExtraTimeRate,
        ExtraTimeThreshold=@ExtraTimeThreshold WHERE FeeRateId=@FeeRateId;
    END TRY BEGIN CATCH THROW; END CATCH
END;
GO

CREATE PROCEDURE stp_DeleteFeeRate @FeeRateId INT
AS
BEGIN
    SET NOCOUNT ON; BEGIN TRY DELETE FROM FeeRates WHERE FeeRateId=@FeeRateId; END TRY BEGIN CATCH THROW; END CATCH
END;
GO

CREATE PROCEDURE stp_GetFeeRateById @FeeRateId INT
AS
BEGIN
    SET NOCOUNT ON; SELECT * FROM FeeRates WHERE FeeRateId=@FeeRateId;
END;
GO

CREATE PROCEDURE stp_GetAllFeeRates
AS
BEGIN
    SET NOCOUNT ON; SELECT * FROM FeeRates;
END;
GO







-- ===== ACTIVITYLOG =====
CREATE PROCEDURE stp_AddActivityLog
    @Action NVARCHAR(100), @Success BIT, @Details NVARCHAR(500), @UserId INT, @VisitId INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO ActivityLog (Action, Success, Details, UserId, VisitId)
        VALUES (@Action, @Success, @Details, @UserId, @VisitId);
    END TRY BEGIN CATCH THROW; END CATCH
END;
GO

CREATE PROCEDURE stp_DeleteActivityLog @LogId INT
AS
BEGIN
    SET NOCOUNT ON; BEGIN TRY DELETE FROM ActivityLog WHERE LogId=@LogId; END TRY BEGIN CATCH THROW; END CATCH
END;
GO

CREATE PROCEDURE stp_GetActivityLogById @LogId INT
AS
BEGIN
    SET NOCOUNT ON; SELECT * FROM ActivityLog WHERE LogId=@LogId;
END;
GO

CREATE PROCEDURE stp_GetAllActivityLogs
AS
BEGIN
    SET NOCOUNT ON; SELECT * FROM ActivityLog;
END;
GO
