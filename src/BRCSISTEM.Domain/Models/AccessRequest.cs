namespace BRCSISTEM.Domain.Models
{
    /// <summary>
    /// Porte fiel do registro da tabela solicitacoes_acesso (Python: views/gerenciar_acessos.py).
    /// </summary>
    public sealed class AccessRequest
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public string RequestedAt { get; set; }
        public string Status { get; set; }
        public string RespondedAt { get; set; }
        public string Responder { get; set; }
    }
}
