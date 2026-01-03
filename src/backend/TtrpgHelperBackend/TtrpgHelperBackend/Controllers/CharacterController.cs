using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TtrpgHelperBackend.DTOs;
using TtrpgHelperBackend.Models;
using TtrpgHelperBackend.Services;

namespace TtrpgHelperBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CharacterController : ControllerBase
{
    private readonly ICharacterService _characterService;
    private readonly ApplicationDbContext _context;

    public CharacterController(ICharacterService characterService, ApplicationDbContext context)
    {
        _characterService = characterService;
        _context = context;
    }
    
    [Authorize] 
    [HttpPost]
    public async Task<ActionResult<CharacterDto>> CreateCharacter([FromBody] CharacterDto request)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized("User ID not found in token.");
    
        // 2. Pass the retrieved userId and the DTO to the service
        var newCharacter = await _characterService.CreateCharacter(userId.Value, request);
        var readDto = new CharacterDto()
        {
            Id = newCharacter.Id,
            Name = newCharacter.Name,
            RaceId = newCharacter.RaceId,
            ClassId = newCharacter.ClassId,
            BackgroundId = newCharacter.BackgroundId,
            Level = newCharacter.Level,
            Strength = newCharacter.Strength,
            Dexterity = newCharacter.Dexterity,
            Constitution = newCharacter.Constitution,
            Intelligence = newCharacter.Intelligence,
            Wisdom = newCharacter.Wisdom,
            Charisma = newCharacter.Charisma,
            CharacterSkillsIds = newCharacter.CharacterSkills.Select(cs => cs.SkillId).ToList()
        };
        
        return Ok(readDto);
    }
    
    [Authorize]
    [HttpGet("AllCharacters")]
    public async Task<ActionResult<IEnumerable<CharacterDto>>> GetAllCharacters()
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized("User ID not found in token.");

        // Fetch all characters for this user
        var characters = await _context.Characters
            .Where(c => c.UserId == userId)
            .Include(c => c.CharacterSkills)
            .ToListAsync();

        var charactersResponse = characters.Select(c => new CharacterDto()
        {
            Id = c.Id,
            Name = c.Name,
            RaceId = c.RaceId,
            ClassId = c.ClassId,
            BackgroundId = c.BackgroundId,
            Level = c.Level,
            Strength = c.Strength,
            Dexterity = c.Dexterity,
            Constitution = c.Constitution,
            Intelligence = c.Intelligence,
            Wisdom = c.Wisdom,
            Charisma = c.Charisma,
            CharacterSkillsIds = c.CharacterSkills.Select(cs => cs.SkillId).ToList()
        }).ToList();
        
        return Ok(charactersResponse);
    }

    [Authorize]
    [HttpPut("UpdateCharacter/{id}")]
    public async Task<IActionResult> UpdateCharacter(int id, [FromBody] CharacterDto request)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized("User ID not found in token.");
    
        // 2. Pass the retrieved userId and the DTO to the service
        var updated = await _characterService.UpdateCharacter(id, userId.Value, request);
        if (updated == null)
            return NotFound("Character not found or does not belong to this user.");
        return Ok("Successfully updated the character");
    }
    
    [Authorize]
    [HttpDelete("DeleteCharacter/{id}")]
    public async Task<IActionResult> DeleteCharacter(int id)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized("User ID not found in token.");

        var deleted = await _characterService.DeleteCharacter(id, userId.Value);
        if (!deleted) 
            return NotFound("Character not found.");

        return Ok("Character deleted successfully.");
    }
    
    [HttpGet("classes")]
    public async Task<ActionResult<IEnumerable<ClassDto>>> GetClasses()
        => Ok(await _context.Classes
            .Select(c => new ClassDto { Id = c.Id, Name = c.Name, Description = c.Description })
            .ToListAsync());

    [HttpGet("races")]
    public async Task<ActionResult<IEnumerable<RaceDto>>> GetRaces()
        => Ok(await _context.Races
            .Select(r => new RaceDto { Id = r.Id, Name = r.Name, Description = r.Description })
            .ToListAsync());

    [HttpGet("backgrounds")]
    public async Task<ActionResult<IEnumerable<BackgroundDto>>> GetBackgrounds()
        => Ok(await _context.Backgrounds
            .Select(b => new BackgroundDto { Id = b.Id, Name = b.Name, Description = b.Description })
            .ToListAsync());

    [HttpGet("skills")]
    public async Task<ActionResult<IEnumerable<SkillDto>>> GetSkills()
        => Ok(await _context.Skills
            .Select(s => new SkillDto { Id = s.Id, Name = s.Name, Description = s.Description })
            .ToListAsync());
    
    private int? GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null && int.TryParse(claim.Value, out int userId)
            ? userId
            : (int?)null;
    }
}