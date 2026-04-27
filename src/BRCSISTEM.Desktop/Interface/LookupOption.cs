using System;

namespace BRCSISTEM.Desktop.Interface
{
    internal sealed class LookupOption
    {
        public string Code { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }

        public string DisplayText
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Code))
                {
                    return Description ?? string.Empty;
                }

                return string.IsNullOrWhiteSpace(Description)
                    ? Code
                    : Code + " - " + Description;
            }
        }

        public override string ToString()
        {
            return DisplayText;
        }
    }
}
