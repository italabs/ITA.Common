using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Interfaces
{
    public interface IEngineServiceConfig
    {
        CultureInfo DefaultCulture { get; }
    }

}
