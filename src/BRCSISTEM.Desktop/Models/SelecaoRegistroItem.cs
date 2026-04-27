using BRCSISTEM.Desktop.Views;

namespace BRCSISTEM.Desktop.Models
{
    /// <summary>
    /// Modelo de linha exibido pela tela <see cref="SelecaoRegistroForm"/>.
    /// Representa uma opcao selecionavel ja preparada para exibicao no grid,
    /// mantendo referencia para a opcao original (<see cref="LookupOption"/>)
    /// para que a View possa devolver o objeto esperado pelo chamador.
    /// </summary>
    internal sealed class SelecaoRegistroItem
    {
        public string Codigo { get; set; }

        public string Descricao { get; set; }

        public string Status { get; set; }

        public LookupOption OpcaoOriginal { get; set; }
    }
}
