using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using robot_controller_api.Persistence;

namespace robot_controller_api.Controllers;

[ApiController]
[Route("api/maps")]
public class MapsController : ControllerBase
{
    /// <summary>
    /// Retrieves all maps.
    /// </summary>
    /// <returns>A list of all maps</returns>
    [HttpGet]
    // [Authorize(Policy = "UserOnly")]
    public IActionResult GetAllMaps()
    {
        var maps = MapDataAccess.GetMaps();
        return Ok(maps);
    }

    /// <summary>
    /// Retrieves maps where the number of rows is equal to the number of columns.
    /// </summary>
    /// <returns>A list of square maps</returns>
    [HttpGet("square")]
    public IActionResult GetSquareMaps()
    {
        var maps = MapDataAccess.GetMaps()
            .Where(m => m.Rows == m.Columns)
            .ToList();
        return Ok(maps);
    }

    /// <summary>
    /// Retrieves a specific map by its ID.
    /// </summary>
    /// <param name="id">The ID of the map</param>
    /// <returns>A map with the specified ID, or a NotFound result if not found</returns>
    [HttpGet("{id}", Name = "GetMapById")]
    // [Authorize(Policy = "UserOnly")]
    
    public IActionResult GetMapById(int id)
    {
        var map = MapDataAccess.GetMapById(id);
        return map != null ? Ok(map) : NotFound();
    }

   
    [HttpPost]
    // [Authorize(Policy = "AdminOnly")]
    // [Authorize(Policy = "DeveloperOnly")]
    public IActionResult AddMap(Map newMap)
    {
        newMap.CreatedDate = DateTime.UtcNow;
        newMap.ModifiedDate = DateTime.UtcNow;

        MapDataAccess.AddMap(newMap);
        return CreatedAtAction(nameof(GetMapById), new { id = newMap.Id }, newMap);
    }

    /// <summary>
    /// Updates an existing map.
    /// </summary>
    /// <param name="id">The ID of the map to update</param>
    /// <param name="updatedMap">The updated map data</param>
    /// <returns>No content if successful, NotFound if map doesn't exist</returns>
    [HttpPut("{id}")]
    // [Authorize(Policy = "AdminOnly")]
    public IActionResult UpdateMap(int id, Map updatedMap)
    {
        updatedMap.ModifiedDate = DateTime.UtcNow;
        var success = MapDataAccess.UpdateMap(id, updatedMap);
        return success ? NoContent() : NotFound();
    }

    /// <summary>
    /// Deletes a map from the system.
    /// </summary>
    /// <param name="id">The ID of the map to delete</param>
    /// <returns>No content if successful, NotFound if map doesn't exist</returns>
    [HttpDelete("{id}")]
    // [Authorize(Policy = "AdminOnly")]
    // [Authorize(Policy = "DeveloperOnly")]
    public IActionResult DeleteMap(int id)
    {
        var success = MapDataAccess.DeleteMap(id);
        return success ? NoContent() : NotFound();
    }

    /// <summary>
    /// Checks if the given coordinates are valid for a specific map.
    /// </summary>
    /// <param name="id">The ID of the map</param>
    /// <param name="x">The x-coordinate to check</param>
    /// <param name="y">The y-coordinate to check</param>
    /// <returns>True if coordinates are within the map, BadRequest if coordinates are negative</returns>
    [HttpGet("{id}/{x}-{y}")]
    // [Authorize(Policy = "UserOnly")]
    public IActionResult CheckCoordinate(int id, int x, int y)
    {
        if (x < 0 || y < 0)
            return BadRequest("Coordinates must be non-negative");

        var map = MapDataAccess.GetMapById(id);
        if (map == null)
            return NotFound();

        bool isOnMap = x < map.Columns && y < map.Rows;
        return Ok(isOnMap);
    }
}
