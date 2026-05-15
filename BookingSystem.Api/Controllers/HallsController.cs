using BookingSystem.Api.Services.Interfaces;
using BookingSystem.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HallsController : ControllerBase
{
    private readonly IHallService _hallService;

    public HallsController(IHallService hallService)
    {
        _hallService = hallService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var halls = await _hallService.GetAllAsync();
        return Ok(halls);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var hall = await _hallService.GetByIdAsync(id);
        if (hall == null) return NotFound();
        return Ok(hall);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] Hall hall)
    {
        var result = await _hallService.CreateAsync(hall);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] Hall hall)
    {
        if (id != hall.Id) return BadRequest();
        var success = await _hallService.UpdateAsync(hall);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _hallService.DeleteAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}
