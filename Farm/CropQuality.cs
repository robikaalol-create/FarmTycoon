namespace FarmTycoon.Farm
{
    public enum CropQuality
    {
        Low, Normal, Good, Premium, Excellent
    }

    public static class CropQualityHelper
    {
        public static float GetPriceMultiplier(CropQuality quality) => quality switch
        {
            CropQuality.Low => 0.7f,
            CropQuality.Normal => 1.0f,
            CropQuality.Good => 1.2f,
            CropQuality.Premium => 1.5f,
            CropQuality.Excellent => 2.0f,
            _ => 1.0f
        };

        public static string GetDisplayName(CropQuality quality) => quality switch
        {
            CropQuality.Low => "Alacsony",
            CropQuality.Normal => "Normál",
            CropQuality.Good => "Jó",
            CropQuality.Premium => "Prémium",
            CropQuality.Excellent => "Kiváló",
            _ => "Ismeretlen"
        };
    }
}
