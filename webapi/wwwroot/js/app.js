const API_BASE_URL = '/api';

const currentData = {
    patients: [],
    doctors: [],
    visits: [],
    visitTypes: []
};

document.addEventListener('DOMContentLoaded', async function () {
    checkAuth();
    initializeUserInfo();
    highlightActiveTab();
    initializeValidation();

    const page = document.body.getAttribute('data-page');

    if (page === 'patients') {
        await loadPatients();
        wirePatientForm();
    } else if (page === 'doctors') {
        await loadDoctors();
        wireDoctorForm();
    } else if (page === 'visits') {
        await loadPatients();
        await loadDoctors();
        await loadVisitTypes();
        await loadVisits();
        wireVisitForm();
    } else if (page === 'visitTypes') {
        await loadVisitTypes();
        wireVisitTypeForm();
    }

    wireVisitTypeSelectFeeAutofill();
});

function checkAuth() {
    const token = localStorage.getItem('token');
    if (!token) {
        window.location.href = '../login.html';
        return;
    }
}

function initializeUserInfo() {
    const username = localStorage.getItem('username') || 'User';
    const userRole = localStorage.getItem('userRole') || 'User';
    const userInfo = document.getElementById('userInfo');
    if (userInfo) {
        userInfo.textContent = `Welcome, ${username} (${userRole})`;
    }
    if (userRole === 'Admin') {
        document.querySelectorAll('.admin-only').forEach(el => {
            el.classList.add('show');
        });
    }
}

function logout() {
    localStorage.clear();
    window.location.href = 'login.html';
}

function highlightActiveTab() {
    const page = document.body.getAttribute('data-page');
    document.querySelectorAll('.nav-tab').forEach(tab => {
        if (tab.dataset.tab === page) {
            tab.classList.add('active');
        } else {
            tab.classList.remove('active');
        }
    });
}

function makeApiRequest(endpoint, options = {}) {
    const token = localStorage.getItem('token');
    const method = options.method || 'GET';
    const headers = {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`,
        ...options.headers
    };
    const body = options.body || null;
    
    return new Promise((resolve, reject) => {
        const xhr = new XMLHttpRequest();
        xhr.open(method, `${API_BASE_URL}${endpoint}`, true);
        
        Object.entries(headers).forEach(([k, v]) => {
            if (v != null) xhr.setRequestHeader(k, v);
        });
        
        xhr.onload = function () {
            if (xhr.status === 401) {
                logout();
                resolve(null);
                return;
            }
            try {
                const json = xhr.responseText ? JSON.parse(xhr.responseText) : null;
                resolve(json);
            } catch (err) {
                reject(err);
            }
        };
        
        xhr.onerror = function () {
            reject(new Error('Network error'));
        };
        
        xhr.ontimeout = function () {
            reject(new Error('Request timeout'));
        };
        
        xhr.timeout = 30000; // 30 second timeout
        
        if (body) {
            xhr.send(body);
        } else {
            xhr.send();
        }
    });
}

function showLoading(section) {
    const loading = document.getElementById(`${section}Loading`);
    const table = document.getElementById(`${section}Table`);
    if (loading) loading.style.display = 'block';
    if (table) table.style.display = 'none';
}

function hideLoading(section) {
    const loading = document.getElementById(`${section}Loading`);
    const table = document.getElementById(`${section}Table`);
    if (loading) loading.style.display = 'none';
    if (table) table.style.display = 'table';
}

function showAlert(section, message, type) {
    const alertContainer = document.getElementById(`${section}Alert`);
    if (!alertContainer) return;
    alertContainer.innerHTML = `
        <div class="alert alert-${type}">
            ${message}
        </div>
    `;
    setTimeout(() => {
        alertContainer.innerHTML = '';
    }, 5000);
}

function filterTable(tableId, searchValue) {
    const table = document.getElementById(tableId);
    if (!table) return;
    const rows = table.querySelectorAll('tbody tr');
    rows.forEach(row => {
        const text = row.textContent.toLowerCase();
        if (text.includes((searchValue || '').toLowerCase())) {
            row.style.display = '';
        } else {
            row.style.display = 'none';
        }
    });
}

// Fix modal click handling
window.addEventListener('click', function (event) {
    if (event.target && event.target.classList && event.target.classList.contains('modal')) {
        event.target.style.display = 'none';
    }
});

function closeModal(modalId) {
    const modal = document.getElementById(modalId);
    if (modal) modal.style.display = 'none';
}

function populateSelectOptions() {
    const patientSelect = document.getElementById('visitPatient');
    if (patientSelect && currentData.patients) {
        patientSelect.innerHTML = '<option value="">Select Patient</option>' +
            currentData.patients.map(p => `<option value="${p.patientId}">${p.patientName}</option>`).join('');
    }

    const doctorSelect = document.getElementById('visitDoctor');
    if (doctorSelect && currentData.doctors) {
        doctorSelect.innerHTML = '<option value="">Select Doctor (Optional)</option>' +
            currentData.doctors.map(d => `<option value="${d.doctorId}">${d.doctorName} - ${d.specialization || 'General'}</option>`).join('');
    }

    const visitTypeSelect = document.getElementById('visitType');
    if (visitTypeSelect && currentData.visitTypes) {
        visitTypeSelect.innerHTML = '<option value="">Select Visit Type</option>' +
            currentData.visitTypes.map(vt => `<option value="${vt.visitTypeId}">${vt.visitTypeName} - ${vt.baseRate.toFixed(2)}</option>`).join('');
    }
}

function wireVisitTypeSelectFeeAutofill() {
    const visitTypeSelect = document.getElementById('visitType');
    const visitFee = document.getElementById('visitFee');
    if (visitTypeSelect && visitFee) {
        visitTypeSelect.addEventListener('change', function () {
            const visitTypeId = this.value;
            if (visitTypeId) {
                const visitType = currentData.visitTypes.find(vt => vt.visitTypeId == visitTypeId);
                if (visitType) {
                    visitFee.value = visitType.baseRate.toFixed(2);
                }
            }
        });
    }
}

// FIXED Toast Function
function showToast(message, type = 'error') {
    // Remove existing toast
    const existingToast = document.querySelector('.toast');
    if (existingToast) {
        existingToast.remove();
    }

    // Create toast element
    const toast = document.createElement('div');
    toast.className = `toast toast-${type}`;
    toast.innerHTML = `
        <span class="toast-message">${message}</span>
        <button class="toast-close" onclick="this.parentElement.remove()">×</button>
    `;

    // Add toast styles if not already in CSS
    if (!document.querySelector('#toastStyles')) {
        const style = document.createElement('style');
        style.id = 'toastStyles';
        style.textContent = `
            .toast {
                position: fixed;
                top: 20px;
                right: 20px;
                padding: 12px 20px;
                border-radius: 4px;
                color: white;
                font-weight: 500;
                z-index: 9999;
                max-width: 400px;
                opacity: 0;
                transform: translateX(100%);
                transition: all 0.3s ease-in-out;
                display: flex;
                align-items: center;
                justify-content: space-between;
                gap: 10px;
                box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
            }
            .toast.show {
                opacity: 1;
                transform: translateX(0);
            }
            .toast-success {
                background-color: #10b981;
            }
            .toast-error {
                background-color: #ef4444;
            }
            .toast-warning {
                background-color: #f59e0b;
            }
            .toast-info {
                background-color: #3b82f6;
            }
            .toast-close {
                background: none;
                border: none;
                color: white;
                font-size: 18px;
                cursor: pointer;
                padding: 0;
                margin: 0;
                line-height: 1;
            }
            .toast-close:hover {
                opacity: 0.8;
            }
            .toast-message {
                flex: 1;
            }
        `;
        document.head.appendChild(style);
    }

    document.body.appendChild(toast);
     
    setTimeout(() => toast.classList.add('show'), 100);
     
    setTimeout(() => {
        if (toast && toast.parentElement) {
            toast.classList.remove('show');
            setTimeout(() => {
                if (toast && toast.parentElement) {
                    toast.remove();
                }
            }, 300);
        }
    }, 4000);
}
 
function validateName(value) {
    if (!value || !value.trim()) return { valid: false, message: 'Name is required' };
    const trimmed = value.trim();
    if (trimmed.length < 2) return { valid: false, message: 'Name must be at least 2 characters' };
    if (trimmed.length > 100) return { valid: false, message: 'Name cannot exceed 100 characters' };
    if (!/^[a-zA-Z\s'-]+$/.test(trimmed)) return { valid: false, message: 'Name can only contain letters, spaces, hyphens and apostrophes' };
    return { valid: true };
}

function validateEmail(value) {
    if (!value || !value.trim()) return { valid: true };
    const trimmed = value.trim();
    if (trimmed.length > 100) return { valid: false, message: 'Email cannot exceed 100 characters' };
    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(trimmed)) return { valid: false, message: 'Please enter a valid email address' };
    return { valid: true };
}

function validatePhone(value) {
    if (!value || !value.trim()) return { valid: true };
    const trimmed = value.trim();
    if (!/^\+?[\d\s\-\(\)]{10,15}$/.test(trimmed)) return { valid: false, message: 'Phone number must be 10-15 digits' };
    return { valid: true };
}

function validateDate(value, fieldName = 'Date') {
    if (!value) return { valid: true };
    const date = new Date(value);
    const today = new Date();
    today.setHours(23, 59, 59, 999);
    if (isNaN(date.getTime())) return { valid: false, message: `Invalid ${fieldName.toLowerCase()}` };
    if (date > today) return { valid: false, message: `${fieldName} cannot be in the future` };
    return { valid: true };
}

function validateDateTime(value) {
    if (!value || !value.trim()) return { valid: false, message: 'Visit date and time is required' };
    const date = new Date(value);
    if (isNaN(date.getTime())) return { valid: false, message: 'Invalid date and time' };
    return { valid: true };
}

function validateFee(value) {
    if (!value || value.trim() === '') return { valid: false, message: 'Fee is required' };
    const fee = parseFloat(value);
    if (isNaN(fee)) return { valid: false, message: 'Fee must be a number' };
    if (fee < 0) return { valid: false, message: 'Fee cannot be negative' };
    if (fee > 999999) return { valid: false, message: 'Fee cannot exceed 999999' };
    return { valid: true };
}

function validateDuration(value) {
    if (!value || value.trim() === '') return { valid: false, message: 'Duration is required' };
    const duration = parseInt(value);
    if (isNaN(duration)) return { valid: false, message: 'Duration must be a number' };
    if (duration <= 0) return { valid: false, message: 'Duration must be greater than 0' };
    if (duration > 1440) return { valid: false, message: 'Duration cannot exceed 1440 minutes' };
    return { valid: true };
}

function validateGender(value) {
    if (!value) return { valid: true };
    if (!['Male', 'Female', 'Other'].includes(value)) return { valid: false, message: 'Please select a valid gender' };
    return { valid: true };
}

function validateSpecialization(value) {
    if (!value || !value.trim()) return { valid: true };
    const trimmed = value.trim();
    if (trimmed.length > 100) return { valid: false, message: 'Specialization cannot exceed 100 characters' };
    if (!/^[a-zA-Z\s&,-]+$/.test(trimmed)) return { valid: false, message: 'Specialization contains invalid characters' };
    return { valid: true };
}

function validateAddress(value) {
    if (!value || !value.trim()) return { valid: true };
    if (value.trim().length > 255) return { valid: false, message: 'Address cannot exceed 255 characters' };
    return { valid: true };
}

function validateDescription(value) {
    if (!value || !value.trim()) return { valid: true };
    if (value.trim().length > 500) return { valid: false, message: 'Description cannot exceed 500 characters' };
    return { valid: true };
}

function validateNote(value) {
    if (!value || !value.trim()) return { valid: true };
    if (value.trim().length > 1000) return { valid: false, message: 'Note cannot exceed 1000 characters' };
    return { valid: true };
}

function validateVisitTypeName(value) {
    if (!value || !value.trim()) return { valid: false, message: 'Visit type name is required' };
    const trimmed = value.trim();
    if (trimmed.length < 2) return { valid: false, message: 'Visit type name must be at least 2 characters' };
    if (trimmed.length > 100) return { valid: false, message: 'Visit type name cannot exceed 100 characters' };
    if (!/^[a-zA-Z\s\-&]+$/.test(trimmed)) return { valid: false, message: 'Visit type name contains invalid characters' };
    return { valid: true };
}

function validateBaseRate(value) {
    if (!value || value.trim() === '') return { valid: false, message: 'Base rate is required' };
    const rate = parseFloat(value);
    if (isNaN(rate)) return { valid: false, message: 'Base rate must be a number' };
    if (rate < 0) return { valid: false, message: 'Base rate cannot be negative' };
    if (rate > 999999) return { valid: false, message: 'Base rate cannot exceed 999999' };
    return { valid: true };
}

function validateSelect(value, fieldName) {
    if (!value) return { valid: false, message: `${fieldName} is required` };
    return { valid: true };
}

// FIXED Field Validation
function addFieldValidation(fieldId, validator, realTime = true) {
    const field = document.getElementById(fieldId);
    if (!field) return null;

    function validate() {
        const result = validator(field.value);
        if (!result.valid) {
            field.classList.add('error');
            // Optionally show field-specific error
            return false;
        } else {
            field.classList.remove('error');
            return true;
        }
    }

    if (realTime) {
        field.addEventListener('blur', validate);
        field.addEventListener('input', () => {
            if (field.classList.contains('error')) {
                validate();
            }
        });
    }

    return validate;
}

// FIXED Input Restrictions
function restrictNumericInput(fieldId) {
    const field = document.getElementById(fieldId);
    if (!field) return;

    field.addEventListener('keypress', function (e) {
        // Allow control keys (backspace, delete, arrow keys, etc.)
        if (e.ctrlKey || e.altKey || e.metaKey) return;
        if (['Backspace', 'Delete', 'ArrowLeft', 'ArrowRight', 'Tab'].includes(e.key)) return;
        
        if (/[0-9]/.test(e.key)) {
            e.preventDefault();
            showToast('Numbers are not allowed in this field', 'warning');
        }
    });

    field.addEventListener('paste', function (e) {
        setTimeout(() => {
            const originalValue = this.value;
            const cleanValue = originalValue.replace(/[0-9]/g, '');
            if (cleanValue !== originalValue) {
                this.value = cleanValue;
                showToast('Numbers were removed from pasted text', 'warning');
            }
        }, 0);
    });
}

function restrictToNumbers(fieldId, allowDecimal = false) {
    const field = document.getElementById(fieldId);
    if (!field) return;

    field.addEventListener('keypress', function (e) {
        // Allow control keys
        if (e.ctrlKey || e.altKey || e.metaKey) return;
        if (['Backspace', 'Delete', 'ArrowLeft', 'ArrowRight', 'Tab'].includes(e.key)) return;
        
        const char = e.key;
        const isNumber = /[0-9]/.test(char);
        const isDecimal = char === '.' && allowDecimal && !this.value.includes('.');

        if (!isNumber && !isDecimal) {
            e.preventDefault();
        }
    });
}
 
function validatePatientForm() {
    let isValid = true;
    const errors = [];
 
    const name = document.getElementById('patientName')?.value || '';
    const dob = document.getElementById('patientDob')?.value || '';
    const gender = document.getElementById('patientGender')?.value || '';
    const contact = document.getElementById('patientContact')?.value || '';
    const email = document.getElementById('patientEmail')?.value || '';
    const address = document.getElementById('patientAddress')?.value || '';
    const emergency = document.getElementById('patientEmergencyContact')?.value || '';
 
    const nameResult = validateName(name);
    if (!nameResult.valid) {
        errors.push(nameResult.message);
        isValid = false;
    }

    const dobResult = validateDate(dob, 'Date of Birth');
    if (!dobResult.valid) {
        errors.push(dobResult.message);
        isValid = false;
    }

    const genderResult = validateGender(gender);
    if (!genderResult.valid) {
        errors.push(genderResult.message);
        isValid = false;
    }

    const contactResult = validatePhone(contact);
    if (!contactResult.valid) {
        errors.push(contactResult.message);
        isValid = false;
    }

    const emailResult = validateEmail(email);
    if (!emailResult.valid) {
        errors.push(emailResult.message);
        isValid = false;
    }

    const addressResult = validateAddress(address);
    if (!addressResult.valid) {
        errors.push(addressResult.message);
        isValid = false;
    }

    const emergencyResult = validatePhone(emergency);
    if (!emergencyResult.valid) {
        errors.push('Emergency contact: ' + emergencyResult.message);
        isValid = false;
    }

    if (!isValid && errors.length > 0) {
        showToast(errors[0], 'error');
    }

    return isValid;
}

function validateDoctorForm() {
    let isValid = true;
    const errors = [];

    const name = document.getElementById('doctorName')?.value || '';
    const specialization = document.getElementById('doctorSpecialization')?.value || '';
    const contact = document.getElementById('doctorContact')?.value || '';
    const email = document.getElementById('doctorEmail')?.value || '';

    const nameResult = validateName(name);
    if (!nameResult.valid) {
        errors.push(nameResult.message);
        isValid = false;
    }

    const specializationResult = validateSpecialization(specialization);
    if (!specializationResult.valid) {
        errors.push(specializationResult.message);
        isValid = false;
    }

    const contactResult = validatePhone(contact);
    if (!contactResult.valid) {
        errors.push(contactResult.message);
        isValid = false;
    }

    const emailResult = validateEmail(email);
    if (!emailResult.valid) {
        errors.push(emailResult.message);
        isValid = false;
    }

    if (!isValid && errors.length > 0) {
        showToast(errors[0], 'error');
    }

    return isValid;
}

function validateVisitForm() {
    let isValid = true;
    const errors = [];

    const patient = document.getElementById('visitPatient')?.value || '';
    const visitType = document.getElementById('visitType')?.value || '';
    const visitDate = document.getElementById('visitDate')?.value || '';
    const duration = document.getElementById('visitDuration')?.value || '';
    const fee = document.getElementById('visitFee')?.value || '';
    const note = document.getElementById('visitNote')?.value || '';

    const patientResult = validateSelect(patient, 'Patient');
    if (!patientResult.valid) {
        errors.push(patientResult.message);
        isValid = false;
    }

    const visitTypeResult = validateSelect(visitType, 'Visit Type');
    if (!visitTypeResult.valid) {
        errors.push(visitTypeResult.message);
        isValid = false;
    }

    const dateResult = validateDateTime(visitDate);
    if (!dateResult.valid) {
        errors.push(dateResult.message);
        isValid = false;
    }

    const durationResult = validateDuration(duration);
    if (!durationResult.valid) {
        errors.push(durationResult.message);
        isValid = false;
    }

    const feeResult = validateFee(fee);
    if (!feeResult.valid) {
        errors.push(feeResult.message);
        isValid = false;
    }

    const noteResult = validateNote(note);
    if (!noteResult.valid) {
        errors.push(noteResult.message);
        isValid = false;
    }

    if (!isValid && errors.length > 0) {
        showToast(errors[0], 'error');
    }

    return isValid;
}

function validateVisitTypeForm() {
    let isValid = true;
    const errors = [];

    const name = document.getElementById('visitTypeName')?.value || '';
    const rate = document.getElementById('visitTypeRate')?.value || '';
    const description = document.getElementById('visitTypeDescription')?.value || '';

    const nameResult = validateVisitTypeName(name);
    if (!nameResult.valid) {
        errors.push(nameResult.message);
        isValid = false;
    }

    const rateResult = validateBaseRate(rate);
    if (!rateResult.valid) {
        errors.push(rateResult.message);
        isValid = false;
    }

    const descResult = validateDescription(description);
    if (!descResult.valid) {
        errors.push(descResult.message);
        isValid = false;
    }

    if (!isValid && errors.length > 0) {
        showToast(errors[0], 'error');
    }

    return isValid;
}

// FIXED Initialization
function initializeValidation() {
    // Apply restrictions
    restrictNumericInput('patientName');
    restrictNumericInput('doctorName');
    restrictNumericInput('doctorSpecialization');
    restrictNumericInput('visitTypeName');

    restrictToNumbers('visitDuration');
    restrictToNumbers('visitFee', true);
    restrictToNumbers('visitTypeRate', true);

    // Add field validations
    addFieldValidation('patientName', validateName);
    addFieldValidation('patientEmail', validateEmail);
    addFieldValidation('patientContact', validatePhone);
    addFieldValidation('patientEmergencyContact', validatePhone);
    addFieldValidation('patientDob', (value) => validateDate(value, 'Date of Birth'));
    addFieldValidation('patientAddress', validateAddress);

    addFieldValidation('doctorName', validateName);
    addFieldValidation('doctorEmail', validateEmail);
    addFieldValidation('doctorContact', validatePhone);
    addFieldValidation('doctorSpecialization', validateSpecialization);

    addFieldValidation('visitFee', validateFee);
    addFieldValidation('visitDuration', validateDuration);
    addFieldValidation('visitNote', validateNote);

    addFieldValidation('visitTypeName', validateVisitTypeName);
    addFieldValidation('visitTypeRate', validateBaseRate);
    addFieldValidation('visitTypeDescription', validateDescription);
}

// Patients Functions
async function loadPatients() {
    showLoading('patients');
    try {
        const response = await makeApiRequest('/patient');
        if (response && response.success) {
            currentData.patients = response.data || [];
            renderPatientsTable();
            populateSelectOptions();
        } else {
            showAlert('patients', 'Failed to load patients', 'error');
        }
    } catch (error) {
        console.error('Error loading patients:', error);
        showAlert('patients', 'Error loading patients', 'error');
    } finally {
        hideLoading('patients');
    }
}

function renderPatientsTable() {
    const tbody = document.querySelector('#patientsTable tbody');
    if (!tbody) return;
    const userRole = localStorage.getItem('userRole');
    tbody.innerHTML = currentData.patients.map(patient => `
        <tr>
            <td>${patient.patientId}</td>
            <td>${patient.patientName}</td>
            <td>${patient.dateOfBirth ? new Date(patient.dateOfBirth).toLocaleDateString() : 'N/A'}</td>
            <td>${patient.gender || 'N/A'}</td>
            <td>${patient.contactNumber || 'N/A'}</td>
            <td>${patient.email || 'N/A'}</td>
            <td class="table-actions">
                <button class="btn btn-small" onclick="editPatient(${patient.patientId})">Edit</button>
                ${userRole === 'Admin' ? `<button class="btn btn-danger btn-small" onclick="deletePatient(${patient.patientId})">Delete</button>` : ''}
            </td>
        </tr>
    `).join('');
}

function showAddPatientModal() {
    const title = document.getElementById('patientModalTitle');
    const form = document.getElementById('patientForm');
    const id = document.getElementById('patientId');
    if (!form || !title || !id) return;
    title.textContent = 'Add Patient';
    form.reset();
    id.value = '';
    document.getElementById('patientModal').style.display = 'block';
}

function editPatient(id) {
    const patient = currentData.patients.find(p => p.patientId === id);
    if (patient) {
        document.getElementById('patientModalTitle').textContent = 'Edit Patient';
        document.getElementById('patientId').value = patient.patientId;
        document.getElementById('patientName').value = patient.patientName || '';
        document.getElementById('patientDob').value = patient.dateOfBirth ? patient.dateOfBirth.split('T')[0] : '';
        document.getElementById('patientGender').value = patient.gender || '';
        document.getElementById('patientContact').value = patient.contactNumber || '';
        document.getElementById('patientEmail').value = patient.email || '';
        document.getElementById('patientAddress').value = patient.address || '';
        document.getElementById('patientEmergencyContact').value = patient.emergencyContact || '';
        document.getElementById('patientModal').style.display = 'block';
    }
}

async function deletePatient(id) {
    if (confirm('Are you sure you want to delete this patient?')) {
        try {
            const response = await makeApiRequest(`/patient/${id}`, { method: 'DELETE' });
            if (response && response.success) {
                showToast('Patient deleted successfully', 'success');
                loadPatients();
            } else {
                showToast((response && response.message) || 'Failed to delete patient', 'error');
            }
        } catch (error) {
            console.error('Error deleting patient:', error);
            showToast('Error deleting patient', 'error');
        }
    }
}

function wirePatientForm() {
    const form = document.getElementById('patientForm');
    if (!form) return;

    form.addEventListener('submit', async function (e) {
        e.preventDefault();

        if (!validatePatientForm()) {
            return;
        }

        const patientId = document.getElementById('patientId').value;
        const isEdit = !!patientId;
        const patientData = {
            patientName: document.getElementById('patientName').value.trim(),
            dateOfBirth: document.getElementById('patientDob').value || null,
            gender: document.getElementById('patientGender').value || null,
            contactNumber: document.getElementById('patientContact').value.trim() || null,
            email: document.getElementById('patientEmail').value.trim() || null,
            address: document.getElementById('patientAddress').value.trim() || null,
            emergencyContact: document.getElementById('patientEmergencyContact').value.trim() || null
        };

        if (isEdit) {
            patientData.patientId = parseInt(patientId);
        }

        try {
            const endpoint = isEdit ? `/patient/${patientId}` : '/patient';
            const method = isEdit ? 'PUT' : 'POST';
            const response = await makeApiRequest(endpoint, {
                method,
                body: JSON.stringify(patientData)
            });

            if (response && response.success) {
                showToast(`Patient ${isEdit ? 'updated' : 'created'} successfully`, 'success');
                closeModal('patientModal');
                loadPatients();
            } else {
                showToast((response && response.message) || `Failed to ${isEdit ? 'update' : 'create'} patient`, 'error');
            }
        } catch (error) {
            console.error(`Error ${isEdit ? 'updating' : 'creating'} patient:`, error);
            showToast(`Error ${isEdit ? 'updating' : 'creating'} patient`, 'error');
        }
    });
}

// Doctors Functions
async function loadDoctors() {
    showLoading('doctors');
    try {
        const response = await makeApiRequest('/doctor');
        if (response && response.success) {
            currentData.doctors = response.data || [];
            renderDoctorsTable();
            populateSelectOptions();
        } else {
            showAlert('doctors', 'Failed to load doctors', 'error');
        }
    } catch (error) {
        console.error('Error loading doctors:', error);
        showAlert('doctors', 'Error loading doctors', 'error');
    } finally {
        hideLoading('doctors');
    }
}

function renderDoctorsTable() {
    const tbody = document.querySelector('#doctorsTable tbody');
    if (!tbody) return;
    const userRole = localStorage.getItem('userRole');
    tbody.innerHTML = currentData.doctors.map(doctor => `
        <tr>
            <td>${doctor.doctorId}</td>
            <td>${doctor.doctorName}</td>
            <td>${doctor.specialization || 'N/A'}</td>
            <td>${doctor.contactNumber || 'N/A'}</td>
            <td>${doctor.email || 'N/A'}</td>
            <td class="table-actions">
                <button class="btn btn-small" onclick="editDoctor(${doctor.doctorId})">Edit</button>
                ${userRole === 'Admin' ? `<button class="btn btn-danger btn-small" onclick="deleteDoctor(${doctor.doctorId})">Delete</button>` : ''}
            </td>
        </tr>
    `).join('');
}

function showAddDoctorModal() {
    const title = document.getElementById('doctorModalTitle');
    const form = document.getElementById('doctorForm');
    const id = document.getElementById('doctorId');
    if (!form || !title || !id) return;
    title.textContent = 'Add Doctor';
    form.reset();
    id.value = '';
    document.getElementById('doctorModal').style.display = 'block';
}

function editDoctor(id) {
    const doctor = currentData.doctors.find(d => d.doctorId === id);
    if (doctor) {
        document.getElementById('doctorModalTitle').textContent = 'Edit Doctor';
        document.getElementById('doctorId').value = doctor.doctorId;
        document.getElementById('doctorName').value = doctor.doctorName || '';
        document.getElementById('doctorSpecialization').value = doctor.specialization || '';
        document.getElementById('doctorContact').value = doctor.contactNumber || '';
        document.getElementById('doctorEmail').value = doctor.email || '';
        document.getElementById('doctorModal').style.display = 'block';
    }
}

async function deleteDoctor(id) {
    if (confirm('Are you sure you want to delete this doctor?')) {
        try {
            const response = await makeApiRequest(`/doctor/${id}`, { method: 'DELETE' });
            if (response && response.success) {
                showToast('Doctor deleted successfully', 'success');
                loadDoctors();
            } else {
                showToast((response && response.message) || 'Failed to delete doctor', 'error');
            }
        } catch (error) {
            console.error('Error deleting doctor:', error);
            showToast('Error deleting doctor', 'error');
        }
    }
}

function wireDoctorForm() {
    const form = document.getElementById('doctorForm');
    if (!form) return;

    form.addEventListener('submit', async function (e) {
        e.preventDefault();

        if (!validateDoctorForm()) {
            return;
        }

        const doctorId = document.getElementById('doctorId').value;
        const isEdit = !!doctorId;
        const doctorData = {
            doctorName: document.getElementById('doctorName').value.trim(),
            specialization: document.getElementById('doctorSpecialization').value.trim() || null,
            contactNumber: document.getElementById('doctorContact').value.trim() || null,
            email: document.getElementById('doctorEmail').value.trim() || null
        };

        if (isEdit) {
            doctorData.doctorId = parseInt(doctorId);
        }

        try {
            const endpoint = isEdit ? `/doctor/${doctorId}` : '/doctor';
            const method = isEdit ? 'PUT' : 'POST';
            const response = await makeApiRequest(endpoint, {
                method,
                body: JSON.stringify(doctorData)
            });

            if (response && response.success) {
                showToast(`Doctor ${isEdit ? 'updated' : 'created'} successfully`, 'success');
                closeModal('doctorModal');
                loadDoctors();
            } else {
                showToast((response && response.message) || `Failed to ${isEdit ? 'update' : 'create'} doctor`, 'error');
            }
        } catch (error) {
            console.error(`Error ${isEdit ? 'updating' : 'creating'} doctor:`, error);
            showToast(`Error ${isEdit ? 'updating' : 'creating'} doctor`, 'error');
        }
    });
}

// Visit Types Functions
async function loadVisitTypes() {
    showLoading('visitTypes');
    try {
        const response = await makeApiRequest('/visittype');
        if (response && response.success) {
            currentData.visitTypes = response.data || [];
            renderVisitTypesTable();
            populateSelectOptions();
        } else {
            showAlert('visitTypes', 'Failed to load visit types', 'error');
        }
    } catch (error) {
        console.error('Error loading visit types:', error);
        showAlert('visitTypes', 'Error loading visit types', 'error');
    } finally {
        hideLoading('visitTypes');
    }
}

function renderVisitTypesTable() {
    const tbody = document.querySelector('#visitTypesTable tbody');
    if (!tbody) return;
    const userRole = localStorage.getItem('userRole');
    tbody.innerHTML = currentData.visitTypes.map(visitType => `
        <tr>
            <td>${visitType.visitTypeId}</td>
            <td>${visitType.visitTypeName}</td>
            <td>${visitType.baseRate.toFixed(2)}</td>
            <td>${visitType.description || 'N/A'}</td>
            <td class="admin-only table-actions">
                <button class="btn btn-danger btn-small" onclick="deleteVisitType(${visitType.visitTypeId})">Delete</button>
            </td>
        </tr>
    `).join('');
    
    if (userRole === 'Admin') {
        document.querySelectorAll('#visitTypesTable .admin-only').forEach(el => {
            el.style.display = 'table-cell';
        });
    }
}

function showAddVisitTypeModal() {
    const form = document.getElementById('visitTypeForm');
    if (!form) return;
    form.reset();
    document.getElementById('visitTypeModal').style.display = 'block';
}

async function deleteVisitType(id) {
    if (confirm('Are you sure you want to delete this visit type?')) {
        try {
            const response = await makeApiRequest(`/visittype/${id}`, { method: 'DELETE' });
            if (response && response.success) {
                showToast('Visit type deleted successfully', 'success');
                loadVisitTypes();
            } else {
                showToast((response && response.message) || 'Failed to delete visit type', 'error');
            }
        } catch (error) {
            console.error('Error deleting visit type:', error);
            showToast('Error deleting visit type', 'error');
        }
    }
}

function wireVisitTypeForm() {
    const form = document.getElementById('visitTypeForm');
    if (!form) return;

    form.addEventListener('submit', async function (e) {
        e.preventDefault();

        if (!validateVisitTypeForm()) {
            return;
        }

        const visitTypeData = {
            visitTypeName: document.getElementById('visitTypeName').value.trim(),
            baseRate: parseFloat(document.getElementById('visitTypeRate').value),
            description: document.getElementById('visitTypeDescription').value.trim() || null
        };

        try {
            const response = await makeApiRequest('/visittype', {
                method: 'POST',
                body: JSON.stringify(visitTypeData)
            });

            if (response && response.success) {
                showToast('Visit type created successfully', 'success');
                closeModal('visitTypeModal');
                loadVisitTypes();
            } else {
                showToast((response && response.message) || 'Failed to create visit type', 'error');
            }
        } catch (error) {
            console.error('Error creating visit type:', error);
            showToast('Error creating visit type', 'error');
        }
    });
}

// Visits Functions
async function loadVisits() {
    showLoading('visits');
    try {
        const response = await makeApiRequest('/patientvisit');
        if (response && response.success) {
            currentData.visits = response.data || [];
            renderVisitsTable();
        } else {
            showAlert('visits', 'Failed to load visits', 'error');
        }
    } catch (error) {
        console.error('Error loading visits:', error);
        showAlert('visits', 'Error loading visits', 'error');
    } finally {
        hideLoading('visits');
    }
}

function renderVisitsTable() {
    const tbody = document.querySelector('#visitsTable tbody');
    if (!tbody) return;
    const userRole = localStorage.getItem('userRole');
    tbody.innerHTML = currentData.visits.map(visit => {
        const patient = currentData.patients.find(p => p.patientId === visit.patientId);
        const doctor = currentData.doctors.find(d => d.doctorId === visit.doctorId);
        const visitType = currentData.visitTypes.find(vt => vt.visitTypeId === visit.visitTypeId);
        return `
            <tr>
                <td>${visit.id}</td>
                <td>${patient ? patient.patientName : 'Unknown'}</td>
                <td>${doctor ? doctor.doctorName : 'N/A'}</td>
                <td>${visitType ? visitType.visitTypeName : 'Unknown'}</td>
                <td>${new Date(visit.visitDate).toLocaleString()}</td>
                <td>${visit.durationInMinutes} mins</td>
                <td>${Number(visit.fee).toFixed(2)}</td>
                <td class="table-actions">
                    <button class="btn btn-small" onclick="editVisit(${visit.id})">Edit</button>
                    ${userRole === 'Admin' ? `<button class="btn btn-danger btn-small" onclick="deleteVisit(${visit.id})">Delete</button>` : ''}
                </td>
            </tr>
        `;
    }).join('');
}

async function showAddVisitModal() {
    await loadPatients();
    await loadDoctors();
    await loadVisitTypes();
    populateSelectOptions();
    const form = document.getElementById('visitForm');
    if (!form) return;
    document.getElementById('visitModalTitle').textContent = 'Add Visit';
    form.reset();
    document.getElementById('visitId').value = '';
    document.getElementById('visitDuration').value = '30';
    document.getElementById('visitModal').style.display = 'block';
}

function editVisit(id) {
    const visit = currentData.visits.find(v => v.id === id);
    if (visit) {
        populateSelectOptions();
        document.getElementById('visitModalTitle').textContent = 'Edit Visit';
        document.getElementById('visitId').value = visit.id;
        document.getElementById('visitPatient').value = visit.patientId;
        document.getElementById('visitDoctor').value = visit.doctorId || '';
        document.getElementById('visitType').value = visit.visitTypeId;
        document.getElementById('visitDate').value = String(visit.visitDate).slice(0, 16);
        document.getElementById('visitDuration').value = visit.durationInMinutes;
        document.getElementById('visitFee').value = visit.fee;
        document.getElementById('visitNote').value = visit.note || '';
        document.getElementById('visitModal').style.display = 'block';
    }
}

async function deleteVisit(id) {
    if (confirm('Are you sure you want to delete this visit?')) {
        try {
            const response = await makeApiRequest(`/patientvisit/${id}`, { method: 'DELETE' });
            if (response && response.success) {
                showToast('Visit deleted successfully', 'success');
                loadVisits();
            } else {
                showToast((response && response.message) || 'Failed to delete visit', 'error');
            }
        } catch (error) {
            console.error('Error deleting visit:', error);
            showToast('Error deleting visit', 'error');
        }
    }
}

function wireVisitForm() {
    const form = document.getElementById('visitForm');
    if (!form) return;

    form.addEventListener('submit', async function (e) {
        e.preventDefault();

        if (!validateVisitForm()) {
            return;
        }

        const visitId = document.getElementById('visitId').value;
        const isEdit = !!visitId;
        const visitData = {
            patientId: parseInt(document.getElementById('visitPatient').value),
            doctorId: document.getElementById('visitDoctor').value ? parseInt(document.getElementById('visitDoctor').value) : null,
            visitTypeId: parseInt(document.getElementById('visitType').value),
            visitDate: document.getElementById('visitDate').value,
            durationInMinutes: parseInt(document.getElementById('visitDuration').value),
            fee: parseFloat(document.getElementById('visitFee').value),
            note: document.getElementById('visitNote').value.trim() || null
        };

        if (isEdit) {
            visitData.id = parseInt(visitId);
        }

        try {
            const endpoint = isEdit ? `/patientvisit/${visitId}` : '/patientvisit';
            const method = isEdit ? 'PUT' : 'POST';
            const response = await makeApiRequest(endpoint, {
                method,
                body: JSON.stringify(visitData)
            });

            if (response && response.success) {
                showToast(`Visit ${isEdit ? 'updated' : 'created'} successfully`, 'success');
                closeModal('visitModal');
                loadVisits();
            } else {
                showToast((response && response.message) || `Failed to ${isEdit ? 'update' : 'create'} visit`, 'error');
            }
        } catch (error) {
            console.error(`Error ${isEdit ? 'updating' : 'creating'} visit:`, error);
            showToast(`Error ${isEdit ? 'updating' : 'creating'} visit`, 'error');
        }
    });
}