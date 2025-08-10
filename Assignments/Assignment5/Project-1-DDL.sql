CREATE DATABASE PatientVisitDB;
USE PatientVisitDB;

-- 1. Roles
CREATE TABLE Roles (
    RoleID INT PRIMARY KEY IDENTITY(1,1),
    RoleName NVARCHAR(50) NOT NULL UNIQUE
);

-- 2. Users
CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    RoleID INT NOT NULL,
    FOREIGN KEY (RoleID) REFERENCES Roles(RoleID)
);

-- 3. Patients
CREATE TABLE Patients (
    PatientID INT PRIMARY KEY IDENTITY(1,1),
    PatientName NVARCHAR(100) NOT NULL
);

-- 4. Doctors
CREATE TABLE Doctors (
    DoctorID INT PRIMARY KEY IDENTITY(1,1),
    DoctorName NVARCHAR(100) NOT NULL
);

-- 5. VisitTypes
CREATE TABLE VisitTypes (
    VisitTypeID INT PRIMARY KEY IDENTITY(1,1),
    VisitTypeName NVARCHAR(100) NOT NULL UNIQUE
);

-- 6. FeeSchedule
CREATE TABLE FeeSchedule (
    FeeID INT PRIMARY KEY IDENTITY(1,1),
    VisitTypeID INT NOT NULL,
    BaseFee DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (VisitTypeID) REFERENCES VisitTypes(VisitTypeID)
);

-- 7. Visits
CREATE TABLE Visits (
    VisitID INT PRIMARY KEY IDENTITY(1,1),
    PatientID INT NOT NULL,
    DoctorID INT NULL,
    VisitTypeID INT NOT NULL,
    VisitDate DATETIME NOT NULL,
    Note NVARCHAR(500),
    DurationInMinutes INT NOT NULL DEFAULT 30,
    Fee DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (PatientID) REFERENCES Patients(PatientID),
    FOREIGN KEY (DoctorID) REFERENCES Doctors(DoctorID),
    FOREIGN KEY (VisitTypeID) REFERENCES VisitTypes(VisitTypeID)
);

-- 8. ActivityLog
CREATE TABLE ActivityLog (
    LogID INT PRIMARY KEY IDENTITY(1,1),
    Action NVARCHAR(50) NOT NULL,
    Status NVARCHAR(20) NOT NULL CHECK (Status IN ('SUCCESS', 'FAILURE')),
    Details NVARCHAR(500),
    LogDate DATETIME NOT NULL DEFAULT GETDATE()
);
