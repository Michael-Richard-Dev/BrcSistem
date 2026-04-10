namespace BRCSISTEM.Domain.Models
{
    public sealed class ModuleDefinition
    {
        public ModuleDefinition(string key, string group, string title, string requiredPermission, string pythonFile, string description, bool implemented)
        {
            Key = key;
            Group = group;
            Title = title;
            RequiredPermission = requiredPermission;
            PythonFile = pythonFile;
            Description = description;
            Implemented = implemented;
        }

        public string Key { get; private set; }

        public string Group { get; private set; }

        public string Title { get; private set; }

        public string RequiredPermission { get; private set; }

        public string PythonFile { get; private set; }

        public string Description { get; private set; }

        public bool Implemented { get; private set; }
    }
}
