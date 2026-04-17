namespace BRCSISTEM.Domain.Models
{
    /// <summary>
    /// Item retornado pelo diagnostico de inconsistencias Lote x Material
    /// (views/bd_inconsistencias_lote_material.py).
    /// Representa um lote com saldo no estoque cujo material registrado nos
    /// movimentos diverge do material cadastrado no lote.
    /// </summary>
    public sealed class LotMaterialInconsistencyEntry
    {
        public string Lot { get; set; }

        public string LotName { get; set; }

        /// <summary>Material como aparece nos movimentos de estoque (divergente).</summary>
        public string MovementMaterial { get; set; }

        public string MovementMaterialName { get; set; }

        /// <summary>Material como cadastrado no lote (referencia correta).</summary>
        public string RegisteredMaterial { get; set; }

        public string RegisteredMaterialName { get; set; }

        public string Warehouse { get; set; }

        public string WarehouseName { get; set; }

        public decimal Balance { get; set; }

        public string Validity { get; set; }

        public string SupplierName { get; set; }
    }
}
