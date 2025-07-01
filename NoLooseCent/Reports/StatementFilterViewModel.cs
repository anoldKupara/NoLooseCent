using Microsoft.AspNetCore.Mvc.Rendering;

namespace NoLooseCent.Reports
{
    public class StatementFilterViewModel
    {
        public int SelectedCurrencyId { get; set; }
        public int SelectedYear { get; set; }
        public int SelectedMonth { get; set; }

        public IEnumerable<SelectListItem> Currencies { get; set; } = [];
        public IEnumerable<SelectListItem> Months { get; set; } = [];
    }
}