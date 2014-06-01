// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System;
using System.Collections.Generic;
using System.Globalization;

namespace LeavinsSoftware.Collection.SQLite.Conversion
{
    public sealed class FromDatabaseFormatHelper
    {
        private const string DbDataTimeFormat = "yyyyMMdd-HH:mm:ss";

        private static readonly Lazy<FromDatabaseFormatHelper> singleton =
            new Lazy<FromDatabaseFormatHelper>(() => new FromDatabaseFormatHelper());

        public static FromDatabaseFormatHelper Instance
        {
            get { return singleton.Value; }
        }

        private FromDatabaseFormatHelper()
        {
        }

        /// <summary>
        /// Converts a database representation of a <see cref="bool" /> value
        /// to a <see cref="bool" /> value.
        /// </summary>
        /// <param name="objectFromReader"></param>
        /// <returns></returns>
        public bool DatabaseToBool(object objectFromReader)
        {
            long? isDiscontinuedReaderValue = objectFromReader as long?;
            return !(isDiscontinuedReaderValue == null || isDiscontinuedReaderValue == 0);
        }

        /// <summary>
        /// Converts a database representation of a <see cref="DateTime" /> value
        /// to a <see cref="DateTime" /> value.
        /// </summary>
        /// <param name="objectFromReader"></param>
        /// <returns></returns>
        public DateTime DatabaseToDateTime(object objectFromReader)
        {
            string dateReaderValue = objectFromReader as string;

            if (string.IsNullOrWhiteSpace(dateReaderValue))
            {
                throw new ArgumentException("objectFromReader must be a string",
                                            "objectFromReader");
            }

            DateTime parsedDate = DateTime.ParseExact(dateReaderValue,
                                                      DbDataTimeFormat,
                                                      CultureInfo.InvariantCulture);

            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(parsedDate,
                                                                        DateTimeKind.Utc),
                                                   TimeZoneInfo.Local);
        }

        public DateTime? DatabaseToNullableDateTime(object objectFromReader)
        {
            string dateReaderValue = objectFromReader as string;

            if (string.IsNullOrWhiteSpace(dateReaderValue))
            {
                return null;
            }
            else
            {
                return DatabaseToDateTime(objectFromReader);
            }
        }

        /// <summary>
        /// Converts a database representation of a <see cref="decimal" /> value
        /// to a <see cref="decimal" /> value.
        /// </summary>
        /// <param name="objectFromReader"></param>
        /// <returns></returns>
        public decimal DatabaseToDecimal(object objectFromReader)
        {
            return decimal.Parse(objectFromReader as string, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts a database representation of a nullable decimal to a
        /// nullable decimal.
        /// </summary>
        /// <param name="objectFromReader"></param>
        /// <returns></returns>
        public Nullable<decimal> DatabaseToNullableDecimal(object objectFromReader)
        {
            if (string.IsNullOrEmpty(objectFromReader as string))
            {
                return null;
            }
            else
            {
                return DatabaseToDecimal(objectFromReader);
            }
        }
    }
}
