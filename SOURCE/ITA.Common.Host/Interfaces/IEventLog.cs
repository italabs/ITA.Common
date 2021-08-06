using ITA.Common.Host.Interfaces;

namespace ITA.Common.Host.Interfaces
{
    public interface IEventLog
    {
        void AddCategory(string Component, short CatId);

        void RemoveCategory(string Component);

        short GetCategory(string Component);

        string Log { get; set; }

        string Source { get; set; }

        void WriteEntry(string message, EEventType eventType, int id, short category);

        void WriteEntry(string message, EEventType eventType);

        void BeginInit();

        void EndInit();

    }
}
