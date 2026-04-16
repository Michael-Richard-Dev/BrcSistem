using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Views
{
    internal static class InventoryPdfReportPdfExporter
    {
        private const int PageWidth = 842;
        private const int PageHeight = 595;
        private const int Margin = 36;
        private const int BodyFontSize = 8;
        private const int TitleFontSize = 13;
        private const int LineHeight = 11;

        public static void Export(string filePath, string[] filterLines, InventoryReportDocument document, string userDisplayName)
        {
            if (document == null)
            {
                throw new InvalidOperationException("Documento nao informado para geracao do PDF.");
            }

            var pages = BuildPages(filterLines ?? Array.Empty<string>(), document, userDisplayName ?? string.Empty);
            WritePdf(filePath, pages);
        }

        private static IReadOnlyList<string[]> BuildPages(string[] filterLines, InventoryReportDocument document, string userDisplayName)
        {
            var allLines = new List<string>
            {
                "BRCSISTEM - RELATORIO DE INVENTARIO",
                "Gerado em: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm", CultureInfo.GetCultureInfo("pt-BR")),
                string.Empty,
                "INVENTARIO: " + NormalizeAscii(document.Number) + " | STATUS: " + NormalizeAscii(document.Status),
                "CRIADOR: " + NormalizeAscii(document.CreatedBy) + " | MAX PONTOS: " + document.MaxOpenPoints.ToString(CultureInfo.InvariantCulture),
                "ABERTURA: " + NormalizeAscii(document.OpenedAtDisplay) + " | FECHAMENTO: " + NormalizeAscii(document.ClosedAtDisplay),
                "FINALIZACAO: " + NormalizeAscii(document.FinalizedAtDisplay),
                "OBSERVACAO: " + NormalizeAscii(document.Observation),
                "MOTIVO CANCELAMENTO: " + NormalizeAscii(document.CancellationReason),
                string.Empty,
            };

            allLines.AddRange(filterLines.Select(NormalizeAscii));
            allLines.Add(string.Empty);
            allLines.Add("Itens: " + (document.Items ?? Array.Empty<InventoryReportItem>()).Length
                + " | Divergentes: " + document.DivergentItemCount
                + " | Entradas: " + document.TotalEntryAdjustmentsText
                + " | Saidas: " + document.TotalOutputAdjustmentsText);
            allLines.Add(string.Empty);
            allLines.Add("ITENS COM DIVERGENCIA");
            if (document.DivergentItemCount == 0)
            {
                allLines.Add("Nao foram encontradas divergencias para este inventario.");
            }
            else
            {
                allLines.Add(Pad("Almox", 8) + Pad("Material", 28) + Pad("Lote", 24) + PadLeft("Sistema", 12) + PadLeft("Contado", 12) + PadLeft("Diverg.", 12) + Pad("Tipo", 14) + Pad("Validade", 12));
                allLines.Add(new string('-', 122));
                foreach (var item in document.DivergentItems)
                {
                    allLines.Add(
                        Pad(item.WarehouseCode, 8)
                        + Pad(item.MaterialDisplay, 28)
                        + Pad(item.LotDisplay, 24)
                        + PadLeft(item.SystemBalanceText, 12)
                        + PadLeft(item.CountedQuantityText, 12)
                        + PadLeft(item.AdjustmentQuantityText, 12)
                        + Pad(item.AdjustmentType, 14)
                        + Pad(item.ExpirationDateDisplay, 12));
                }
            }

            allLines.Add(string.Empty);
            allLines.Add("MOVIMENTOS DE AJUSTE LANCADOS");
            if ((document.Movements ?? Array.Empty<InventoryReportMovement>()).Length == 0)
            {
                allLines.Add("Nenhum movimento de ajuste encontrado para este inventario.");
            }
            else
            {
                allLines.Add(Pad("Data/Hora", 17) + Pad("Item", 6) + Pad("Tipo", 14) + Pad("Almox", 8) + Pad("Material", 12) + Pad("Lote", 12) + PadLeft("Qtd", 10) + Pad("Forn.", 12) + Pad("Usuario", 14) + Pad("Status", 10));
                allLines.Add(new string('-', 115));
                foreach (var movement in document.Movements ?? Array.Empty<InventoryReportMovement>())
                {
                    allLines.Add(
                        Pad(movement.MovementDateDisplay, 17)
                        + Pad(movement.ItemNumberText, 6)
                        + Pad(movement.Type, 14)
                        + Pad(movement.WarehouseCode, 8)
                        + Pad(movement.MaterialCode, 12)
                        + Pad(movement.LotCode, 12)
                        + PadLeft(movement.QuantityText, 10)
                        + Pad(movement.SupplierCode, 12)
                        + Pad(movement.UserName, 14)
                        + Pad(movement.Status, 10));
                }
            }

            allLines.Add(string.Empty);
            allLines.Add("Usuario: " + NormalizeAscii(userDisplayName));

            var pages = new List<string[]>();
            var currentPage = new List<string>();
            var maxLinesPerPage = (PageHeight - (Margin * 2)) / LineHeight - 1;
            foreach (var line in allLines)
            {
                currentPage.Add(line);
                if (currentPage.Count >= maxLinesPerPage)
                {
                    pages.Add(currentPage.ToArray());
                    currentPage = new List<string>();
                }
            }

            if (currentPage.Count > 0)
            {
                pages.Add(currentPage.ToArray());
            }

            return pages;
        }

        private static string Pad(string value, int width)
        {
            var normalized = NormalizeAscii(value);
            if (normalized.Length > width)
            {
                return normalized.Substring(0, width);
            }

            return normalized.PadRight(width);
        }

        private static string PadLeft(string value, int width)
        {
            var normalized = NormalizeAscii(value);
            if (normalized.Length > width)
            {
                return normalized.Substring(0, width);
            }

            return normalized.PadLeft(width);
        }

        private static string NormalizeAscii(string value)
        {
            var normalized = (value ?? string.Empty).Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder(normalized.Length);
            foreach (var character in normalized)
            {
                var category = CharUnicodeInfo.GetUnicodeCategory(character);
                if (category == UnicodeCategory.NonSpacingMark)
                {
                    continue;
                }

                builder.Append(character > 127 ? '?' : character);
            }

            return builder.ToString();
        }

        private static void WritePdf(string filePath, IReadOnlyList<string[]> pages)
        {
            var objects = new List<string>();
            objects.Add("<< /Type /Catalog /Pages 2 0 R >>");

            var pageObjectNumbers = new List<int>();
            var contentObjectNumbers = new List<int>();
            objects.Add(string.Empty);
            objects.Add("<< /Type /Font /Subtype /Type1 /BaseFont /Courier >>");

            foreach (var pageLines in pages)
            {
                var pageObjectNumber = objects.Count + 1;
                pageObjectNumbers.Add(pageObjectNumber);
                objects.Add(string.Empty);

                var contentObjectNumber = objects.Count + 1;
                contentObjectNumbers.Add(contentObjectNumber);
                objects.Add(BuildContentObject(pageLines));
            }

            objects[1] = "<< /Type /Pages /Count " + pageObjectNumbers.Count + " /Kids [ " + string.Join(" ", pageObjectNumbers.Select(number => number + " 0 R")) + " ] >>";
            for (var index = 0; index < pageObjectNumbers.Count; index++)
            {
                var pageObjectIndex = pageObjectNumbers[index] - 1;
                objects[pageObjectIndex] = "<< /Type /Page /Parent 2 0 R /MediaBox [0 0 " + PageWidth + " " + PageHeight + "] /Resources << /Font << /F1 3 0 R >> >> /Contents " + contentObjectNumbers[index] + " 0 R >>";
            }

            var builder = new StringBuilder();
            builder.AppendLine("%PDF-1.4");
            var xrefPositions = new List<int> { 0 };
            for (var index = 0; index < objects.Count; index++)
            {
                xrefPositions.Add(builder.Length);
                builder.Append(index + 1).AppendLine(" 0 obj");
                builder.Append(objects[index]).AppendLine();
                builder.AppendLine("endobj");
            }

            var xrefStart = builder.Length;
            builder.AppendLine("xref");
            builder.Append("0 ").Append(objects.Count + 1).AppendLine();
            builder.AppendLine("0000000000 65535 f ");
            foreach (var position in xrefPositions.Skip(1))
            {
                builder.Append(position.ToString("0000000000")).AppendLine(" 00000 n ");
            }

            builder.AppendLine("trailer");
            builder.Append("<< /Size ").Append(objects.Count + 1).Append(" /Root 1 0 R >>").AppendLine();
            builder.AppendLine("startxref");
            builder.AppendLine(xrefStart.ToString(CultureInfo.InvariantCulture));
            builder.AppendLine("%%EOF");

            File.WriteAllBytes(filePath, Encoding.ASCII.GetBytes(builder.ToString()));
        }

        private static string BuildContentObject(string[] lines)
        {
            var content = new StringBuilder();
            content.AppendLine("BT");
            content.AppendLine("/F1 " + TitleFontSize + " Tf");
            content.AppendLine(Margin + " " + (PageHeight - Margin) + " Td");

            var first = true;
            var yOffset = 0;
            foreach (var rawLine in lines)
            {
                var line = EscapePdfText(rawLine);
                if (first)
                {
                    content.Append("(").Append(line).AppendLine(") Tj");
                    content.AppendLine("/F1 " + BodyFontSize + " Tf");
                    first = false;
                    yOffset = LineHeight + 6;
                    continue;
                }

                content.Append("0 -").Append(yOffset).AppendLine(" Td");
                content.Append("(").Append(line).AppendLine(") Tj");
                yOffset = LineHeight;
            }

            content.AppendLine("ET");
            var stream = content.ToString();
            return "<< /Length " + Encoding.ASCII.GetByteCount(stream) + " >>\nstream\n" + stream + "endstream";
        }

        private static string EscapePdfText(string value)
        {
            return NormalizeAscii(value)
                .Replace("\\", "\\\\")
                .Replace("(", "\\(")
                .Replace(")", "\\)");
        }
    }
}
