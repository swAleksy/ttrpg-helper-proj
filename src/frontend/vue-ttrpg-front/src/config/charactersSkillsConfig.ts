export type Ability = "STR" | "DEX" | "CON" | "INT" | "WIS" | "CHA"

export const SKILL_ABILITY = {
  Athletics: "STR",
  Acrobatics: "DEX",
  "Sleight of Hand": "DEX",
  Stealth: "DEX",
  Arcana: "INT",
  History: "INT",
  Investigation: "INT",
  Nature: "INT",
  Religion: "INT",
  "Animal Handling": "WIS",
  Insight: "WIS",
  Medicine: "WIS",
  Perception: "WIS",
  Survival: "WIS",
  Deception: "CHA",
  Intimidation: "CHA",
  Performance: "CHA",
  Persuasion: "CHA",
} as const satisfies Record<string, Ability>

export type SkillName = keyof typeof SKILL_ABILITY

export const BACKGROUND_SKILLS: Record<string, SkillName[]> = {
  Acolyte: ["Insight", "Religion"],
  Criminal: ["Deception", "Stealth"],
  Entertainer: ["Acrobatics", "Performance"],
  Noble: ["History", "Persuasion"],
  Outlander: ["Athletics", "Survival"],
  Sailor: ["Athletics", "Perception"],
  Soldier: ["Athletics", "Intimidation"],
  Scholar: ["Arcana", "History"],
}

export const CLASS_SKILLS: Record<
  string,
  { pick: number; options: SkillName[] }
> = {
  Barbarian: { pick: 2, options: ["Animal Handling", "Athletics", "Intimidation", "Nature", "Perception", "Survival"] },
  Bard: { pick: 3, options: Object.keys(SKILL_ABILITY) as SkillName[] },
  Cleric: { pick: 2, options: ["History", "Insight", "Medicine", "Persuasion", "Religion"] },
  Druid: { pick: 2, options: ["Arcana", "Animal Handling", "Insight", "Medicine", "Nature", "Perception", "Religion", "Survival"] },
  Fighter: { pick: 2, options: ["Acrobatics", "Animal Handling", "Athletics", "History", "Insight", "Intimidation", "Perception", "Survival"] },
  Monk: { pick: 2, options: ["Acrobatics", "Athletics", "History", "Insight", "Religion", "Stealth"] },
  Paladin: { pick: 2, options: ["Athletics", "Insight", "Intimidation", "Medicine", "Persuasion", "Religion"] },
  Ranger: { pick: 3, options: ["Animal Handling", "Athletics", "Insight", "Investigation", "Nature", "Perception", "Stealth", "Survival"] },
  Rogue: { pick: 4, options: ["Acrobatics", "Athletics", "Deception", "Insight", "Intimidation", "Investigation", "Perception", "Performance", "Persuasion", "Sleight of Hand", "Stealth"] },
  Sorcerer: { pick: 2, options: ["Arcana", "Deception", "Insight", "Intimidation", "Persuasion", "Religion"] },
  Warlock: { pick: 2, options: ["Arcana", "Deception", "History", "Intimidation", "Investigation", "Nature", "Religion"] },
  Wizard: { pick: 2, options: ["Arcana", "History", "Insight", "Investigation", "Medicine", "Religion"] },
}