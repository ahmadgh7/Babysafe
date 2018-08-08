using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BsPiClient
{
    interface IWeatherDataProvider
    {
        double GetTemperature();
        double GetHumidity();
    }
}
