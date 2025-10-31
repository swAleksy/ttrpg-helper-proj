using Microsoft.EntityFrameworkCore;
using TtrpgHelperBackend.DTOs;
using TtrpgHelperBackend.Models;

namespace TtrpgHelperBackend.Services;

public interface ICharacterService
{
    Task<Character> CreateCharacter(int userId, CharacterDto request);
    Task<Character> UpdateCharacter(int characterId, int userId, CharacterDto request);
    Task<Boolean> DeleteCharacter(int characterId, int userId);
}

public class CharacterService : ICharacterService
{
    private readonly ApplicationDbContext _context;

    public CharacterService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Character> CreateCharacter(int userId, CharacterDto request)
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

    public async Task<Character> UpdateCharacter(int characterId, int userId, CharacterDto request)
    {
        var character = await _context.Characters
            .Include(c => c.CharacterSkills)
            .FirstOrDefaultAsync(c => c.Id == characterId && c.UserId == userId);

        if (character == null)
            return null;
        
        character.Name = request.Name;
        character.RaceId = request.RaceId;
        character.ClassId = request.ClassId;
        character.BackgroundId = request.BackgroundId;
        character.Level = request.Level;
        character.Strength = request.Strength;
        character.Dexterity = request.Dexterity;
        character.Constitution = request.Constitution;
        character.Intelligence = request.Intelligence;
        character.Wisdom = request.Wisdom;
        character.Charisma = request.Charisma;
        
        character.CharacterSkills.Clear();
        foreach (var skillId in request.CharacterSkillsIds)
        {
            character.CharacterSkills.Add(new CharacterSkill
            {
                CharacterId = characterId,
                SkillId = skillId
            });
        }

        // Save changes
        await _context.SaveChangesAsync();
        return character;
    }

    public async Task<Boolean> DeleteCharacter(int characterId, int userId)
    {
        var character = await _context.Characters
            .Include(c => c.CharacterSkills)
            .FirstOrDefaultAsync(c => c.Id == characterId && c.UserId == userId);

        if (character == null)
            return false;
        
        _context.Remove(character);
        await _context.SaveChangesAsync();
        return true;

    }
}