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
    public async Task<ActionResult<CharacterDto>> CreateCharacter([FromBody] CharacterDto request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return Unauthorized("User ID not found in token.");
        }
    
        // 2. Pass the retrieved userId and the DTO to the service
        var newCharacter = await _characterService.CreateCharacter(userId, request);
        var readDto = new CharacterDto()
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
    public async Task<ActionResult<IEnumerable<ClassDto>>> GetClasses()
    {
        var classResponse = new List<ClassDto>();
        
        var classes = await _context.Classes.ToListAsync();
        foreach (var clss in classes)
        {
            ClassDto newClass = new ClassDto()
            {
                Id = clss.Id,
                Name = clss.Name,
                Description = clss.Description
            };
            classResponse.Add(newClass); 
        }
        return Ok(classResponse);
    }
    
    [HttpGet("races")]
    public async Task<ActionResult<IEnumerable<RaceDto>>> GetRaces()
    {
        var racesResponse = new List<RaceDto>();
        
        var races = await _context.Races.ToListAsync();
        foreach (var race in races)
        {
            RaceDto newRace = new RaceDto()
            {
                Id = race.Id,
                Name = race.Name,
                Description = race.Description
            };
            racesResponse.Add(newRace); 
        }
        return Ok(racesResponse);
    }
    
    [HttpGet("backgrounds")]
    public async Task<ActionResult<IEnumerable<BackgroundDto>>> GetBackgrounds()
    {
        var bgResponse = new List<BackgroundDto>();
        
        var bgs = await _context.Backgrounds.ToListAsync();
        foreach (var bg in bgs)
        {
            BackgroundDto newBg = new BackgroundDto()
            {
                Id = bg.Id,
                Name = bg.Name,
                Description = bg.Description
            };
            bgResponse.Add(newBg); 
        }
        return Ok(bgResponse);
    }
    
    [HttpGet("skills")]
    public async Task<ActionResult<IEnumerable<SkillDto>>> GetSkills()
    {
        var skillResponse = new List<SkillDto>();
        
        var skills = await _context.Skills.ToListAsync();
        foreach (var skill in skills)
        {
            SkillDto newSkill = new SkillDto()
            {
                Id = skill.Id,
                Name = skill.Name,
                Description = skill.Description
            };
            skillResponse.Add(newSkill); 
        }
        return Ok(skillResponse);
    }

    [Authorize]
    [HttpGet("AllCharacters")]
    public async Task<ActionResult<IEnumerable<Character>>> GetAllCharacters()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return Unauthorized("User ID not found in token.");
        }

        // Fetch all characters for this user
        var characters = await _context.Characters
            .Where(c => c.UserId == userId)
            .Include(c => c.CharacterSkills)
            .ToListAsync();

        var charactersResponse = characters.Select(c => new CharacterDto()
        {
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
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return Unauthorized("User ID not found in token.");
        }
    
        // 2. Pass the retrieved userId and the DTO to the service
        var characterUpdate = await _characterService.UpdateCharacter(id, userId, request);
        if (characterUpdate == null)
            return NotFound("Character not found or does not belong to this user.");
        return Ok("Successfully updated the character");
    }
    
    [Authorize]
    [HttpDelete("DeleteCharacter/{id}")]
    public async Task<IActionResult> DeleteCharacter(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            return Unauthorized();

        var deleted = await _characterService.DeleteCharacter(id, userId);

        if (!deleted) return NotFound("Character not found.");

        return Ok("Character deleted successfully.");
    }
}