using System.Collections.Generic;
using System.Linq;

namespace FarmTycoon.Farm
{
    public class CropRotationTracker
    {
        private Dictionary<string, List<string>> _history = new();

        public void RecordPlanting(string landId, string cropId)
        {
            if (!_history.ContainsKey(landId))
                _history[landId] = new List<string>();
            _history[landId].Insert(0, cropId);
            if (_history[landId].Count > 5)
                _history[landId].RemoveAt(5);
        }

        public float GetRotationBonus(string landId, string cropId)
        {
            if (!_history.ContainsKey(landId) || _history[landId].Count < 2)
                return 0f;

            int consecutiveSame = 0;
            foreach (var pastCrop in _history[landId])
            {
                if (pastCrop == cropId) consecutiveSame++;
                else break;
            }

            if (consecutiveSame >= 3) return -0.2f;
            if (consecutiveSame == 2) return -0.1f;

            var uniqueCrops = _history[landId].Distinct().Count();
            if (uniqueCrops >= 3) return 0.1f;
            return 0f;
        }

        public string GetPreviousCrop(string landId)
        {
            if (!_history.ContainsKey(landId) || _history[landId].Count < 2)
                return null;
            return _history[landId][1];
        }
    }
}
