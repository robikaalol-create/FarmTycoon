using System.Collections.Generic;

namespace FarmTycoon.Buildings
{
    public class Site
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public string Region { get; private set; }
        public float TotalArea { get; private set; }
        public float UsedArea { get; private set; }
        public List<Building> Buildings { get; private set; } = new();
        public List<Infrastructure> Infrastructure { get; private set; } = new();

        public Site(string id, string name, string region, float totalArea)
        {
            Id = id; Name = name; Region = region; TotalArea = totalArea;
        }

        public bool HasSpaceFor(float footprint) => (UsedArea + footprint) <= TotalArea;

        public bool AddBuilding(Building building, float footprint)
        {
            if (!HasSpaceFor(footprint)) return false;
            Buildings.Add(building);
            UsedArea += footprint;
            return true;
        }

        public void AddInfrastructure(Infrastructure infra) => Infrastructure.Add(infra);
        public float GetFreeArea() => TotalArea - UsedArea;
    }
}
