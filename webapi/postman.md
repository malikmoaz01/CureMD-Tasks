
# PVMS 6606 - API Documentation

## 📌 ActivityLog APIs

### GET /api/ActivityLog
**Response (200 OK)**
```json
[
  {
    "logId": 1,
    "logDateTime": "2025-08-17T17:30:00Z",
    "action": "Create Visit",
    "success": true,
    "details": "Visit record created successfully.",
    "userId": 101,
    "visitId": 501
  }
]
````

---

### POST /api/ActivityLog

**Request Body**

```json
{
  "logId": 0,
  "logDateTime": "2025-08-17T19:10:00Z",
  "action": "Update Visit",
  "success": true,
  "details": "Visit record updated by user.",
  "userId": 101,
  "visitId": 503
}
```

**Response (200 OK)**

```json
{
  "message": "Activity log created successfully.",
  "logId": 2
}
```

---

### GET /api/ActivityLog/{id}

**Example:** `/api/ActivityLog/1`

**Response (200 OK)**

```json
{
  "logId": 1,
  "logDateTime": "2025-08-17T17:30:00Z",
  "action": "Create Visit",
  "success": true,
  "details": "Visit record created successfully.",
  "userId": 101,
  "visitId": 501
}
```

---

### DELETE /api/ActivityLog/{id}

**Example:** `/api/ActivityLog/2`

**Response (200 OK)**

```json
{
  "message": "Activity log with ID 2 deleted successfully."
}
```

---

## 🔑 Auth APIs

### POST /api/Auth/login

**Request Body**

```json
{
  "email": "doctor@example.com",
  "password": "Pass@123"
}
```

**Response (200 OK)**

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI...",
  "userId": 101,
  "email": "doctor@example.com",
  "role": "Admin"
}
```

---

### POST /api/Auth/register

**Request Body**

```json
{
  "email": "nurse@example.com",
  "password": "SecurePass@123",
  "userRole": "Nurse"
}
```

**Response (200 OK)**

```json
{
  "message": "User registered successfully.",
  "userId": 102
}
```

```
