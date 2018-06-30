using GalaSoft.MvvmLight;

namespace SNROI.Models
{
    public class CultureCurrencyPair : ObservableObject
    {
        public string Country { get; set; }
        public string CultureCode { get; set; }
    }
}
