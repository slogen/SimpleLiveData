using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Parquet;
using Parquet.Data;
using Scm.Sys;

namespace Scm.Presentation.Mvc.Parquet
{
    public static class DynamicParquet
    {
        public static CompressionMethod DefaultCompressionMethod { get; set; } = CompressionMethod.Snappy;
        public static int DefaultRowGroupSize { get; set; } = 5000;
        private static MethodInfo _parquetConvertSerialize;

        private static MethodInfo ParquetConvertSerialize =>
            _parquetConvertSerialize ?? (_parquetConvertSerialize = FindParquetConverSerialize());

        private static MethodInfo FindParquetConverSerialize()
        {

            var mis = typeof(ParquetConvert)
                .GetMethods(BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.Public)
                .ToList();
            var mis2 = mis
                    .Where(mi => mi.GetParameters().FirstOrDefault()?.ParameterType?.IsArray ?? false)
                    .ToList();
            var mis3 = mis2.Where(mi => mi.GetParameters().Skip(1).Select(x => x.ParameterType).SequenceEqual(new[]
                {typeof(Stream), typeof(Schema), typeof(CompressionMethod), typeof(int)})).ToList();
            return mis3.First();
        }

        public static Schema ParquetSerialize(
            this Array objectInstances, Stream destination,
            Schema schema = null,
            CompressionMethod? compressionMethod = null,
            int? rowGroupSize = null)
        {
            if (objectInstances == null)
                throw new ArgumentNullException(nameof(objectInstances));
            var elementType = objectInstances.GetType().GetElementType();
            var bakedMethodInfo = ParquetConvertSerialize.MakeGenericMethod(elementType);
            var result = bakedMethodInfo.Invoke(null,
                new object[]
                {
                    objectInstances, destination, schema, compressionMethod ?? DefaultCompressionMethod,
                    rowGroupSize ?? DefaultRowGroupSize
                });
            return (Schema) result;
        }

        public static async Task<Schema> ParquetSerializeAsync(
            this Array objectInstances, Stream destination,
            CancellationToken cancellationToken,
            Schema schema = null,
            CompressionMethod? compressionMethod = null,
            int? rowGroupSize = null)
        {
            return await Task.Factory
                .StartNew(() => objectInstances.ParquetSerialize(destination, schema, compressionMethod, rowGroupSize),
                    cancellationToken).ConfigureAwait(false);
        }

        public static int DefaultBufferSize { get; set; } = 81920;

        public static async Task<IList<T>> ParquetDeserializeAsync<T>(this Stream stream,
            CancellationToken cancellationToken,
            int? bufferSize = null)
            where T : new()
        {
            // ReSharper disable once AccessToDisposedClosure -- use is awaited
            using (var s = await stream.AsSeekableAsync(cancellationToken, bufferSize).ConfigureAwait(false))
                return await Task.Run(() => ParquetConvert.Deserialize<T>(s), cancellationToken).ConfigureAwait(false);
        }

        public static async Task<IList<T>> ParquetDeserialize<T>(this Task<Stream> stream,
            CancellationToken cancellationToken,
            int? bufferSize = null)
            where T : new()
            => await (await stream.ConfigureAwait(false)).ParquetDeserializeAsync<T>(cancellationToken, bufferSize)
                .ConfigureAwait(false);
    }
}