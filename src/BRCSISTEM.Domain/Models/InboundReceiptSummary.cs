using System;
using System.Globalization;

namespace BRCSISTEM.Domain.Models
{
    public sealed class InboundReceiptSummary
    {
        public string Numero_Nota { get; set; }

        public string Cod_Fornecedor { get; set; }

        public string Fornecedor { get; set; }
        public string Cod_Almoxarifado { get; set; }

        public string Almoxarifado  { get; set; }

        public string Data_Hora_Movimento { get; set; }

        public string Status { get; set; }

        public int Versao { get; set; }

        public string Bloqueado_Por { get; set; }

        /*public string SupplierDisplay
        {
            get
            {
                if (string.IsNullOrWhiteSpace(CodigoFornecedor))
                {
                    return NomeFornecedor ?? string.Empty;
                }

                return string.IsNullOrWhiteSpace(NomeFornecedor)
                    ? CodigoFornecedor
                    : CodigoFornecedor + " - " + NomeFornecedor;
            }
        }

        public string WarehouseDisplay
        {
            get
            {
                if (string.IsNullOrWhiteSpace(CodigoAlmoxarifado))
                {
                    return NomeAlmoxarifado ?? string.Empty;
                }

                return string.IsNullOrWhiteSpace(NomeAlmoxarifado)
                    ? CodigoAlmoxarifado
                    : CodigoAlmoxarifado + " - " + NomeAlmoxarifado;
            }
        }

        public string MovementDateTimeDisplay
        {
            get
            {
                if (string.IsNullOrWhiteSpace(DataHoraMovimento))
                {
                    return string.Empty;
                }

                DateTime parsed;
                var formats = new[]
                {
                    "yyyy-MM-dd HH:mm:ss",
                    "yyyy-MM-dd HH:mm",
                    "dd/MM/yyyy HH:mm",
                    "dd/MM/yyyy"
                };

                if (DateTime.TryParseExact(DataHoraMovimento, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
                {
                    return parsed.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                }

                return DataHoraMovimento;
            }
        }*/
    }
}
