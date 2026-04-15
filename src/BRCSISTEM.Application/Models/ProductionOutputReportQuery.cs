namespace BRCSISTEM.Application.Models
{
    public sealed class ProductionOutputReportQuery
    {
        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public string OutputNumber { get; set; }

        public string WarehouseCode { get; set; }

        public string ProductCode { get; set; }

        public string Shift { get; set; }

        public string UserName { get; set; }

        public bool ExcludeCanceled { get; set; }
    }
}
