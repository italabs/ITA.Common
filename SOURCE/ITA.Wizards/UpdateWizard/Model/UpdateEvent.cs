using System;
using System.Data.Common;

namespace ITA.Wizards.UpdateWizard.Model
{
    /// <summary>
    /// Тип события, связанного с обновлением БД.
    /// </summary>
    public enum UpdateEventType
    {
        StepStarted,

        StepFinished,

        Finished,

        Failed
    }

    public delegate void UpdateEventHandler(object sender, UpdateEventArg arg);

    /// <summary>
    /// Событие обновления БД.
    /// </summary>
    public class UpdateEventArg : EventArgs
    {
        private readonly Exception _exception;

        private readonly int _progress;
        private readonly DatabaseUpdateStep _step;
        private readonly UpdateEventType _type;

        public UpdateEventArg(UpdateEventType type, DatabaseUpdateStep step)
        {
            _type = type;
            _step = step;
        }

        public UpdateEventArg(UpdateEventType type, DatabaseUpdateStep step, int progress)
            : this(type, step)
        {
            _progress = progress;
        }

        public UpdateEventArg(UpdateEventType type, DatabaseUpdateStep step, Exception exceptions) :
            this(type, step)
        {
            _exception = exceptions;
        }

        public UpdateEventType Type
        {
            get { return _type; }
        }

        public DatabaseUpdateStep Step
        {
            get { return _step; }
        }

        public int Progress
        {
            get { return _progress; }
        }

        public Exception Exception
        {
            get { return _exception; }
        }
    }

    public delegate void UpdateVersionEventHandler(object sender, UpdateVersionEventArg arg);

    public class UpdateVersionEventArg :EventArgs
    {
        private readonly DbTransaction _transaction;
        private readonly Version _from;
        private readonly Version _to;

        public UpdateVersionEventArg(DbTransaction transaction, Version from, Version to)
        {
            _transaction = transaction;
            _from = from;
            _to = to;
        }

        public DbTransaction Transaction
        {
            get { return _transaction; }
        }

        public Version From
        {
            get { return _from; }
        }

        public Version To
        {
            get { return _to; }
        }
    }
}