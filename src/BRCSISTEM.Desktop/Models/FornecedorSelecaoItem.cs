using BRCSISTEM.Desktop.Views;

namespace BRCSISTEM.Desktop.Models
{
    internal sealed class FornecedorSelecaoItem
    {
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public string Status { get; set; }
        public LookupOption OpcaoOriginal { get; set; }
    }
}
