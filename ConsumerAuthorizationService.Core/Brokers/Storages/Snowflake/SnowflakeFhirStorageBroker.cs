// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Snowflake.Data.Client;

namespace ConsumerAuthorizationService.Core.Brokers.Storages.Snowflake
{
    public partial class SnowflakeFhirStorageBroker : ISnowflakeFhirStorageBroker
    {
        private readonly IConfiguration configuration;
        private readonly string connectionString;
        private readonly string setSessionTag = "ALTER SESSION SET QUERY_TAG = 'LDS_FHIR_API'";

        public SnowflakeFhirStorageBroker(IConfiguration configuration)
        {
            this.configuration = configuration;

            this.connectionString = configuration
                .GetConnectionString(name: "LondonDataServices.SnowflakeFhirConnectionString") ?? string.Empty;
        }

        private List<T> SelectByNhsNumber<T>(
            string nhsNumber,
            string sql,
            System.Func<IDataReader, T> mapRow)
        {
            using IDbConnection connection = new SnowflakeDbConnection
            {
                ConnectionString = this.connectionString
            };

            connection.Open();

            using (IDbCommand tagCommand = connection.CreateCommand())
            {
                tagCommand.CommandText = this.setSessionTag;
                tagCommand.ExecuteNonQuery();
            }

            using IDbCommand command = connection.CreateCommand();
            command.CommandText = sql;

            IDbDataParameter parameter = command.CreateParameter();
            parameter.ParameterName = nameof(nhsNumber); // -> "nhsNumber"
            parameter.DbType = DbType.String;
            parameter.Value = nhsNumber;
            command.Parameters.Add(parameter);

            using IDataReader reader = command.ExecuteReader();

            var results = new List<T>();

            while (reader.Read())
            {
                results.Add(mapRow(reader));
            }

            return results;
        }

        private static string ReadVariantOrArrayAsString(IDataReader reader, string columnName)
        {
            int i = reader.GetOrdinal(columnName);

            if (reader.IsDBNull(i))
            {
                return null;
            }

            object value = reader.GetValue(i);
            return value?.ToString();
        }

        private static string ReadString(IDataReader reader, string columnName)
        {
            int i = reader.GetOrdinal(columnName);
            return reader.IsDBNull(i) ? null : reader.GetString(i);
        }

        private static bool ReadBool(IDataReader reader, string columnName)
        {
            int i = reader.GetOrdinal(columnName);
            return !reader.IsDBNull(i) && Convert.ToBoolean(reader.GetValue(i));
        }

        private static long ReadInt64(IDataReader reader, string columnName)
        {
            int i = reader.GetOrdinal(columnName);
            return reader.IsDBNull(i) ? 0L : Convert.ToInt64(reader.GetValue(i));
        }

        private static int ReadInt32(IDataReader reader, string columnName)
        {
            int i = reader.GetOrdinal(columnName);
            return reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader.GetValue(i));
        }

        private static DateTime ReadDateTime(IDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);

            if (reader.IsDBNull(ordinal))
            {
                return default;
            }

            object value = reader.GetValue(ordinal);

            return value switch
            {
                DateTime dateTime => dateTime,

                // ✅ Snowflake driver can return DateTimeOffset for DATE/TIMESTAMP columns
                DateTimeOffset dateTimeOffset => dateTimeOffset.UtcDateTime,

                // Optional: if anything ever comes back as DateOnly
                DateOnly dateOnly => dateOnly.ToDateTime(TimeOnly.MinValue),

                // Optional: if it comes back as text
                string text when DateTime.TryParse(
                    text,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                    out DateTime parsed) => parsed,

                // Fallback for other convertible types
                IConvertible convertible => Convert.ToDateTime(convertible, CultureInfo.InvariantCulture),

                _ => throw new InvalidCastException(
                    $"Column '{columnName}' value type '{value.GetType().FullName}' cannot be converted to DateTime.")
            };
        }
    }
}