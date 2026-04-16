using System.Collections.Generic;

namespace BRCSISTEM.Domain.Models
{
    public sealed class DuplicateNoteMovementGroup
    {
        public string NoteNumber { get; set; }

        public string Supplier { get; set; }

        public string Reason { get; set; }

        public List<long> DuplicateMovementIds { get; set; } = new List<long>();

        public int TotalDuplicates => DuplicateMovementIds?.Count ?? 0;

        public string DisplayLabel => $"{NoteNumber} / {Supplier}";
    }
}
