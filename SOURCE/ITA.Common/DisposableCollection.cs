using System;
using System.Collections.Generic;

namespace ITA.Common
{
    public class DisposableCollection<T> : List<T>, IDisposable where T : IDisposable
    {
        private bool _disposed;

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        #endregion

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                ForEach(item => item.Dispose());
            }

            _disposed = true;
        }
    }
}