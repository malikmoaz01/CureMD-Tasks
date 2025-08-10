-- USE PatientVisitMS6606;

-- Users Table
INSERT INTO Users (Username, Password, UserRole) VALUES
('admin1', 'pass123', 'Admin'),
('admin2', 'pass456', 'Admin'),
('reception1', 'pass789', 'Receptionist'),
('reception2', 'pass321', 'Receptionist'),
('reception3', 'pass654', 'Receptionist');

-- Doctors Table
INSERT INTO Doctors (DoctorName, Specialization, ContactNumber, Email) VALUES
('Dr. Ahmed Khan', 'Cardiologist', '03001234567', 'ahmed.khan@gmail.com.com'),
('Dr. Sara Malik', 'Dermatologist', '03019876543', 'sara.malik@gmail.com.com'),
('Dr. Ali Raza', 'Orthopedic', '03017654321', 'ali.raza@gmail.com.com'),
('Dr. Mehwish Iqbal', 'Pediatrician', '03013456789', 'mehwish.iqbal@gmail.com.com'),
('Dr. Faisal Noor', 'Neurologist', '03014567890', 'faisal.noor@gmail.com.com');

-- Patients Table
INSERT INTO Patients (PatientName, DateOfBirth, Gender, ContactNumber, Email, Address, EmergencyContact) VALUES
('Hassan Ali', '1990-05-10', 'Male', '03001112222', 'hassan.ali@gmail.com.com', 'Lahore', '03009998888'),
('Maria Khan', '1985-07-15', 'Female', '03003334444', 'maria.khan@gmail.com.com', 'Karachi', '03007776666'),
('Usman Rafiq', '2000-01-20', 'Male', '03005556666', 'usman.rafiq@gmail.com.com', 'Islamabad', '03008889999'),
('Ayesha Ahmed', '1995-03-25', 'Female', '03007778888', 'ayesha.ahmed@gmail.com.com', 'Faisalabad', '03001110000'),
('Bilal Tariq', '1988-12-12', 'Male', '03009990000', 'bilal.tariq@gmail.com.com', 'Multan', '03002223333'),
('Sara Javed', '1992-08-05', 'Female', '03004445555', 'sara.javed@gmail.com.com', 'Rawalpindi', '03006667777'),
('Hamza Qureshi', '1989-11-11', 'Male', '03008887777', 'hamza.qureshi@gmail.com.com', 'Hyderabad', '03005557777'),
('Fatima Noor', '1993-09-09', 'Female', '03006665555', 'fatima.noor@gmail.com.com', 'Peshawar', '03003337777'),
('Tariq Mehmood', '1980-02-02', 'Male', '03001113333', 'tariq.mehmood@gmail.com.com', 'Sialkot', '03009997777'),
('Maha Ali', '1998-06-06', 'Female', '03002224444', 'maha.ali@gmail.com.com', 'Quetta', '03008886666');

-- VisitTypes Table
INSERT INTO VisitTypes (VisitTypeName, BaseRate, Description) VALUES
('General Consultation', 500.00, 'Basic health consultation'),
('Specialist Consultation', 1000.00, 'Consultation with a specialist'),
('Follow-up Visit', 300.00, 'Follow-up after treatment'),
('Emergency Visit', 1500.00, 'Urgent care visit'),
('Surgery Consultation', 2000.00, 'Pre-surgery discussion');

-- FeeRates Table
INSERT INTO FeeRates (VisitTypeId, BaseRate, ExtraTimeRate, ExtraTimeThreshold) VALUES
(1, 500.00, 0.25, 30),
(2, 1000.00, 0.30, 30),
(3, 300.00, 0.20, 30),
(4, 1500.00, 0.50, 30),
(5, 2000.00, 0.60, 30);

-- PatientVisits Table
INSERT INTO PatientVisits (PatientId, DoctorId, VisitTypeId, VisitDate, Note, DurationInMinutes, Fee, CreatedBy) VALUES
(1, 1, 1, '2025-08-01 10:00', 'Routine checkup', 30, 500.00, 1),
(2, 2, 2, '2025-08-02 11:00', 'Skin rash consultation', 40, 1100.00, 3),
(3, 3, 3, '2025-08-03 09:30', 'Post-surgery follow-up', 30, 300.00, 2),
(4, 4, 4, '2025-08-04 14:00', 'Emergency case', 50, 1750.00, 4),
(5, 5, 5, '2025-08-05 13:00', 'Pre-surgery meeting', 60, 2300.00, 5),
(6, 1, 1, '2025-08-06 10:15', 'Cold symptoms', 30, 500.00, 3),
(7, 2, 2, '2025-08-07 12:00', 'Allergy consultation', 45, 1125.00, 1),
(8, 3, 3, '2025-08-08 15:00', 'Follow-up visit', 30, 300.00, 2),
(9, 4, 4, '2025-08-09 17:00', 'Severe pain', 60, 1800.00, 4),
(10, 5, 5, '2025-08-10 09:00', 'Surgery consultation', 90, 2600.00, 5);

-- ActivityLog Table
INSERT INTO ActivityLog (Action, Success, Details, UserId, VisitId) VALUES
('Create Visit', 1, 'Visit for Patient 1', 1, 1),
('Create Visit', 1, 'Visit for Patient 2', 3, 2),
('Create Visit', 1, 'Visit for Patient 3', 2, 3),
('Create Visit', 1, 'Visit for Patient 4', 4, 4),
('Create Visit', 1, 'Visit for Patient 5', 5, 5),
('Update Visit', 1, 'Updated visit duration', 3, 6),
('Update Visit', 1, 'Updated visit notes', 1, 7),
('Cancel Visit', 0, 'Visit cancelled by patient', 2, 8),
('Create Visit', 1, 'New emergency visit', 4, 9),
('Create Visit', 1, 'Surgery scheduled', 5, 10);
