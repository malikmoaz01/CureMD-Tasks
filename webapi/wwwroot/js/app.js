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
        window.location.href = 'login.html';
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

window.onclick = function (event) {
    if (event.target && event.target.classList && event.target.classList.contains('modal')) {
        event.target.style.display = 'none';
    }
};

function closeModal(modalId) {
    const modal = document.getElementById(modalId);
    if (modal) modal.style.display = 'none';
}

function populateSelectOptions() {
    const patientSelect = document.getElementById('visitPatient');
    if (patientSelect) {
        patientSelect.innerHTML = '<option value="">Select Patient</option>' +
            currentData.patients.map(p => `<option value="${p.patientId}">${p.patientName}</option>`).join('');
    }

    const doctorSelect = document.getElementById('visitDoctor');
    if (doctorSelect) {
        doctorSelect.innerHTML = '<option value="">Select Doctor (Optional)</option>' +
            currentData.doctors.map(d => `<option value="${d.doctorId}">${d.doctorName} - ${d.specialization || 'General'}</option>`).join('');
    }

    const visitTypeSelect = document.getElementById('visitType');
    if (visitTypeSelect) {
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

// Patients
async function loadPatients() {
    showLoading('patients');
    try {
        const response = await makeApiRequest('/patient');
        if (response && response.success) {
            currentData.patients = response.data;
            renderPatientsTable();
        } else {
            showAlert('patients', 'Failed to load patients', 'error');
        }
    } catch (error) {
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
        document.getElementById('patientName').value = patient.patientName;
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
                showAlert('patients', 'Patient deleted successfully', 'success');
                loadPatients();
            } else {
                showAlert('patients', (response && response.message) || 'Failed to delete patient', 'error');
            }
        } catch (error) {
            showAlert('patients', 'Error deleting patient', 'error');
        }
    }
}

function wirePatientForm() {
    const form = document.getElementById('patientForm');
    if (!form) return;
    form.addEventListener('submit', async function (e) {
        e.preventDefault();
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
                showAlert('patients', `Patient ${isEdit ? 'updated' : 'created'} successfully`, 'success');
                closeModal('patientModal');
                loadPatients();
            } else {
                showAlert('patients', (response && response.message) || `Failed to ${isEdit ? 'update' : 'create'} patient`, 'error');
            }
        } catch (error) {
            showAlert('patients', `Error ${isEdit ? 'updating' : 'creating'} patient`, 'error');
        }
    });
}

// Doctors
async function loadDoctors() {
    showLoading('doctors');
    try {
        const response = await makeApiRequest('/doctor');
        if (response && response.success) {
            currentData.doctors = response.data;
            renderDoctorsTable();
            populateSelectOptions();
        } else {
            showAlert('doctors', 'Failed to load doctors', 'error');
        }
    } catch (error) {
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
        document.getElementById('doctorName').value = doctor.doctorName;
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
                showAlert('doctors', 'Doctor deleted successfully', 'success');
                loadDoctors();
            } else {
                showAlert('doctors', (response && response.message) || 'Failed to delete doctor', 'error');
            }
        } catch (error) {
            showAlert('doctors', 'Error deleting doctor', 'error');
        }
    }
}

function wireDoctorForm() {
    const form = document.getElementById('doctorForm');
    if (!form) return;
    form.addEventListener('submit', async function (e) {
        e.preventDefault();
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
                showAlert('doctors', `Doctor ${isEdit ? 'updated' : 'created'} successfully`, 'success');
                closeModal('doctorModal');
                loadDoctors();
            } else {
                showAlert('doctors', (response && response.message) || `Failed to ${isEdit ? 'update' : 'create'} doctor`, 'error');
            }
        } catch (error) {
            showAlert('doctors', `Error ${isEdit ? 'updating' : 'creating'} doctor`, 'error');
        }
    });
}

// Visit Types
async function loadVisitTypes() {
    showLoading('visitTypes');
    try {
        const response = await makeApiRequest('/visittype');
        if (response && response.success) {
            currentData.visitTypes = response.data;
            renderVisitTypesTable();
            populateSelectOptions();
        } else {
            showAlert('visitTypes', 'Failed to load visit types', 'error');
        }
    } catch (error) {
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
                showAlert('visitTypes', 'Visit type deleted successfully', 'success');
                loadVisitTypes();
            } else {
                showAlert('visitTypes', (response && response.message) || 'Failed to delete visit type', 'error');
            }
        } catch (error) {
            showAlert('visitTypes', 'Error deleting visit type', 'error');
        }
    }
}

function wireVisitTypeForm() {
    const form = document.getElementById('visitTypeForm');
    if (!form) return;
    form.addEventListener('submit', async function (e) {
        e.preventDefault();
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
                showAlert('visitTypes', 'Visit type created successfully', 'success');
                closeModal('visitTypeModal');
                loadVisitTypes();
            } else {
                showAlert('visitTypes', (response && response.message) || 'Failed to create visit type', 'error');
            }
        } catch (error) {
            showAlert('visitTypes', 'Error creating visit type', 'error');
        }
    });
}

// Visits
async function loadVisits() {
    showLoading('visits');
    try {
        const response = await makeApiRequest('/patientvisit');
        if (response && response.success) {
            currentData.visits = response.data;
            renderVisitsTable();
        } else {
            showAlert('visits', 'Failed to load visits', 'error');
        }
    } catch (error) {
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
                showAlert('visits', 'Visit deleted successfully', 'success');
                loadVisits();
            } else {
                showAlert('visits', (response && response.message) || 'Failed to delete visit', 'error');
            }
        } catch (error) {
            showAlert('visits', 'Error deleting visit', 'error');
        }
    }
}

function wireVisitForm() {
    const form = document.getElementById('visitForm');
    if (!form) return;
    form.addEventListener('submit', async function (e) {
        e.preventDefault();
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
                showAlert('visits', `Visit ${isEdit ? 'updated' : 'created'} successfully`, 'success');
                closeModal('visitModal');
                loadVisits();
            } else {
                showAlert('visits', (response && response.message) || `Failed to ${isEdit ? 'update' : 'create'} visit`, 'error');
            }
        } catch (error) {
            showAlert('visits', `Error ${isEdit ? 'updating' : 'creating'} visit`, 'error');
        }
    });
}