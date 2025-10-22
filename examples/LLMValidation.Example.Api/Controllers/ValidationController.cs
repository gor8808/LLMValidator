using LLMValidation.Example.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace LLMValidation.Example.Api.Controllers;

/// <summary>
/// API controller for validating dog descriptions using LLM-based validation.
/// FluentValidation automatically validates the model before entering the action method.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ValidationController : ControllerBase
{
    /// <summary>
    /// Validates a dog description.
    /// </summary>
    /// <param name="model">The dog description to validate</param>
    /// <returns>Validation result indicating success or failure with error details</returns>
    /// <response code="200">Description is valid</response>
    /// <response code="400">Description failed validation</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Validate([FromBody] DogDescription model)
    {
        // FluentValidation automatically validates the model
        // If validation fails, ModelState will contain errors
        // This code only executes if validation passes

        return Ok(new
        {
            IsValid = true,
            Message = "Description is valid!"
        });
    }

    /// <summary>
    /// Health check endpoint.
    /// </summary>
    [HttpGet("health")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Health()
    {
        return Ok(new
        {
            Status = "healthy",
            Timestamp = DateTime.UtcNow
        });
    }
}
