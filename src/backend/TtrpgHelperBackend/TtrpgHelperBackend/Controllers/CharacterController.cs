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
    //public CharacterController(ICharacterService characterServiceService, ApplicationDbContext context)

    public CharacterController(ICharacterService characterServiceService, ApplicationDbContext context)
    {
        _characterService = characterServiceService;
        _context = context;
    }
    
    [Authorize] 
    [HttpPost]
    public async Task<ActionResult<CharacterReadDto>> CreateCharacter([FromBody] CharacterCreationDto request)
    {
       
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return Unauthorized("User ID not found in token.");
        }
    
        // 2. Pass the retrieved userId and the DTO to the service
        var newCharacter = await _characterService.CreateCharacter(userId, request);
        var readDto = new CharacterReadDto()
        {
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
    
    [HttpGet("classes")]
    public async Task<ActionResult<IEnumerable<Class>>> GetClasses()
    {
        var classes = await _context.Classes.ToListAsync();
        return Ok(classes);
    }
    
    [HttpGet("races")]
    public async Task<ActionResult<IEnumerable<Race>>> GetRaces()
    {
        var races = await _context.Races.ToListAsync();
        return Ok(races);
    }
    
    // [HttpGet("backgrounds")]
    // public async Task<ActionResult<IEnumerable<Background>>> GetBackgrounds()
    // {
    //     var bg = await _context.Backgrounds.ToListAsync();
    //     return Ok(bg);
    // }
    [HttpGet("Skills")]
    public async Task<ActionResult<IEnumerable<Race>>> GetSkills()
    {
        var skills = await _context.Skills.ToListAsync();
        return Ok(skills);
    }
}