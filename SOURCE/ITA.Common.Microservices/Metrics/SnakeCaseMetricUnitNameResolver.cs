using System.Linq;
using System.Text;

namespace ITA.Common.Microservices.Metrics
{
    public class SnakeCaseMetricUnitNameResolver : IMetricUnitNameResolver
    {
        private static readonly char[] ReplacedSnakeCaseSymbols = {' ', '-'};
        
        #region Implementation of IMetricUnitNameResolver

        public virtual string Resolve(MetricUnitMetadata metadata)
        {
            return ConvertToSnakeCase(metadata.UnitName);
        }

        #endregion

        protected string ConvertToSnakeCase(string name)
        {
            return name
                .Select((symbol, index) => (symbol, index))
                .Aggregate(
                    new StringBuilder(), 
                    (builder, item) =>
                    {
                        if (ReplacedSnakeCaseSymbols.Contains(item.symbol))
                        {
                            builder.Append('_');
                            return builder;
                        }

                        if (!char.IsUpper(item.symbol))
                        {
                            builder.Append(item.symbol);
                            return builder;
                        }

                        if (item.index != 0)
                        {
                            builder.Append('_');
                        }
                        builder.Append(char.ToLower(item.symbol));

                        return builder;
                    }).ToString();
        }
    }
}