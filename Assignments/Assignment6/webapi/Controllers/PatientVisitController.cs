using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webapi.Models;
using webapi.Repositories;
using System.Security.Claims;
using FluentValidation;

namespace webapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PatientVisitController : ControllerBase
    {
        private readonly IPatientVisitRepository _patientVisitRepository;
        private readonly IValidator<CreatePatientVisitDto> _createPatientVisitValidator;
        private readonly IValidator<UpdatePatientVisitDto> _updatePatientVisitValidator;

        public PatientVisitController(IPatientVisitRepository patientVisitRepository,
            IValidator<CreatePatientVisitDto> createPatientVisitValidator,
            IValidator<UpdatePatientVisitDto> updatePatientVisitValidator)
        {
            _patientVisitRepository = patientVisitRepository;
            _createPatientVisitValidator = createPatientVisitValidator;
            _updatePatientVisitValidator = updatePatientVisitValidator;
        }

        [HttpGet]
        [Authorize(Policy = "AdminOrReceptionist")]
        public async Task<IActionResult> GetAllPatientVisits()
        {
            try
            {
                var patientVisits = await _patientVisitRepository.GetAllAsync();
                return Ok(ApiResponse<IEnumerable<PatientVisit>>.SuccessResult(patientVisits, "Patient visits retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResult("Error retrieving patient visits."));
            }
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOrReceptionist")]
        public async Task<IActionResult> GetPatientVisitById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(ApiResponse<object>.ErrorResult("Invalid patient visit ID."));
                }

                var patientVisit = await _patientVisitRepository.GetByIdAsync(id);
                
                if (patientVisit == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResult("Patient visit not found."));
                }

                return Ok(ApiResponse<PatientVisit>.SuccessResult(patientVisit, "Patient visit retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResult("Error retrieving patient visit."));
            }
        }

        [HttpPost]
        [Authorize(Policy = "AdminOrReceptionist")]
        public async Task<IActionResult> CreatePatientVisit([FromBody] CreatePatientVisitDto createPatientVisitDto)
        {
            try
            {
                var validationResult = await _createPatientVisitValidator.ValidateAsync(createPatientVisitDto);
                if (!validationResult.IsValid)
                {
                    return BadRequest(ApiResponse<object>.ErrorResult(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))));
                }

                var currentUserId = GetCurrentUserId();

                var patientVisit = new PatientVisit
                {
                    PatientId = createPatientVisitDto.PatientId,
                    DoctorId = createPatientVisitDto.DoctorId,
                    VisitTypeId = createPatientVisitDto.VisitTypeId,
                    VisitDate = createPatientVisitDto.VisitDate,
                    Note = createPatientVisitDto.Note?.Trim(),
                    DurationInMinutes = createPatientVisitDto.DurationInMinutes,
                    Fee = createPatientVisitDto.Fee,
                    CreatedBy = currentUserId
                };

                var patientVisitId = await _patientVisitRepository.AddAsync(patientVisit);
                
                if (patientVisitId > 0)
                {
                    var createdPatientVisit = await _patientVisitRepository.GetByIdAsync(patientVisitId);
                    return CreatedAtAction(nameof(GetPatientVisitById), new { id = patientVisitId }, 
                        ApiResponse<PatientVisit>.SuccessResult(createdPatientVisit, "Patient visit created successfully."));
                }

                return BadRequest(ApiResponse<object>.ErrorResult("Failed to create patient visit."));
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
                return StatusCode(500, ApiResponse<object>.ErrorResult("Error creating patient visit."));
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdatePatientVisit(int id, [FromBody] UpdatePatientVisitDto updatePatientVisitDto)
        {
            try
            {
                if (id <= 0 || updatePatientVisitDto.Id != id)
                {
                    return BadRequest(ApiResponse<object>.ErrorResult("Invalid patient visit ID."));
                }

                var validationResult = await _updatePatientVisitValidator.ValidateAsync(updatePatientVisitDto);
                if (!validationResult.IsValid)
                {
                    return BadRequest(ApiResponse<object>.ErrorResult(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))));
                }

                var existingPatientVisit = await _patientVisitRepository.GetByIdAsync(id);
                if (existingPatientVisit == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResult("Patient visit not found."));
                }

                var currentUserId = GetCurrentUserId();

                var patientVisit = new PatientVisit
                {
                    Id = updatePatientVisitDto.Id,
                    PatientId = updatePatientVisitDto.PatientId,
                    DoctorId = updatePatientVisitDto.DoctorId,
                    VisitTypeId = updatePatientVisitDto.VisitTypeId,
                    VisitDate = updatePatientVisitDto.VisitDate,
                    Note = updatePatientVisitDto.Note?.Trim(),
                    DurationInMinutes = updatePatientVisitDto.DurationInMinutes,
                    Fee = updatePatientVisitDto.Fee,
                    ModifiedBy = currentUserId
                };

                var success = await _patientVisitRepository.UpdateAsync(patientVisit);
                
                if (success)
                {
                    var updatedPatientVisit = await _patientVisitRepository.GetByIdAsync(id);
                    return Ok(ApiResponse<PatientVisit>.SuccessResult(updatedPatientVisit, "Patient visit updated successfully."));
                }

                return BadRequest(ApiResponse<object>.ErrorResult("Failed to update patient visit."));
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
                return StatusCode(500, ApiResponse<object>.ErrorResult("Error updating patient visit."));
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeletePatientVisit(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(ApiResponse<object>.ErrorResult("Invalid patient visit ID."));
                }

                var existingPatientVisit = await _patientVisitRepository.GetByIdAsync(id);
                if (existingPatientVisit == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResult("Patient visit not found."));
                }

                var success = await _patientVisitRepository.DeleteAsync(id);
                
                if (success)
                {
                    return Ok(ApiResponse<object>.SuccessResult(null, "Patient visit deleted successfully."));
                }

                return BadRequest(ApiResponse<object>.ErrorResult("Failed to delete patient visit."));
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
                return StatusCode(500, ApiResponse<object>.ErrorResult("Error deleting patient visit."));
            }
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : null;
        }
    }
}
