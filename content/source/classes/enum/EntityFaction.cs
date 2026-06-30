namespace RPGFramework.Entities;

// Tipo/facção da entidade. Organiza Player, inimigos, NPCs e (futuro) pets.
// Cada Pawn se registra num grupo pela facção (ex: "enemy"), e a IA mira alvos por facção.
public enum EntityFaction
{
    Player,
    Enemy,
    NPC,
    Pet,
}
