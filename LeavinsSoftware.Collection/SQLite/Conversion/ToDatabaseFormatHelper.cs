// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System;
using System.Collections.Generic;
using System.Globalization;

namespace LeavinsSoftware.Collection.SQLite.Conversion
{
    public sealed class ToDatabaseFormatHelper
    {
        private const string DbDataTimeFormat = "yyyyMMdd-HH:mm:ss";

        private static readonly Lazy<ToDatabaseFormatHelper> singleton =
            new Lazy<ToDatabaseFormatHelper>(() => new ToDatabaseFormatHelper());

        public static ToDatabaseFormatHelper Instance
        {
            get { return singleton.Value; }
        }

        private ToDatabaseFormatHelper()
        {
        }

        /// <summary>
        /// Returns the database representation of the given <see cref="bool" /> value.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public object BoolToDatabase(bool input)
        {
            return input ? 1 : 0;
        }

        /// <summary>
        /// Returns the database representation of the given <see cref="DateTime" /> value.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public object DateTimeToDatabase(DateTime input)
        {
            if (input.Kind == DateTimeKind.Utc)
            {
                return input.ToString(DbDataTimeFormat, CultureInfo.InvariantCulture);
            }
            else // 'Local' times can have an Unspecified Kind.
            {
                return TimeZoneInfo.ConvertTimeToUtc(input, TimeZoneInfo.Local)
                    .ToString(DbDataTimeFormat, CultureInfo.InvariantCulture);
            }
        }

        public object NullableDateTimeToDatabase(DateTime? input)
        {
            if (input.HasValue)
            {
                return DateTimeToDatabase(input.Value);
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Returns the database representation of the given <see cref="decimal" /> value.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public object DecimalToDatabase(decimal input)
        {
            return input.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts a nullable decimal to its database representation.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public object NullableDecimalToDatabase(Nullable<decimal> input)
        {
            return input == null ? "" : (input.Value).ToString(CultureInfo.InvariantCulture);
        }
    }
}
