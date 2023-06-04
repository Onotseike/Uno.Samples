using System.Collections.Immutable;

using UnoCameraSupport.DataContracts;

namespace UnoCameraSupport.Services.Caching
{
	public interface IWeatherCache
	{
		ValueTask<IImmutableList<WeatherForecast>> GetForecast(CancellationToken token);
	}
}