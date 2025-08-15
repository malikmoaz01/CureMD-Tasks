using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webapi.Models;
using webapi.Repositories;

namespace webapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VisitTypeController : ControllerBase
    {
        private readonly IVisitTypeRepository _visitTypeRepository;

        public VisitTypeController(IVisitTypeRepository visitTypeRepository)
        {
            _visitTypeRepository = visitTypeRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllVisitTypes()
        {
            try
            {
                var visitTypes = await _visitTypeRepository.GetAllAsync();
                return Ok(ApiResponse<IEnumerable<VisitType>>.SuccessResult(visitTypes, "Visit types retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResult("Error retrieving visit types."));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVisitTypeById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(ApiResponse<object>.ErrorResult("Invalid visit type ID."));
                }

                var visitType = await _visitTypeRepository.GetByIdAsync(id);
                
                if (visitType == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResult("Visit type not found."));
                }

                return Ok(ApiResponse<VisitType>.SuccessResult(visitType, "Visit type retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResult("Error retrieving visit type."));
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateVisitType([FromBody] CreateVisitTypeDto createVisitTypeDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<object>.ErrorResult("Invalid input data."));
                }

                var visitType = new VisitType
                {
                    VisitTypeName = createVisitTypeDto.VisitTypeName?.Trim(),
                    BaseRate = createVisitTypeDto.BaseRate,
                    Description = createVisitTypeDto.Description?.Trim()
                };

                var visitTypeId = await _visitTypeRepository.AddAsync(visitType);
                
                if (visitTypeId > 0)
                {
                    var createdVisitType = await _visitTypeRepository.GetByIdAsync(visitTypeId);
                    return CreatedAtAction(nameof(GetVisitTypeById), new { id = visitTypeId }, 
                        ApiResponse<VisitType>.SuccessResult(createdVisitType, "Visit type created successfully."));
                }

                return BadRequest(ApiResponse<object>.ErrorResult("Failed to create visit type."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResult("Error creating visit type."));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVisitType(int id, [FromBody] VisitType visitType)
        {
            try
            {
                if (id <= 0 || visitType.VisitTypeId != id)
                {
                    return BadRequest(ApiResponse<object>.ErrorResult("Invalid visit type ID."));
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<object>.ErrorResult("Invalid input data."));
                }

                var existingVisitType = await _visitTypeRepository.GetByIdAsync(id);
                if (existingVisitType == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResult("Visit type not found."));
                }

                visitType.VisitTypeName = visitType.VisitTypeName?.Trim();
                visitType.Description = visitType.Description?.Trim();

                var success = await _visitTypeRepository.UpdateAsync(visitType);
                
                if (success)
                {
                    var updatedVisitType = await _visitTypeRepository.GetByIdAsync(id);
                    return Ok(ApiResponse<VisitType>.SuccessResult(updatedVisitType, "Visit type updated successfully."));
                }

                return BadRequest(ApiResponse<object>.ErrorResult("Failed to update visit type."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResult("Error updating visit type."));
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteVisitType(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(ApiResponse<object>.ErrorResult("Invalid visit type ID."));
                }

                var existingVisitType = await _visitTypeRepository.GetByIdAsync(id);
                if (existingVisitType == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResult("Visit type not found."));
                }

                var success = await _visitTypeRepository.DeleteAsync(id);
                
                if (success)
                {
                    return Ok(ApiResponse<object>.SuccessResult(null, "Visit type deleted successfully."));
                }

                return BadRequest(ApiResponse<object>.ErrorResult("Failed to delete visit type."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResult("Error deleting visit type."));
            }
        }
    }
}