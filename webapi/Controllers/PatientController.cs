using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webapi.Models;
using webapi.Models;
using webapi.Repositories;

namespace webapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PatientController : ControllerBase
    {
        private readonly IPatientRepository _patientRepository;

        public PatientController(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPatients()
        {
            try
            {
                var patients = await _patientRepository.GetAllAsync();
                return Ok(ApiResponse<IEnumerable<Patient>>.SuccessResult(patients, "Patients retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResult("Error retrieving patients."));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPatientById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(ApiResponse<object>.ErrorResult("Invalid patient ID."));
                }

                var patient = await _patientRepository.GetByIdAsync(id);
                
                if (patient == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResult("Patient not found."));
                }

                return Ok(ApiResponse<Patient>.SuccessResult(patient, "Patient retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResult("Error retrieving patient."));
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePatient([FromBody] CreatePatientDto createPatientDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<object>.ErrorResult("Invalid input data."));
                }

                var patient = new Patient
                {
                    PatientName = createPatientDto.PatientName?.Trim(),
                    DateOfBirth = createPatientDto.DateOfBirth,
                    Gender = createPatientDto.Gender?.Trim(),
                    ContactNumber = createPatientDto.ContactNumber?.Trim(),
                    Email = createPatientDto.Email?.Trim(),
                    Address = createPatientDto.Address?.Trim(),
                    EmergencyContact = createPatientDto.EmergencyContact?.Trim()
                };

                var patientId = await _patientRepository.AddAsync(patient);
                
                if (patientId > 0)
                {
                    var createdPatient = await _patientRepository.GetByIdAsync(patientId);
                    return CreatedAtAction(nameof(GetPatientById), new { id = patientId }, 
                        ApiResponse<Patient>.SuccessResult(createdPatient, "Patient created successfully."));
                }

                return BadRequest(ApiResponse<object>.ErrorResult("Failed to create patient."));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResult("Error creating patient."));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatient(int id, [FromBody] UpdatePatientDto updatePatientDto)
        {
            try
            {
                if (id <= 0 || updatePatientDto.PatientId != id)
                {
                    return BadRequest(ApiResponse<object>.ErrorResult("Invalid patient ID."));
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<object>.ErrorResult("Invalid input data."));
                }

                var existingPatient = await _patientRepository.GetByIdAsync(id);
                if (existingPatient == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResult("Patient not found."));
                }

                var patient = new Patient
                {
                    PatientId = updatePatientDto.PatientId,
                    PatientName = updatePatientDto.PatientName?.Trim(),
                    DateOfBirth = updatePatientDto.DateOfBirth,
                    Gender = updatePatientDto.Gender?.Trim(),
                    ContactNumber = updatePatientDto.ContactNumber?.Trim(),
                    Email = updatePatientDto.Email?.Trim(),
                    Address = updatePatientDto.Address?.Trim(),
                    EmergencyContact = updatePatientDto.EmergencyContact?.Trim()
                };

                var success = await _patientRepository.UpdateAsync(patient);
                
                if (success)
                {
                    var updatedPatient = await _patientRepository.GetByIdAsync(id);
                    return Ok(ApiResponse<Patient>.SuccessResult(updatedPatient, "Patient updated successfully."));
                }

                return BadRequest(ApiResponse<object>.ErrorResult("Failed to update patient."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResult("Error updating patient."));
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(ApiResponse<object>.ErrorResult("Invalid patient ID."));
                }

                var existingPatient = await _patientRepository.GetByIdAsync(id);
                if (existingPatient == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResult("Patient not found."));
                }

                var success = await _patientRepository.DeleteAsync(id);
                
                if (success)
                {
                    return Ok(ApiResponse<object>.SuccessResult(null, "Patient deleted successfully."));
                }

                return BadRequest(ApiResponse<object>.ErrorResult("Failed to delete patient."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResult("Error deleting patient."));
            }
        }
    }
}