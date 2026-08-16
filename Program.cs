using System;
using System.Linq;
using FarmTycoon.Animals;
using FarmTycoon.Buildings;
using FarmTycoon.Core;
using FarmTycoon.Data;
using FarmTycoon.Machines;
using FarmTycoon.Processing;
using FarmTycoon.Workers;

namespace FarmTycoon
{
    class Program
    {
        static void Main(string[] args)
        {
            var game = GameManager.Instance;
            game.Initialize();

            game.FarmManager.PlantCropOnParcel("land_001", "wheat");
            Console.WriteLine("Búza vetve.");

            Console.WriteLine("\n=== Alkalmazottak ===");
            game.WorkerManager.HireWorker("János", WorkerType.FarmWorker, 1600f);

            Console.WriteLine("\n=== Feldolgozóüzem ===");
            var mill = game.ProcessingManager.CreateFacility(
                "mill_001", "Malom", BuildingCategory.Production, 5000f);

            if (mill != null)
            {
                var line = new ProductionLine("line1", "Őrlő sor 1", 200f);
                line.SetRecipe(game.ProcessingManager.GetRecipe("wheat_to_flour"));
                line.Start();
                mill.AddProductionLine(line);
                mill.AddInput("wheat", 500f);
                Console.WriteLine("Malom létrehozva, búza: 500kg");
            }

            Console.WriteLine("\n=== 10 nap szimulálása ===\n");
            for (int day = 0; day < 10; day++)
            {
                for (int hour = 0; hour < 24; hour++)
                    game.SimulateHour();

                float flour = 0f;
                float bran = 0f;
                game.ProcessingManager.GlobalInventory.TryGetValue("flour", out flour);
                game.ProcessingManager.GlobalInventory.TryGetValue("bran", out bran);

                Console.WriteLine($"Nap {game.TimeManager.CurrentDay}: " +
                    $"Liszt={flour:F0}kg, Korpa={bran:F0}kg, " +
                    $"Pénz={game.PlayerMoney:C}");
            }

            Console.WriteLine("\n=== Játék vége ===");
            Console.WriteLine($"Idő: {game.TimeManager.GetDateString()}");
            Console.WriteLine($"Pénz: {game.PlayerMoney:C}");
        }
    }
}
