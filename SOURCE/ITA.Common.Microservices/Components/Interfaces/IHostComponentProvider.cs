using System.Collections.Generic;

namespace ITA.Common.Microservices.Components
{
    public interface IHostComponentProvider
    {
        IEnumerable<IHostComponent> GetComponents();
    }
}