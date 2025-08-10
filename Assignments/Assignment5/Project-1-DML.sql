USE PatientVisitDB;

-- Roles
INSERT INTO Roles (RoleName) VALUES ('Admin'), ('Receptionist');
 
-- Users
INSERT INTO Users (Username, PasswordHash, RoleID)
VALUES ('curemd', 'curemd', 1),
       ('curemd1', 'curemd1', 2);

-- Visit Types
INSERT INTO VisitTypes (VisitTypeName) VALUES
('Checkup'),
('Emergency'),
('Follow-up'),
('Consultation'),
('Surgery'),
('Lab Test'),
('X-Ray'),
('Vaccination'),
('Physical Therapy'),
('Dental');

-- Fee Schedule
INSERT INTO FeeSchedule (VisitTypeID, BaseFee)
SELECT VisitTypeID,
       CASE VisitTypeName
            WHEN 'Consultation' THEN 500
            WHEN 'Follow-up' THEN 300
            WHEN 'Emergency' THEN 1000
            ELSE 500
       END
FROM VisitTypes;

-- Doctors
INSERT INTO Doctors (DoctorName) VALUES
('Dr. Ahmad'),
('Dr. Fatima'),
('Dr. Hassan'),
('Dr. Ayesha'),
('Dr. Ali'),
('Dr. Sara'),
('Dr. Usman'),
('Dr. Zainab'),
('Dr. Bilal'),
('Dr. Mariam');

-- Patients
INSERT INTO Patients (PatientName) VALUES
('Ahmed Ali'),
('Sara Khan'),
('Muhammad Hassan'),
('Fatima Sheikh'),
('Ali Raza'),
('Ayesha Malik'),
('Usman Ahmed'),
('Zainab Hussain'),
('Hassan Ali'),
('Mariam Qureshi');

-- Visits
INSERT INTO Visits (PatientID, DoctorID, VisitTypeID, VisitDate, Note, DurationInMinutes, Fee)
VALUES
(1, 1, 4, '2025-08-01 10:00:00', 'Consultation for back pain', 30, 500),
(2, 2, 3, '2025-08-02 14:30:00', 'Follow-up for flu recovery', 45, 375),
(3, 3, 2, '2025-08-03 09:15:00', 'Emergency visit - fever', 30, 1000);

-- Activity Logs
INSERT INTO ActivityLog (Action, Status, Details)
VALUES
('Add Visit', 'SUCCESS', 'Visit added for Ahmed Ali (ID: 1)'),
('Add Visit', 'SUCCESS', 'Visit added for Sara Khan (ID: 2)'),
('Add Visit', 'SUCCESS', 'Visit added for Muhammad Hassan (ID: 3)');
