using GalaSoft.MvvmLight;

namespace SNROI.Models
{
    public class Material : ObservableObject
    {
        public string Name { get; set; } = "Material";
        public double CostPerWeightUnit { get; set; } = 0.00;
        public double TotalMonthlyPurchasedWeight { get; set; } = 1;
    }
}
