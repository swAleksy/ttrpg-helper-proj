using Microsoft.EntityFrameworkCore;
using TtrpgHelperBackend.DTOs;
using TtrpgHelperBackend.Models;

namespace TtrpgHelperBackend.Services;

public interface ICharacterService
{
    Task<Character> CreateCharacter(int userId, CharacterCreationDto request);
}

public class CharacterService : ICharacterService
{
    private readonly ApplicationDbContext _context;

    public CharacterService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Character> CreateCharacter(int userId, CharacterCreationDto request)
    {
        var newCharacter = new Character
        {
            UserId = userId, 
            
            Name = request.Name,
            RaceId = request.RaceId,
            ClassId = request.ClassId,
            BackgroundId = request.BackgroundId,
            Level = request.Level,

            Strength = request.Strength,
            Dexterity = request.Dexterity,
            Constitution = request.Constitution,
            Intelligence = request.Intelligence,
            Wisdom = request.Wisdom,
            Charisma = request.Charisma
        };
        
        if (request.CharacterSkillsIds.Any())
        {
            newCharacter.CharacterSkills = request.CharacterSkillsIds
                .Select(skillId => new CharacterSkill
                {
                    SkillId = skillId,
                    IsProficient = true, // Flag it as proficient
                    SkillValue = 0 // Placeholder, or calculate this on save/update
                }).ToList();
        }

        // 3. Add the Character (and linked CharacterSkills) to the database
        _context.Characters.Add(newCharacter);
        await _context.SaveChangesAsync();
        
        return newCharacter;
    }

}