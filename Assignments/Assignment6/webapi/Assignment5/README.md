# **Patient Visit Management System 6606 CureMD**

## **1. Design Decisions**

* **Relational Database Model**
  The schema follows a normalized relational model to reduce redundancy and maintain data integrity.

* **Entity Separation**

  * **Users**: Stores system users (Admins, Receptionists) with roles for access control.
  * **Doctors**: Holds doctor details and specializations.
  * **Patients**: Contains patient personal and contact information.
  * **VisitTypes**: Lookup table for categorizing visits (General, Specialist, Emergency, etc.).
  * **PatientVisits**: Tracks visits, linking patients, doctors, and visit types.
  * **ActivityLog**: Records system actions for auditing.
  * **FeeRates**: Manages base and extra time charges for different visit types.

* **Referential Integrity**
  Foreign keys ensure valid relationships between entities (e.g., `PatientVisits.PatientId` → `Patients.PatientId`).

* **Indexes**
  Common query fields (`PatientId`, `DoctorId`, `VisitDate`, etc.) have indexes to improve performance.

---

## **2. Normalization Steps**

* **1NF**:

  * No repeating groups; each attribute contains atomic values (e.g., contact numbers are separate fields).

* **2NF**:

  * All non-key attributes fully depend on the primary key (e.g., visit details depend on `Id` in `PatientVisits`).

* **3NF**:

  * No transitive dependencies (e.g., `BaseRate` moved to `FeeRates` instead of being stored directly in `VisitTypes`).

---

## **3. How to Run the Scripts**

**Make Sure You have to USE SAME DATABASE**.

1. **Open SQL Server Management Studio (SSMS)**.
2. Execute the **DDL Script** (creates database, tables, and indexes).
3. Execute the **DML Script** (inserts sample data into tables).

   Example order:

   ```sql 
   CREATE DATABASE PatientVisitMS6606;
   USE PatientVisitMS6606; 

   INSERT INTO Users (...) VALUES (...); 
   ```

---

## **4. How to Execute Stored Procedures (SPs)**

You have to execute by using EXEC Command like this i do in add patient 

```sql
-- Add a new patient
EXEC stp_AddPatient 
    @PatientName = 'Malik Moaz',
    @DateOfBirth = '2003-05-10',
    @Gender = 'Male',
    @ContactNumber = '03001234567',
    @Email = 'malik.moaz@email.com',
    @Address = 'Lahore',
    @EmergencyContact = '03009998888';
```

---

## **5. Key Features Supported**

* Role-based user management.
* Patient visit scheduling and logging.
* Fee calculation based on visit type and duration.
* Historical activity tracking.
* Data integrity with constraints and foreign keys.

---
