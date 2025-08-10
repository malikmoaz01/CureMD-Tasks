-- Project-1-SP.sql

--------------------
-- Patients CRUD
--------------------
CREATE PROCEDURE stp_AddPatient
    @PatientName NVARCHAR(100)
AS
BEGIN
    INSERT INTO Patients (PatientName)
    VALUES (@PatientName);
END;
GO

CREATE PROCEDURE stp_UpdatePatient
    @PatientID INT,
    @PatientName NVARCHAR(100)
AS
BEGIN
    UPDATE Patients
    SET PatientName = @PatientName
    WHERE PatientID = @PatientID;
END;
GO

CREATE PROCEDURE stp_DeletePatient
    @PatientID INT
AS
BEGIN
    DELETE FROM Patients
    WHERE PatientID = @PatientID;
END;
GO

CREATE PROCEDURE stp_GetPatients
AS
BEGIN
    SELECT * FROM Patients;
END;
GO


--------------------
-- Doctors CRUD
--------------------
CREATE PROCEDURE stp_AddDoctor
    @DoctorName NVARCHAR(100)
AS
BEGIN
    INSERT INTO Doctors (DoctorName)
    VALUES (@DoctorName);
END;
GO

CREATE PROCEDURE stp_UpdateDoctor
    @DoctorID INT,
    @DoctorName NVARCHAR(100)
AS
BEGIN
    UPDATE Doctors
    SET DoctorName = @DoctorName
    WHERE DoctorID = @DoctorID;
END;
GO

CREATE PROCEDURE stp_DeleteDoctor
    @DoctorID INT
AS
BEGIN
    DELETE FROM Doctors
    WHERE DoctorID = @DoctorID;
END;
GO

CREATE PROCEDURE stp_GetDoctors
AS
BEGIN
    SELECT * FROM Doctors;
END;
GO


--------------------
-- VisitTypes CRUD
--------------------
CREATE PROCEDURE stp_AddVisitType
    @VisitTypeName NVARCHAR(100)
AS
BEGIN
    INSERT INTO VisitTypes (VisitTypeName)
    VALUES (@VisitTypeName);
END;
GO

CREATE PROCEDURE stp_UpdateVisitType
    @VisitTypeID INT,
    @VisitTypeName NVARCHAR(100)
AS
BEGIN
    UPDATE VisitTypes
    SET VisitTypeName = @VisitTypeName
    WHERE VisitTypeID = @VisitTypeID;
END;
GO

CREATE PROCEDURE stp_DeleteVisitType
    @VisitTypeID INT
AS
BEGIN
    DELETE FROM VisitTypes
    WHERE VisitTypeID = @VisitTypeID;
END;
GO

CREATE PROCEDURE stp_GetVisitTypes
AS
BEGIN
    SELECT * FROM VisitTypes;
END;
GO


--------------------
-- FeeSchedule CRUD
--------------------
CREATE PROCEDURE stp_AddFeeSchedule
    @VisitTypeID INT,
    @BaseFee DECIMAL(10,2)
AS
BEGIN
    INSERT INTO FeeSchedule (VisitTypeID, BaseFee)
    VALUES (@VisitTypeID, @BaseFee);
END;
GO

CREATE PROCEDURE stp_UpdateFeeSchedule
    @FeeID INT,
    @VisitTypeID INT,
    @BaseFee DECIMAL(10,2)
AS
BEGIN
    UPDATE FeeSchedule
    SET VisitTypeID = @VisitTypeID,
        BaseFee = @BaseFee
    WHERE FeeID = @FeeID;
END;
GO

CREATE PROCEDURE stp_DeleteFeeSchedule
    @FeeID INT
AS
BEGIN
    DELETE FROM FeeSchedule
    WHERE FeeID = @FeeID;
END;
GO

CREATE PROCEDURE stp_GetFeeSchedules
AS
BEGIN
    SELECT * FROM FeeSchedule;
END;
GO


--------------------
-- Visits CRUD
--------------------
CREATE PROCEDURE stp_AddVisit
    @PatientID INT,
    @DoctorID INT,
    @VisitTypeID INT,
    @VisitDate DATETIME,
    @Note NVARCHAR(500),
    @DurationInMinutes INT,
    @Fee DECIMAL(10,2)
AS
BEGIN
    INSERT INTO Visits (PatientID, DoctorID, VisitTypeID, VisitDate, Note, DurationInMinutes, Fee)
    VALUES (@PatientID, @DoctorID, @VisitTypeID, @VisitDate, @Note, @DurationInMinutes, @Fee);
END;
GO

CREATE PROCEDURE stp_UpdateVisit
    @VisitID INT,
    @PatientID INT,
    @DoctorID INT,
    @VisitTypeID INT,
    @VisitDate DATETIME,
    @Note NVARCHAR(500),
    @DurationInMinutes INT,
    @Fee DECIMAL(10,2)
AS
BEGIN
    UPDATE Visits
    SET PatientID = @PatientID,
        DoctorID = @DoctorID,
        VisitTypeID = @VisitTypeID,
        VisitDate = @VisitDate,
        Note = @Note,
        DurationInMinutes = @DurationInMinutes,
        Fee = @Fee
    WHERE VisitID = @VisitID;
END;
GO

CREATE PROCEDURE stp_DeleteVisit
    @VisitID INT
AS
BEGIN
    DELETE FROM Visits
    WHERE VisitID = @VisitID;
END;
GO

CREATE PROCEDURE stp_GetVisits
AS
BEGIN
    SELECT * FROM Visits;
END;
GO
