using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webapi.Models;
using webapi.Repositories;
using System.Security.Claims;

namespace webapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ActivityLogController : ControllerBase
    {
        private readonly IActivityLogRepository _activityLogRepository;

        public ActivityLogController(IActivityLogRepository activityLogRepository)
        {
            _activityLogRepository = activityLogRepository;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllActivityLogs()
        {
            try
            {
                var activityLogs = await _activityLogRepository.GetAllAsync();
                return Ok(ApiResponse<IEnumerable<ActivityLog>>.SuccessResult(activityLogs, "Activity logs retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResult("Error retrieving activity logs."));
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
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
                return StatusCode(500, ApiResponse<object>.ErrorResult("Error retrieving activity log."));
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateActivityLog([FromBody] ActivityLog activityLog)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<object>.ErrorResult("Invalid input data."));
                }

                var currentUserId = GetCurrentUserId();
                activityLog.UserId = currentUserId;

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
                return StatusCode(500, ApiResponse<object>.ErrorResult("Error creating activity log."));
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
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

                return BadRequest(ApiResponse<object>.ErrorResult("Failed to delete activity log."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResult("Error deleting activity log."));
            }
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : null;
        }
    }
}