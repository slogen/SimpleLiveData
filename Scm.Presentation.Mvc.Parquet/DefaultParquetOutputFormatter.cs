using System.Collections.Generic;

namespace Scm.Presentation.Mvc.Parquet
{
    public class DefaultParquetOutputFormatter: AbstractParquetOutputFormatter {
        public static IEnumerable<string> SupportedMediaTypeList { get; } = new[]{
            "application/parquet",
            "application/octet-stream+parquet"
        };

        public static DefaultParquetOutputFormatter Default = new DefaultParquetOutputFormatter();
        public DefaultParquetOutputFormatter()
        {
            foreach (var mediatype in SupportedMediaTypeList)
                SupportedMediaTypes.Add(mediatype);
        }
    }
}
