using TtrpgHelperBackend.Models.Resource;

namespace TtrpgHelperBackend.Seed;

public static class RuleSeeder
{
    public static async Task SeedRules(ApplicationDbContext db)
    {
        if (db.Rules.Any()) return;

        var rules = new List<Rule>
        {
            new Rule
            {
                Category = "Core Mechanics",
                Name = "Ability Checks",
                ContentMarkdown = """
When a character attempts an action whose outcome is uncertain,
the DM calls for an Ability Check.

**Roll:**
- d20 + relevant ability modifier

**Success:**
- Compare result to Difficulty Class (DC).
"""
            },

            new Rule
            {
                Category = "Core Mechanics",
                Name = "Saving Throws",
                ContentMarkdown = """
Saving Throws represent attempts to resist harmful effects.

**Roll:**
- d20 + appropriate saving throw modifier

Common uses:
- Dodging traps
- Resisting magic
- Enduring poisons
"""
            },

            new Rule
            {
                Category = "Core Mechanics",
                Name = "Advantage & Disadvantage",
                ContentMarkdown = """
When you have **Advantage**:
- Roll 2d20 and take the higher result.

When you have **Disadvantage**:
- Roll 2d20 and take the lower result.

Advantages never stack — only one applies at a time.
"""
            },
            new Rule
            {
                Category = "Combat",
                Name = "Initiative",
                ContentMarkdown = """
Combat begins by determining initiative order.

**Roll:**
- d20 + Dexterity modifier

Highest result acts first.
"""
            },

            new Rule
            {
                Category = "Combat",
                Name = "Attack Rolls",
                ContentMarkdown = """
To hit a target:

**Roll:**
- d20 + attack modifier

**Hit if:**
- Result ≥ target AC

**Critical hit:**
- Natural 20
- Double damage dice.
"""
            },

            new Rule
            {
                Category = "Combat",
                Name = "Damage",
                ContentMarkdown = """
After a hit, deal damage:

- Roll weapon or spell damage dice
- Add relevant modifiers

Damage reduces **Hit Points (HP)**.
"""
            },

            new Rule
            {
                Category = "Combat",
                Name = "Conditions",
                ContentMarkdown = """
Some common conditions:

- **Blinded** – Disadvantage on attacks.
- **Poisoned** – Disadvantage on ability checks.
- **Paralyzed** – Auto-fail STR/DEX saves.
"""
            },
            new Rule
            {
                Category = "Magic",
                Name = "Spell Slots",
                ContentMarkdown = """
Spellcasters use **Spell Slots** to cast leveled spells.

- Slots refresh on Long Rest
- Higher level slots can be used to upcast spells.
"""
            },

            new Rule
            {
                Category = "Magic",
                Name = "Spell Saving Throws",
                ContentMarkdown = """
Some spells require targets to make Saving Throws.

**DC formula:**

8 + Spellcasting Modifier + Proficiency Bonus
"""
            },

            new Rule
            {
                Category = "Magic",
                Name = "Concentration",
                ContentMarkdown = """
A caster may only concentrate on **one spell at a time**.

Concentration ends when:
- The caster casts another concentration spell.
- The caster takes damage and fails a CON save.
"""
            },
            new Rule
            {
                Category = "Rest",
                Name = "Short Rest",
                ContentMarkdown = """
A Short Rest lasts 1 hour.

Allows:
- Spending Hit Dice to heal.
- Recovery of some class features.
"""
            },

            new Rule
            {
                Category = "Rest",
                Name = "Long Rest",
                ContentMarkdown = """
A Long Rest lasts 8 hours.

Recovers:
- All HP
- All spell slots
- Half of spent Hit Dice.
"""
            },
            new Rule
            {
                Category = "Movement",
                Name = "Dash",
                ContentMarkdown = """
Dash doubles your movement for the current turn.
"""
            },

            new Rule
            {
                Category = "Movement",
                Name = "Opportunity Attacks",
                ContentMarkdown = """
Leaving an enemy's melee reach provokes an opportunity attack.

Uses **Reaction**.
"""
            },
            new Rule
            {
                Category = "Death",
                Name = "Dropping to 0 HP",
                ContentMarkdown = """
At 0 HP:

- You fall **Unconscious**
- Begin making **Death Saves**.
"""
            },

            new Rule
            {
                Category = "Death",
                Name = "Death Saving Throws",
                ContentMarkdown = """
Roll a d20 at the start of each turn:

- 10+ = success
- <10 = failure

3 successes → stabilized  
3 failures → death
"""
            },
        };
        
        await db.Rules.AddRangeAsync(rules);
        await db.SaveChangesAsync();
    }
}
