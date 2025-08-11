-- CREATE DATABASE PatientVisitMS6606
-- USE PatientVisitMS6606


-- Users table 
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL,
    UserRole NVARCHAR(20) NOT NULL CHECK (UserRole IN ('Admin', 'Receptionist')),
    CreatedDate DATETIME2 DEFAULT GETDATE()
);

-- Doctors table
CREATE TABLE Doctors (
    DoctorId INT IDENTITY(1,1) PRIMARY KEY,
    DoctorName NVARCHAR(100) NOT NULL,
    Specialization NVARCHAR(100),
    ContactNumber NVARCHAR(15),
    Email NVARCHAR(100),
    CreatedDate DATETIME2 DEFAULT GETDATE()
);

-- Patients table
CREATE TABLE Patients (
    PatientId INT IDENTITY(1,1) PRIMARY KEY,
    PatientName NVARCHAR(100) NOT NULL,
    DateOfBirth DATE,
    Gender NVARCHAR(10) CHECK (Gender IN ('Male', 'Female', 'Other')),
    ContactNumber NVARCHAR(15),
    Email NVARCHAR(100),
    Address NVARCHAR(255),
    EmergencyContact NVARCHAR(15),
    CreatedDate DATETIME2 DEFAULT GETDATE()
    
);

-- Visit Types table 
CREATE TABLE VisitTypes (
    VisitTypeId INT IDENTITY(1,1) PRIMARY KEY,
    VisitTypeName NVARCHAR(50) NOT NULL UNIQUE,
    BaseRate DECIMAL(10,2) NOT NULL DEFAULT 500.00,
    Description NVARCHAR(255)
    
);

-- Patient Visits table 
CREATE TABLE PatientVisits (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    PatientId INT NOT NULL,
    DoctorId INT,
    VisitTypeId INT NOT NULL,
    VisitDate DATETIME2 NOT NULL,
    Note NVARCHAR(500),
    DurationInMinutes INT NOT NULL DEFAULT 30,
    Fee DECIMAL(10,2) NOT NULL,
    CreatedDate DATETIME2 DEFAULT GETDATE(),
    CreatedBy INT,
    ModifiedDate DATETIME2,
    ModifiedBy INT,
     
    CONSTRAINT FK_PatientVisits_Patients FOREIGN KEY (PatientId) REFERENCES Patients(PatientId),
    CONSTRAINT FK_PatientVisits_Doctors FOREIGN KEY (DoctorId) REFERENCES Doctors(DoctorId),
    CONSTRAINT FK_PatientVisits_VisitTypes FOREIGN KEY (VisitTypeId) REFERENCES VisitTypes(VisitTypeId),
    CONSTRAINT FK_PatientVisits_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_PatientVisits_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES Users(UserId)
);

-- Activity Log table 
CREATE TABLE ActivityLog (
    LogId INT IDENTITY(1,1) PRIMARY KEY,
    LogDateTime DATETIME2 NOT NULL DEFAULT GETDATE(),
    Action NVARCHAR(100) NOT NULL,
    Success BIT NOT NULL,
    Details NVARCHAR(500),
    UserId INT,
    VisitId INT,
    CONSTRAINT FK_ActivityLog_Users FOREIGN KEY (UserId) REFERENCES Users(UserId),
    CONSTRAINT FK_ActivityLog_PatientVisits FOREIGN KEY (VisitId) REFERENCES PatientVisits(Id)
);

-- Fee Rates table 
CREATE TABLE FeeRates (
    FeeRateId INT IDENTITY(1,1) PRIMARY KEY,
    VisitTypeId INT NOT NULL,
    BaseRate DECIMAL(10,2) NOT NULL,
    ExtraTimeRate DECIMAL(5,4) NOT NULL DEFAULT 0.25, 
    ExtraTimeThreshold INT NOT NULL DEFAULT 30,
    EffectiveDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT FK_FeeRates_VisitTypes FOREIGN KEY (VisitTypeId) REFERENCES VisitTypes(VisitTypeId)
);


CREATE INDEX IX_PatientVisits_PatientId ON PatientVisits(PatientId);
CREATE INDEX IX_PatientVisits_DoctorId ON PatientVisits(DoctorId);
CREATE INDEX IX_PatientVisits_VisitDate ON PatientVisits(VisitDate);
CREATE INDEX IX_PatientVisits_VisitTypeId ON PatientVisits(VisitTypeId);
CREATE INDEX IX_ActivityLog_LogDateTime ON ActivityLog(LogDateTime);
CREATE INDEX IX_ActivityLog_UserId ON ActivityLog(UserId);