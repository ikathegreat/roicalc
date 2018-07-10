using GalaSoft.MvvmLight;

namespace SNROI.Models
{
    public class Material : ObservableObject
    {
        public string Name { get; set; }
        public double CostPerWeightUnit { get; set; }
        public double TotalMonthlyPurchasedWeight { get; set; }
    }
}
