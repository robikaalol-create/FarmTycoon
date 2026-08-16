using System;

namespace FarmTycoon.Animals
{
    public class AnimalGenetics
    {
        public float ProductivityGene { get; set; }
        public float DiseaseResistance { get; set; }
        public float GrowthRate { get; set; }
        public float Fertility { get; set; }
        public float Longevity { get; set; }
        public float StressResistance { get; set; }

        private static Random _random = new Random();

        public AnimalGenetics()
        {
            ProductivityGene = 0.3f + (float)_random.NextDouble() * 0.4f;
            DiseaseResistance = 0.3f + (float)_random.NextDouble() * 0.4f;
            GrowthRate = 0.3f + (float)_random.NextDouble() * 0.4f;
            Fertility = 0.3f + (float)_random.NextDouble() * 0.4f;
            Longevity = 0.3f + (float)_random.NextDouble() * 0.4f;
            StressResistance = 0.3f + (float)_random.NextDouble() * 0.4f;
        }

        public static AnimalGenetics Breed(AnimalGenetics parent1, AnimalGenetics parent2)
        {
            var child = new AnimalGenetics();
            child.ProductivityGene = MixGene(parent1.ProductivityGene, parent2.ProductivityGene);
            child.DiseaseResistance = MixGene(parent1.DiseaseResistance, parent2.DiseaseResistance);
            child.GrowthRate = MixGene(parent1.GrowthRate, parent2.GrowthRate);
            child.Fertility = MixGene(parent1.Fertility, parent2.Fertility);
            child.Longevity = MixGene(parent1.Longevity, parent2.Longevity);
            child.StressResistance = MixGene(parent1.StressResistance, parent2.StressResistance);
            return child;
        }

        private static float MixGene(float gene1, float gene2)
        {
            float baseValue = (gene1 + gene2) / 2f;
            float mutation = ((float)_random.NextDouble() - 0.5f) * 0.2f;
            return Clamp(baseValue + mutation, 0.05f, 0.95f);
        }

        private static float Clamp(float value, float min, float max) => value < min ? min : (value > max ? max : value);
    }
}
