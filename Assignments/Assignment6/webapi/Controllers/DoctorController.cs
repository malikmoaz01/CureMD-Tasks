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
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IValidator<CreateDoctorDto> _createDoctorValidator;
        private readonly IValidator<UpdateDoctorDto> _updateDoctorValidator;

        public DoctorController(IDoctorRepository doctorRepository, 
            IValidator<CreateDoctorDto> createDoctorValidator,
            IValidator<UpdateDoctorDto> updateDoctorValidator)
        {
            _doctorRepository = doctorRepository;
            _createDoctorValidator = createDoctorValidator;
            _updateDoctorValidator = updateDoctorValidator;
        }

        [HttpGet]
        [Authorize(Policy = "AdminOrReceptionist")]
        public async Task<IActionResult> GetAllDoctors()
        {
            try
            {
                var doctors = await _doctorRepository.GetAllAsync();
                return Ok(ApiResponse<IEnumerable<Doctor>>.SuccessResult(doctors, "Doctors retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResult("Error retrieving doctors."));
            }
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOrReceptionist")]
        public async Task<IActionResult> GetDoctorById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(ApiResponse<object>.ErrorResult("Invalid doctor ID."));
                }

                var doctor = await _doctorRepository.GetByIdAsync(id);
                
                if (doctor == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResult("Doctor not found."));
                }

                return Ok(ApiResponse<Doctor>.SuccessResult(doctor, "Doctor retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResult("Error retrieving doctor."));
            }
        }

        [HttpPost]
        [Authorize(Policy = "AdminOrReceptionist")]
        public async Task<IActionResult> CreateDoctor([FromBody] CreateDoctorDto createDoctorDto)
        {
            try
            {
                var validationResult = await _createDoctorValidator.ValidateAsync(createDoctorDto);
                if (!validationResult.IsValid)
                {
                    return BadRequest(ApiResponse<object>.ErrorResult(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))));
                }

                var doctor = new Doctor
                {
                    DoctorName = createDoctorDto.DoctorName?.Trim(),
                    Specialization = createDoctorDto.Specialization?.Trim(),
                    ContactNumber = createDoctorDto.ContactNumber?.Trim(),
                    Email = createDoctorDto.Email?.Trim()
                };

                var doctorId = await _doctorRepository.AddAsync(doctor);
                
                if (doctorId > 0)
                {
                    var createdDoctor = await _doctorRepository.GetByIdAsync(doctorId);
                    return CreatedAtAction(nameof(GetDoctorById), new { id = doctorId }, 
                        ApiResponse<Doctor>.SuccessResult(createdDoctor, "Doctor created successfully."));
                }

                return BadRequest(ApiResponse<object>.ErrorResult("Failed to create doctor."));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResult("Error creating doctor."));
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateDoctor(int id, [FromBody] UpdateDoctorDto updateDoctorDto)
        {
            try
            {
                if (id <= 0 || updateDoctorDto.DoctorId != id)
                {
                    return BadRequest(ApiResponse<object>.ErrorResult("Invalid doctor ID."));
                }

                var validationResult = await _updateDoctorValidator.ValidateAsync(updateDoctorDto);
                if (!validationResult.IsValid)
                {
                    return BadRequest(ApiResponse<object>.ErrorResult(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))));
                }

                var existingDoctor = await _doctorRepository.GetByIdAsync(id);
                if (existingDoctor == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResult("Doctor not found."));
                }

                var doctor = new Doctor
                {
                    DoctorId = updateDoctorDto.DoctorId,
                    DoctorName = updateDoctorDto.DoctorName?.Trim(),
                    Specialization = updateDoctorDto.Specialization?.Trim(),
                    ContactNumber = updateDoctorDto.ContactNumber?.Trim(),
                    Email = updateDoctorDto.Email?.Trim()
                };

                var success = await _doctorRepository.UpdateAsync(doctor);
                
                if (success)
                {
                    var updatedDoctor = await _doctorRepository.GetByIdAsync(id);
                    return Ok(ApiResponse<Doctor>.SuccessResult(updatedDoctor, "Doctor updated successfully."));
                }

                return BadRequest(ApiResponse<object>.ErrorResult("Failed to update doctor."));
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
                return StatusCode(500, ApiResponse<object>.ErrorResult("Error updating doctor."));
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(ApiResponse<object>.ErrorResult("Invalid doctor ID."));
                }

                var existingDoctor = await _doctorRepository.GetByIdAsync(id);
                if (existingDoctor == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResult("Doctor not found."));
                }

                var success = await _doctorRepository.DeleteAsync(id);
                
                if (success)
                {
                    return Ok(ApiResponse<object>.SuccessResult(null, "Doctor deleted successfully."));
                }

                return BadRequest(ApiResponse<object>.ErrorResult("Failed to delete doctor."));
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
                return StatusCode(500, ApiResponse<object>.ErrorResult("Error deleting doctor."));
            }
        }
    }
}