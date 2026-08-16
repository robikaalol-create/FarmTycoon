namespace FarmTycoon.Buildings
{
    public enum ConstructionPhase
    {
        Foundation, Structure, Walls, Roof, Mechanics, Completed
    }

    public static class ConstructionPhaseHelper
    {
        public static string GetDisplayName(ConstructionPhase phase) => phase switch
        {
            ConstructionPhase.Foundation => "Alapozás",
            ConstructionPhase.Structure => "Szerkezet",
            ConstructionPhase.Walls => "Falak",
            ConstructionPhase.Roof => "Tető",
            ConstructionPhase.Mechanics => "Gépészet",
            ConstructionPhase.Completed => "Kész",
            _ => "Ismeretlen"
        };

        public static ConstructionPhase GetNextPhase(ConstructionPhase current) => current switch
        {
            ConstructionPhase.Foundation => ConstructionPhase.Structure,
            ConstructionPhase.Structure => ConstructionPhase.Walls,
            ConstructionPhase.Walls => ConstructionPhase.Roof,
            ConstructionPhase.Roof => ConstructionPhase.Mechanics,
            ConstructionPhase.Mechanics => ConstructionPhase.Completed,
            _ => ConstructionPhase.Completed
        };
    }
}
