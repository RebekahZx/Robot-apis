using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using robot_controller_api.Persistence;

namespace robot_controller_api.Controllers;

[ApiController]
[Route("api/robot-commands")]
public class RobotCommandsController : ControllerBase
{
    /// <summary>
    /// Retrieves all robot commands.
    /// </summary>
    /// <returns>A list of all robot commands</returns>
    [HttpGet]
    
    public IActionResult GetAllRobotCommands()
    {
        var commands = RobotCommandDataAccess.GetRobotCommands();
        return Ok(commands);
    }

    /// <summary>
    /// Retrieves all move-related robot commands.
    /// </summary>
    /// <returns>A list of robot move commands</returns>
    [HttpGet("move")]
    
    public IActionResult GetMoveCommands()
    {
        var commands = RobotCommandDataAccess.GetRobotCommands()
            .Where(cmd => cmd.IsMoveCommand)
            .ToList();
        return Ok(commands);
    }

    /// <summary>
    /// Retrieves a specific robot command by its ID.
    /// </summary>
    /// <param name="id">The ID of the robot command</param>
    /// <returns>The robot command with the specified ID, or NotFound if not found</returns>
    [HttpGet("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult GetRobotCommandById(int id)
    {
        var command = RobotCommandDataAccess.GetRobotCommands()
            .FirstOrDefault(cmd => cmd.Id == id);

        return command != null ? Ok(command) : NotFound();
    }


    [HttpPost]
    // [Authorize(Policy = "AdminOnly")]
    public IActionResult AddRobotCommand(RobotCommand newCommand)
    {
        if (newCommand == null)
        {
            return BadRequest("Command cannot be null.");
        }

        newCommand.CreatedDate = DateTime.UtcNow;
        newCommand.ModifiedDate = DateTime.UtcNow;

        RobotCommandDataAccess.AddRobotCommand(newCommand);
        return Ok("Command added successfully.");
    }

    /// <summary>
    /// Updates an existing robot command.
    /// </summary>
    /// <param name="id">The ID of the robot command to update</param>
    /// <param name="updatedCommand">The updated robot command data</param>
    /// <returns>NoContent if the update was successful, NotFound if the command doesn't exist</returns>
    [HttpPut("{id}")]
    // [Authorize(Policy = "AdminOnly")]
    public IActionResult UpdateRobotCommand(int id, RobotCommand updatedCommand)
    {
        updatedCommand.ModifiedDate = DateTime.UtcNow;

        bool success = RobotCommandDataAccess.UpdateRobotCommand(id, updatedCommand);
        return success ? NoContent() : NotFound();
    }

    /// <summary>
    /// Deletes a robot command by its ID.
    /// </summary>
    /// <param name="id">The ID of the robot command to delete</param>
    /// <returns>NoContent if the deletion was successful, NotFound if the command doesn't exist</returns>
    [HttpDelete("{id}")]
    public IActionResult DeleteRobotCommand(int id)
    {
        bool success = RobotCommandDataAccess.DeleteRobotCommand(id);
        return success ? NoContent() : NotFound();
    }
}
