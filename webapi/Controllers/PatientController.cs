// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using webapi.Models;
// using webapi.Models;
// using webapi.Repositories;

// namespace webapi.Controllers
// {
//     [ApiController]
//     [Route("api/[controller]")]
//     [Authorize]
//     public class PatientController : ControllerBase
//     {
//         private readonly IPatientRepository _patientRepository;

//         public PatientController(IPatientRepository patientRepository)
//         {
//             _patientRepository = patientRepository;
//         }

//         [HttpGet]
//         public async Task<IActionResult> GetAllPatients()
//         {
//             try
//             {
//                 var patients = await _patientRepository.GetAllAsync();
//                 return Ok(ApiResponse<IEnumerable<Patient>>.SuccessResult(patients, "Patients retrieved successfully."));
//             }
//             catch (Exception ex)
//             {
//                 return StatusCode(500, ApiResponse<object>.ErrorResult("Error retrieving patients."));
//             }
//         }

//         [HttpGet("{id}")]
//         public async Task<IActionResult> GetPatientById(int id)
//         {
//             try
//             {
//                 if (id <= 0)
//                 {
//                     return BadRequest(ApiResponse<object>.ErrorResult("Invalid patient ID."));
//                 }

//                 var patient = await _patientRepository.GetByIdAsync(id);
                
//                 if (patient == null)
//                 {
//                     return NotFound(ApiResponse<object>.ErrorResult("Patient not found."));
//                 }

//                 return Ok(ApiResponse<Patient>.SuccessResult(patient, "Patient retrieved successfully."));
//             }
//             catch (Exception ex)
//             {
//                 return StatusCode(500, ApiResponse<object>.ErrorResult("Error retrieving patient."));
//             }
//         }

//         [HttpPost]
//         public async Task<IActionResult> CreatePatient([FromBody] CreatePatientDto createPatientDto)
//         {
//             try
//             {
//                 if (!ModelState.IsValid)
//                 {
//                     return BadRequest(ApiResponse<object>.ErrorResult("Invalid input data."));
//                 }

//                 var patient = new Patient
//                 {
//                     PatientName = createPatientDto.PatientName?.Trim(),
//                     DateOfBirth = createPatientDto.DateOfBirth,
//                     Gender = createPatientDto.Gender?.Trim(),
//                     ContactNumber = createPatientDto.ContactNumber?.Trim(),
//                     Email = createPatientDto.Email?.Trim(),
//                     Address = createPatientDto.Address?.Trim(),
//                     EmergencyContact = createPatientDto.EmergencyContact?.Trim()
//                 };

//                 var patientId = await _patientRepository.AddAsync(patient);
                
//                 if (patientId > 0)
//                 {
//                     var createdPatient = await _patientRepository.GetByIdAsync(patientId);
//                     return CreatedAtAction(nameof(GetPatientById), new { id = patientId }, 
//                         ApiResponse<Patient>.SuccessResult(createdPatient, "Patient created successfully."));
//                 }

//                 return BadRequest(ApiResponse<object>.ErrorResult("Failed to create patient."));
//             }
//             catch (ArgumentException ex)
//             {
//                 return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
//             }
//             catch (Exception ex)
//             {
//                 return StatusCode(500, ApiResponse<object>.ErrorResult("Error creating patient."));
//             }
//         }

//          [HttpPut("{id}")]
// public async Task<IActionResult> UpdatePatient(int id, [FromBody] UpdatePatientDto updatePatientDto)
// {
//     try
//     { 
//         Console.WriteLine($"Attempting to update patient with ID: {id}");
        
//         if (id <= 0 || updatePatientDto.PatientId != id)
//         {
//             Console.WriteLine("Invalid patient ID or ID mismatch");
//             return BadRequest(ApiResponse<object>.ErrorResult("Invalid patient ID."));
//         }

//         if (!ModelState.IsValid)
//         {
//             var errors = ModelState
//                 .Where(x => x.Value.Errors.Count > 0)
//                 .Select(x => new { Field = x.Key, Errors = x.Value.Errors.Select(e => e.ErrorMessage) });
            
//             var errorMessage = $"Invalid input data: {string.Join(", ", errors.SelectMany(e => e.Errors))}";
//             Console.WriteLine($"Model validation failed: {errorMessage}");
//             return BadRequest(ApiResponse<object>.ErrorResult(errorMessage));
//         }

//         var existingPatient = await _patientRepository.GetByIdAsync(id);
//         if (existingPatient == null)
//         {
//             Console.WriteLine($"Patient with ID {id} not found for update");
//             return NotFound(ApiResponse<object>.ErrorResult("Patient not found."));
//         }

//         var patient = new Patient
//         {
//             PatientId = updatePatientDto.PatientId,
//             PatientName = updatePatientDto.PatientName?.Trim(),
//             DateOfBirth = updatePatientDto.DateOfBirth,
//             Gender = updatePatientDto.Gender?.Trim(),
//             ContactNumber = updatePatientDto.ContactNumber?.Trim(),
//             Email = updatePatientDto.Email?.Trim(),
//             Address = updatePatientDto.Address?.Trim(),
//             EmergencyContact = updatePatientDto.EmergencyContact?.Trim()
//         };

//         Console.WriteLine($"Updating patient: {patient.PatientName}");

//         var success = await _patientRepository.UpdateAsync(patient);
        
//         Console.WriteLine($"Update result: {success}");
        
//         if (success)
//         {
//             var updatedPatient = await _patientRepository.GetByIdAsync(id);
//             return Ok(ApiResponse<Patient>.SuccessResult(updatedPatient, "Patient updated successfully."));
//         }

//         Console.WriteLine("Update operation returned false");
//         return BadRequest(ApiResponse<object>.ErrorResult("Failed to update patient."));
//     }
//     catch (ArgumentException ex)
//     {
//         Console.WriteLine($"Argument exception: {ex.Message}");
//         return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
//     }
//     catch (InvalidOperationException ex)
//     {
//         Console.WriteLine($"Invalid operation exception: {ex.Message}");
//         return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
//     }
//     catch (Exception ex)
//     {
//         Console.WriteLine($"General exception: {ex.Message}");
//         Console.WriteLine($"Stack trace: {ex.StackTrace}");
//         return StatusCode(500, ApiResponse<object>.ErrorResult("Error updating patient."));
//     }
// }

// [HttpDelete("{id}")]
// [Authorize(Roles = "Admin")]
// public async Task<IActionResult> DeletePatient(int id)
// {
//     try
//     {
//         // Log the incoming request
//         Console.WriteLine($"Attempting to delete patient with ID: {id}");
        
//         if (id <= 0)
//         {
//             Console.WriteLine("Invalid patient ID provided");
//             return BadRequest(ApiResponse<object>.ErrorResult("Invalid patient ID."));
//         }

//         var existingPatient = await _patientRepository.GetByIdAsync(id);
//         if (existingPatient == null)
//         {
//             Console.WriteLine($"Patient with ID {id} not found");
//             return NotFound(ApiResponse<object>.ErrorResult("Patient not found."));
//         }

//         Console.WriteLine($"Patient found: {existingPatient.PatientName}, attempting deletion");

//         var success = await _patientRepository.DeleteAsync(id);
        
//         Console.WriteLine($"Deletion result: {success}");
        
//         if (success)
//         {
//             return Ok(ApiResponse<object>.SuccessResult(null, "Patient deleted successfully."));
//         }

//         Console.WriteLine("Delete operation returned false");
//         return BadRequest(ApiResponse<object>.ErrorResult("Failed to delete patient."));
//     }
//     catch (ArgumentException ex)
//     {
//         Console.WriteLine($"Argument exception: {ex.Message}");
//         return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
//     }
//     catch (InvalidOperationException ex)
//     {
//         Console.WriteLine($"Invalid operation exception: {ex.Message}");
//         return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
//     }
//     catch (Exception ex)
//     {
//         Console.WriteLine($"General exception: {ex.Message}");
//         Console.WriteLine($"Stack trace: {ex.StackTrace}");
//         return StatusCode(500, ApiResponse<object>.ErrorResult("Error deleting patient."));
//     }
// }
//     }
// }
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webapi.Models;
using webapi.Repositories;
using FluentValidation;

namespace webapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PatientController : ControllerBase
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IValidator<CreatePatientDto> _createPatientValidator;
        private readonly IValidator<UpdatePatientDto> _updatePatientValidator;

        public PatientController(IPatientRepository patientRepository,
            IValidator<CreatePatientDto> createPatientValidator,
            IValidator<UpdatePatientDto> updatePatientValidator)
        {
            _patientRepository = patientRepository;
            _createPatientValidator = createPatientValidator;
            _updatePatientValidator = updatePatientValidator;
        }

        [HttpGet]
        [Authorize(Policy = "AdminOrReceptionist")]
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
        [Authorize(Policy = "AdminOrReceptionist")]
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
        [Authorize(Policy = "AdminOrReceptionist")]
        public async Task<IActionResult> CreatePatient([FromBody] CreatePatientDto createPatientDto)
        {
            try
            {
                var validationResult = await _createPatientValidator.ValidateAsync(createPatientDto);
                if (!validationResult.IsValid)
                {
                    return BadRequest(ApiResponse<object>.ErrorResult(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))));
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
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdatePatient(int id, [FromBody] UpdatePatientDto updatePatientDto)
        {
            try
            {
                if (id <= 0 || updatePatientDto.PatientId != id)
                {
                    return BadRequest(ApiResponse<object>.ErrorResult("Invalid patient ID."));
                }

                var validationResult = await _updatePatientValidator.ValidateAsync(updatePatientDto);
                if (!validationResult.IsValid)
                {
                    return BadRequest(ApiResponse<object>.ErrorResult(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))));
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
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResult("Error updating patient."));
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
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
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResult("Error deleting patient."));
            }
        }
    }
}
