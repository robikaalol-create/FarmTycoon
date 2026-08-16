using System.Collections.Generic;

namespace FarmTycoon.Utils
{
    public class BuildingConstructionStartedEvent : GameEvent
    {
        public string BuildingId { get; }
        public string BuildingName { get; }
        public float TotalCost { get; }

        public BuildingConstructionStartedEvent(string buildingId, string buildingName, float totalCost)
        {
            BuildingId = buildingId;
            BuildingName = buildingName;
            TotalCost = totalCost;
        }
    }

    public class BuildingConstructionCompletedEvent : GameEvent
    {
        public string BuildingId { get; }
        public string BuildingName { get; }

        public BuildingConstructionCompletedEvent(string buildingId, string buildingName)
        {
            BuildingId = buildingId;
            BuildingName = buildingName;
        }
    }

    public class BuildingUpgradedEvent : GameEvent
    {
        public string BuildingId { get; }
        public int NewLevel { get; }

        public BuildingUpgradedEvent(string buildingId, int newLevel)
        {
            BuildingId = buildingId;
            NewLevel = newLevel;
        }
    }

    public class BuildingDemolishedEvent : GameEvent
    {
        public string BuildingId { get; }
        public float RecoveredMaterials { get; }

        public BuildingDemolishedEvent(string buildingId, float recoveredMaterials)
        {
            BuildingId = buildingId;
            RecoveredMaterials = recoveredMaterials;
        }
    }

    public class InfrastructurePlacedEvent : GameEvent
    {
        public string InfrastructureId { get; }
        public string Type { get; }

        public InfrastructurePlacedEvent(string infrastructureId, string type)
        {
            InfrastructureId = infrastructureId;
            Type = type;
        }
    }
}
