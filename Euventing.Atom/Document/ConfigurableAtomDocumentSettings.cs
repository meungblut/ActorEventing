namespace Eventing.Atom.Document
{
    public class ConfigurableAtomDocumentSettings : IAtomDocumentSettings
    {
        public ConfigurableAtomDocumentSettings(int numberOfEventsPerDocument)
        {
            NumberOfEventsPerDocument = numberOfEventsPerDocument;
        }

        public int NumberOfEventsPerDocument { get; }
    }
}
