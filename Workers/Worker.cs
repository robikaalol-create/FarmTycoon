using System;

namespace FarmTycoon.Workers
{
    public class Worker
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public WorkerType Type { get; private set; }
        public WorkerSkillLevel SkillLevel { get; private set; } = WorkerSkillLevel.Beginner;

        public float WorkSpeed { get; private set; }
        public float Accuracy { get; private set; }
        public float Reliability { get; private set; }
        public float Experience { get; private set; } = 0f;
        public float StressResistance { get; private set; }
        public float MachineSkill { get; private set; }
        public float AnimalSkill { get; private set; }

        public float Morale { get; private set; } = 0.7f;
        public float BaseSalary { get; private set; }
        public float CurrentSalary { get; private set; }
        public bool IsOverworked { get; private set; } = false;
        public int DaysEmployed { get; private set; } = 0;
        public bool IsAvailable { get; set; } = true;

        public WorkShift AssignedShift { get; set; } = WorkShift.Day;

        private Random _random = new Random();

        public Worker(string id, string name, WorkerType type, float baseSalary)
        {
            Id = id;
            Name = name;
            Type = type;
            BaseSalary = baseSalary;
            CurrentSalary = baseSalary;

            WorkSpeed = 0.3f + (float)_random.NextDouble() * 0.5f;
            Accuracy = 0.3f + (float)_random.NextDouble() * 0.5f;
            Reliability = 0.4f + (float)_random.NextDouble() * 0.4f;
            StressResistance = 0.3f + (float)_random.NextDouble() * 0.5f;
            MachineSkill = 0.2f + (float)_random.NextDouble() * 0.5f;
            AnimalSkill = 0.2f + (float)_random.NextDouble() * 0.5f;
        }

        public void UpdateDaily()
        {
            DaysEmployed++;

            Experience += 0.5f * Reliability;
            if (Experience >= 100f)
            {
                Experience = 0f;
                Promote();
            }

            float moraleChange = 0f;
            if (IsOverworked) moraleChange -= 0.1f;
            if (CurrentSalary > BaseSalary * 1.2f) moraleChange += 0.05f;
            if (CurrentSalary < BaseSalary) moraleChange -= 0.1f;

            Morale += moraleChange;
            Morale = Clamp(Morale, 0f, 1f);

            if (Morale < 0.3f)
            {
                WorkSpeed *= 0.9f;
                Accuracy *= 0.9f;
            }
        }

        public void SetSalary(float newSalary)
        {
            CurrentSalary = newSalary;
        }

        public float CalculateSeverance()
        {
            return CurrentSalary * (1f + DaysEmployed / 30f * 0.5f);
        }

        public float GetPerformanceMultiplier()
        {
            float skillMultiplier = SkillLevel switch
            {
                WorkerSkillLevel.Beginner => 0.7f,
                WorkerSkillLevel.Experienced => 1.0f,
                WorkerSkillLevel.Expert => 1.3f,
                WorkerSkillLevel.Master => 1.6f,
                _ => 1.0f
            };

            return skillMultiplier * (0.5f + Morale * 0.5f) * WorkSpeed;
        }

        private void Promote()
        {
            SkillLevel = SkillLevel switch
            {
                WorkerSkillLevel.Beginner => WorkerSkillLevel.Experienced,
                WorkerSkillLevel.Experienced => WorkerSkillLevel.Expert,
                WorkerSkillLevel.Expert => WorkerSkillLevel.Master,
                _ => SkillLevel
            };
            BaseSalary *= 1.2f;
            CurrentSalary = BaseSalary;
        }

        private float Clamp(float v, float min, float max) => v < min ? min : (v > max ? max : v);
    }
}
