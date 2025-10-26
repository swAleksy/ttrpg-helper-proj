using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TtrpgHelperBackend.Migrations
{
    /// <inheritdoc />
    public partial class _3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Classes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "Classes",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "A fierce warrior of primal strength and rage.", "Barbarian" },
                    { 2, "A charismatic performer and jack-of-all-trades using song and magic.", "Bard" },
                    { 3, "A divine spellcaster and healer, empowered by a deity or faith.", "Cleric" },
                    { 4, "A master of nature, able to shapeshift and call upon natural powers.", "Druid" },
                    { 5, "A skilled and versatile warrior trained in weapons and armour.", "Fighter" },
                    { 6, "A martial artist using ki, speed, and precision in combat.", "Monk" },
                    { 7, "A holy warrior bound by oath, wielding divine power and martial might.", "Paladin" },
                    { 8, "A wilderness scout, expert with ranged weapons and nature’s allies.", "Ranger" },
                    { 9, "A stealthy opportunist, skilled in infiltration, tricks and precision attacks.", "Rogue" },
                    { 10, "A spontaneous arcane caster whose magic comes from innate power.", "Sorcerer" },
                    { 11, "A spellcaster who has made a pact with a powerful entity.", "Warlock" },
                    { 12, "A studious arcane caster whose power comes from rigorous training and knowledge.", "Wizard" }
                });

            migrationBuilder.InsertData(
                table: "Races",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Versatile and adaptable, humans receive a bonus to all abilities.", "Human" },
                    { 2, "Graceful and long-lived, elves are dexterous and attuned to magic.", "Elf" },
                    { 3, "Stout and hardy, dwarves are resilient in combat and skilled craftsmen.", "Dwarf" },
                    { 4, "Small and nimble, halflings are lucky and quick in tricky situations.", "Halfling" },
                    { 5, "A blend of human and elven heritage, charismatic and versatile.", "Half-Elf" },
                    { 6, "Strong and ferocious, half-orcs have orcish blood and fierce instincts.", "Half-Orc" },
                    { 7, "Small in stature and quick of mind, gnomes excel in intelligence and cunning.", "Gnome" },
                    { 8, "Marked by infernal heritage, tieflings wield otherworldly power and charisma.", "Tiefling" },
                    { 9, "Dark elves of the Underdark, with keen senses and shadow-affinities.", "Drow" },
                    { 10, "Warrior-bred astral-plane beings, fierce in combat and psionically gifted.", "Githyanki" },
                    { 11, "Draconic-bodied humanoids, born of dragon-ancestors, with breath weapons.", "Dragonborn" }
                });

            migrationBuilder.InsertData(
                table: "Skills",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Balance, tumble, avoid falling or being shoved.", "Acrobatics" },
                    { 2, "Interact, calm, or command animals.", "Animal Handling" },
                    { 3, "Knowledge of magic, magical effects and items.", "Arcana" },
                    { 4, "Climb, swim, jump, and physically struggle.", "Athletics" },
                    { 5, "Lie convincingly, deceive others.", "Deception" },
                    { 6, "Recall lore about past events, places, people.", "History" },
                    { 7, "Sense motives, detect lies, read people.", "Insight" },
                    { 8, "Coerce or bully others through fear or strength.", "Intimidation" },
                    { 9, "Examine, search, deduce hidden clues and details.", "Investigation" },
                    { 10, "Treat injuries, diagnose illness, apply healing.", "Medicine" },
                    { 11, "Understand flora, fauna, natural environment.", "Nature" },
                    { 12, "Notice hidden things, traps, secret doors, distant sounds.", "Perception" },
                    { 13, "Entertain or impress through music, acting, or oration.", "Performance" },
                    { 14, "Convince or influence others socially.", "Persuasion" },
                    { 15, "Knowledge of deities, religious rites, sacred things.", "Religion" },
                    { 16, "Pickpocket, manipulate objects subtly, perform tricks.", "Sleight of Hand" },
                    { 17, "Move silently, hide, sneak past detection.", "Stealth" },
                    { 18, "Track, forage, endure wilderness, navigate terrain.", "Survival" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Races",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Races",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Races",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Races",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Races",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Races",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Races",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Races",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Races",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Races",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Races",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Classes");
        }
    }
}
