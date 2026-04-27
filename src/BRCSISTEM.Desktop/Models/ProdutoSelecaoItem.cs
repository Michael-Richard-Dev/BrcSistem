using BRCSISTEM.Desktop.Views;

namespace BRCSISTEM.Desktop.Models
{
    internal sealed class ProdutoSelecaoItem
    {
        public string Codigo { get; set; }
        public string Descricao { get; set; }
        public string Status { get; set; }
        public LookupOption OpcaoOriginal { get; set; }
    }
}
