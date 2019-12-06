using System;
using System.Collections.Generic;
using System.Text;

namespace Harutama.TimeShortLongHashID
{
    public partial class TslhID
    {

        private const long UnixEpochMillSec = 62135596800000;

        private const long MaxUnixMillSec = 281474976710655 - UnixEpochMillSec;

        /// <summary> Min value of TslhID. </summary>
        public static readonly TslhID MinValue = new TslhID(DateTimeOffset.FromUnixTimeMilliseconds(0), short.MinValue, long.MinValue);

        /// <summary> Max value of TslhID. </summary>
        public static readonly TslhID MaxValue = new TslhID(DateTimeOffset.FromUnixTimeMilliseconds(MaxUnixMillSec), short.MaxValue, long.MaxValue);

        /// <summary>
        /// Constructor to create min or max value.
        /// </summary>
        /// <param name="datetime">datetime</param>
        /// <param name="key">key value</param>
        /// <param name="hashValue">hash value</param>
        private TslhID(DateTimeOffset datetime, short key, long hashValue)
        {
            byte[] b = ToByteArray(datetime, key, hashValue);
            this.ArrayExpression = new ArraySegment<byte>(b);
        }

    }
}
