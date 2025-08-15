using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webapi.Models;
using webapi.Repositories;

namespace webapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorRepository _doctorRepository;

        public DoctorController(IDoctorRepository doctorRepository)
        {
            _doctorRepository = doctorRepository;
        }

        [HttpGet]
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
public async Task<IActionResult> CreateDoctor([FromBody] CreateDoctorDto createDoctorDto)
{
    try
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new { Field = x.Key, Errors = x.Value.Errors.Select(e => e.ErrorMessage) });
            
            return BadRequest(ApiResponse<object>.ErrorResult($"Invalid input data: {string.Join(", ", errors.SelectMany(e => e.Errors))}"));
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
[HttpDelete("{id}")]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> DeleteDoctor(int id)
{
    try
    {
        // Log the incoming request
        Console.WriteLine($"Attempting to delete doctor with ID: {id}");
        
        if (id <= 0)
        {
            Console.WriteLine("Invalid doctor ID provided");
            return BadRequest(ApiResponse<object>.ErrorResult("Invalid doctor ID."));
        }

        // Check if doctor exists first
        var existingDoctor = await _doctorRepository.GetByIdAsync(id);
        if (existingDoctor == null)
        {
            Console.WriteLine($"Doctor with ID {id} not found");
            return NotFound(ApiResponse<object>.ErrorResult("Doctor not found."));
        }

        Console.WriteLine($"Doctor found: {existingDoctor.DoctorName}, attempting deletion");

        var success = await _doctorRepository.DeleteAsync(id);
        
        Console.WriteLine($"Deletion result: {success}");
        
        if (success)
        {
            return Ok(ApiResponse<object>.SuccessResult(null, "Doctor deleted successfully."));
        }

        Console.WriteLine("Delete operation returned false");
        return BadRequest(ApiResponse<object>.ErrorResult("Failed to delete doctor."));
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine($"Argument exception: {ex.Message}");
        return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"Invalid operation exception: {ex.Message}");
        return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
    }
    catch (Exception ex)
    {
        Console.WriteLine($"General exception: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
        return StatusCode(500, ApiResponse<object>.ErrorResult("Error deleting doctor."));
    }
}

[HttpPut("{id}")]
public async Task<IActionResult> UpdateDoctor(int id, [FromBody] UpdateDoctorDto updateDoctorDto)
{
    try
    {
        // Log the incoming request
        Console.WriteLine($"Attempting to update doctor with ID: {id}");
        
        if (id <= 0 || updateDoctorDto.DoctorId != id)
        {
            Console.WriteLine("Invalid doctor ID or ID mismatch");
            return BadRequest(ApiResponse<object>.ErrorResult("Invalid doctor ID."));
        }

        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new { Field = x.Key, Errors = x.Value.Errors.Select(e => e.ErrorMessage) });
            
            var errorMessage = $"Invalid input data: {string.Join(", ", errors.SelectMany(e => e.Errors))}";
            Console.WriteLine($"Model validation failed: {errorMessage}");
            return BadRequest(ApiResponse<object>.ErrorResult(errorMessage));
        }

        // Check if doctor exists first
        var existingDoctor = await _doctorRepository.GetByIdAsync(id);
        if (existingDoctor == null)
        {
            Console.WriteLine($"Doctor with ID {id} not found for update");
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

        Console.WriteLine($"Updating doctor: {doctor.DoctorName}");

        var success = await _doctorRepository.UpdateAsync(doctor);
        
        Console.WriteLine($"Update result: {success}");
        
        if (success)
        {
            var updatedDoctor = await _doctorRepository.GetByIdAsync(id);
            return Ok(ApiResponse<Doctor>.SuccessResult(updatedDoctor, "Doctor updated successfully."));
        }

        Console.WriteLine("Update operation returned false");
        return BadRequest(ApiResponse<object>.ErrorResult("Failed to update doctor."));
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine($"Argument exception: {ex.Message}");
        return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"Invalid operation exception: {ex.Message}");
        return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
    }
    catch (Exception ex)
    {
        Console.WriteLine($"General exception: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
        return StatusCode(500, ApiResponse<object>.ErrorResult("Error updating doctor."));
    }
}
    }
}