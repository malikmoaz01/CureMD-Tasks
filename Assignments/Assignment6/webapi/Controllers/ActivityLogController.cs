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
    public class ActivityLogController : ControllerBase
    {
        private readonly IActivityLogRepository _activityLogRepository;
        private readonly IValidator<ActivityLog> _activityLogValidator;

        public ActivityLogController(IActivityLogRepository activityLogRepository, IValidator<ActivityLog> activityLogValidator)
        {
            _activityLogRepository = activityLogRepository;
            _activityLogValidator = activityLogValidator;
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetAllActivityLogs()
        {
            try
            {
                var activityLogs = await _activityLogRepository.GetAllAsync();
                return Ok(ApiResponse<IEnumerable<ActivityLog>>.SuccessResult(activityLogs, "Activity logs retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResult($"Error retrieving activity logs: {ex.Message}"));
            }
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetActivityLogById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(ApiResponse<object>.ErrorResult("Invalid activity log ID."));
                }

                var activityLog = await _activityLogRepository.GetByIdAsync(id);
                
                if (activityLog == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResult("Activity log not found."));
                }

                return Ok(ApiResponse<ActivityLog>.SuccessResult(activityLog, "Activity log retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResult($"Error retrieving activity log: {ex.Message}"));
            }
        }

        [HttpPost]
        [Authorize(Policy = "AdminOrReceptionist")]
        public async Task<IActionResult> CreateActivityLog([FromBody] ActivityLog activityLog)
        {
            try
            {
                var validationResult = await _activityLogValidator.ValidateAsync(activityLog);
                if (!validationResult.IsValid)
                {
                    return BadRequest(ApiResponse<object>.ErrorResult(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))));
                }

                var currentUserId = GetCurrentUserId();
                activityLog.UserId = currentUserId;
                activityLog.LogDateTime = DateTime.UtcNow;

                var activityLogId = await _activityLogRepository.AddAsync(activityLog);
                
                if (activityLogId > 0)
                {
                    var createdActivityLog = await _activityLogRepository.GetByIdAsync(activityLogId);
                    return CreatedAtAction(nameof(GetActivityLogById), new { id = activityLogId }, 
                        ApiResponse<ActivityLog>.SuccessResult(createdActivityLog, "Activity log created successfully."));
                }

                return BadRequest(ApiResponse<object>.ErrorResult("Failed to create activity log."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResult($"Error creating activity log: {ex.Message}"));
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteActivityLog(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(ApiResponse<object>.ErrorResult("Invalid activity log ID."));
                }

                var existingActivityLog = await _activityLogRepository.GetByIdAsync(id);
                
                if (existingActivityLog == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResult("Activity log not found."));
                }

                var success = await _activityLogRepository.DeleteAsync(id);
                
                if (success)
                {
                    return Ok(ApiResponse<object>.SuccessResult(null, "Activity log deleted successfully."));
                }
                else
                {
                    return StatusCode(500, ApiResponse<object>.ErrorResult("Failed to delete activity log - no rows affected."));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResult($"Error deleting activity log: {ex.Message}"));
            }
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : null;
        }
    }
}