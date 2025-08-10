-- ==============================
-- Patients Table CRUD
-- ==============================

CREATE PROCEDURE stp_AddPatient
    @PatientName NVARCHAR(100),
    @DOB DATE,
    @Gender NVARCHAR(10)
AS
BEGIN
    INSERT INTO Patients (PatientName, DOB, Gender)
    VALUES (@PatientName, @DOB, @Gender);
END
GO

CREATE PROCEDURE stp_UpdatePatient
    @PatientID INT,
    @PatientName NVARCHAR(100),
    @DOB DATE,
    @Gender NVARCHAR(10)
AS
BEGIN
    UPDATE Patients
    SET PatientName = @PatientName,
        DOB = @DOB,
        Gender = @Gender
    WHERE PatientID = @PatientID;
END
GO

CREATE PROCEDURE stp_DeletePatient
    @PatientID INT
AS
BEGIN
    DELETE FROM Patients WHERE PatientID = @PatientID;
END
GO

CREATE PROCEDURE stp_GetPatient
    @PatientID INT
AS
BEGIN
    SELECT * FROM Patients WHERE PatientID = @PatientID;
END
GO

CREATE PROCEDURE stp_GetAllPatients
AS
BEGIN
    SELECT * FROM Patients;
END
GO


-- ==============================
-- VisitTypes Table CRUD
-- ==============================

CREATE PROCEDURE stp_AddVisitType
    @VisitTypeName NVARCHAR(50)
AS
BEGIN
    INSERT INTO VisitTypes (VisitTypeName)
    VALUES (@VisitTypeName);
END
GO

CREATE PROCEDURE stp_UpdateVisitType
    @VisitTypeID INT,
    @VisitTypeName NVARCHAR(50)
AS
BEGIN
    UPDATE VisitTypes
    SET VisitTypeName = @VisitTypeName
    WHERE VisitTypeID = @VisitTypeID;
END
GO

CREATE PROCEDURE stp_DeleteVisitType
    @VisitTypeID INT
AS
BEGIN
    DELETE FROM VisitTypes WHERE VisitTypeID = @VisitTypeID;
END
GO

CREATE PROCEDURE stp_GetVisitType
    @VisitTypeID INT
AS
BEGIN
    SELECT * FROM VisitTypes WHERE VisitTypeID = @VisitTypeID;
END
GO

CREATE PROCEDURE stp_GetAllVisitTypes
AS
BEGIN
    SELECT * FROM VisitTypes;
END
GO


-- ==============================
-- Roles Table CRUD
-- ==============================

CREATE PROCEDURE stp_AddRole
    @RoleName NVARCHAR(50)
AS
BEGIN
    INSERT INTO Roles (RoleName)
    VALUES (@RoleName);
END
GO

CREATE PROCEDURE stp_UpdateRole
    @RoleID INT,
    @RoleName NVARCHAR(50)
AS
BEGIN
    UPDATE Roles
    SET RoleName = @RoleName
    WHERE RoleID = @RoleID;
END
GO

CREATE PROCEDURE stp_DeleteRole
    @RoleID INT
AS
BEGIN
    DELETE FROM Roles WHERE RoleID = @RoleID;
END
GO

CREATE PROCEDURE stp_GetRole
    @RoleID INT
AS
BEGIN
    SELECT * FROM Roles WHERE RoleID = @RoleID;
END
GO

CREATE PROCEDURE stp_GetAllRoles
AS
BEGIN
    SELECT * FROM Roles;
END
GO


-- ==============================
-- FeeSchedule Table CRUD
-- ==============================

CREATE PROCEDURE stp_AddFeeSchedule
    @VisitTypeID INT,
    @FeeAmount DECIMAL(10,2)
AS
BEGIN
    INSERT INTO FeeSchedule (VisitTypeID, FeeAmount)
    VALUES (@VisitTypeID, @FeeAmount);
END
GO

CREATE PROCEDURE stp_UpdateFeeSchedule
    @FeeScheduleID INT,
    @VisitTypeID INT,
    @FeeAmount DECIMAL(10,2)
AS
BEGIN
    UPDATE FeeSchedule
    SET VisitTypeID = @VisitTypeID,
        FeeAmount = @FeeAmount
    WHERE FeeScheduleID = @FeeScheduleID;
END
GO

CREATE PROCEDURE stp_DeleteFeeSchedule
    @FeeScheduleID INT
AS
BEGIN
    DELETE FROM FeeSchedule WHERE FeeScheduleID = @FeeScheduleID;
END
GO

CREATE PROCEDURE stp_GetFeeSchedule
    @FeeScheduleID INT
AS
BEGIN
    SELECT * FROM FeeSchedule WHERE FeeScheduleID = @FeeScheduleID;
END
GO

CREATE PROCEDURE stp_GetAllFeeSchedules
AS
BEGIN
    SELECT * FROM FeeSchedule;
END
GO


-- ==============================
-- Visits Table CRUD
-- ==============================

CREATE PROCEDURE stp_AddVisit
    @PatientID INT,
    @VisitTypeID INT,
    @VisitDate DATE,
    @FeeScheduleID INT
AS
BEGIN
    INSERT INTO Visits (PatientID, VisitTypeID, VisitDate, FeeScheduleID)
    VALUES (@PatientID, @VisitTypeID, @VisitDate, @FeeScheduleID);
END
GO

CREATE PROCEDURE stp_UpdateVisit
    @VisitID INT,
    @PatientID INT,
    @VisitTypeID INT,
    @VisitDate DATE,
    @FeeScheduleID INT
AS
BEGIN
    UPDATE Visits
    SET PatientID = @PatientID,
        VisitTypeID = @VisitTypeID,
        VisitDate = @VisitDate,
        FeeScheduleID = @FeeScheduleID
    WHERE VisitID = @VisitID;
END
GO

CREATE PROCEDURE stp_DeleteVisit
    @VisitID INT
AS
BEGIN
    DELETE FROM Visits WHERE VisitID = @VisitID;
END
GO

CREATE PROCEDURE stp_GetVisit
    @VisitID INT
AS
BEGIN
    SELECT * FROM Visits WHERE VisitID = @VisitID;
END
GO

CREATE PROCEDURE stp_GetAllVisits
AS
BEGIN
    SELECT * FROM Visits;
END
GO
