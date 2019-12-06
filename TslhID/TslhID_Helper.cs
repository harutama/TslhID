using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Harutama.TimeShortLongHashID
{
    public partial class TslhID
    {



        private static byte[] DateTimeOffsetToByteArray(DateTimeOffset value)
        {
            if (value.ToUnixTimeMilliseconds() > MaxUnixMillSec)
            {
                throw new ArgumentException("Assigned datetime is overflow.");
            }

            var us = Convert.ToUInt64(value.ToUnixTimeMilliseconds() + UnixEpochMillSec);
            var mb = BitConverter.GetBytes(us);
            var retval = new[] { mb[5], mb[4], mb[3], mb[2], mb[1], mb[0] };

            return retval;
        }

        private static DateTimeOffset ByteArrayToDateTimeOffset(byte[] value, TimeSpan span)
        {
            ulong us = BitConverter.ToUInt64(new byte[] { value[5], value[4], value[3], value[2], value[1], value[0], 0, 0 }, 0);
            long tics = Convert.ToInt64(us) - UnixEpochMillSec;

            DateTimeOffset retval = DateTimeOffset.FromUnixTimeMilliseconds(tics);

            return retval;
        }

        private static byte[] ShortToByteArray(short value)
        {
            //#####ソートのことを考えて数値の型はunsignedにしていることに超絶注意#####
            ushort us = BitConverter.ToUInt16(BitConverter.GetBytes(value), 0);
            return BitConverter.GetBytes(us);
        }

        private static short ByteArrayToShort(byte[] value)
        {
            return BitConverter.ToInt16(value, 0);
        }

        private static byte[] LongToByteArray(long value)
        {
            //#####ソートのことを考えて数値の型はunsignedにしていることに超絶注意#####
            ulong us = BitConverter.ToUInt64(BitConverter.GetBytes(value), 0);
            return BitConverter.GetBytes(us);
        }

        private static long ByteArrayToLong(byte[] value)
        {
            return BitConverter.ToInt64(value, 0);
        }

        private static byte[] ConvertGuidAndSqlGuid(byte[] bytes)
        {
            //なんでわざわざこんな事をしているかというと↓これがいいかな?
            //https://blogs.msdn.microsoft.com/dbthisne/2012/07/03/how-to-generate-sequential-guids-for-sql-server-in-net/

            byte[] retval = new byte[16];

            retval[3] = bytes[0];
            retval[2] = bytes[1];
            retval[1] = bytes[2];
            retval[0] = bytes[3];
            retval[5] = bytes[4];
            retval[4] = bytes[5];
            retval[7] = bytes[6];
            retval[6] = bytes[7];
            retval[8] = bytes[8];
            retval[9] = bytes[9];
            retval[10] = bytes[10];
            retval[11] = bytes[11];
            retval[12] = bytes[12];
            retval[13] = bytes[13];
            retval[14] = bytes[14];
            retval[15] = bytes[15];

            return retval;
        }

        public override string ToString()
        {
            return this.AsGuid.ToString();
        }

        private byte[] ToByteArray(DateTimeOffset dt, short key, long hash)
        {
            byte[] retval = new byte[16];

            Array.Copy(LongToByteArray(hash), 0, retval, 0, 8);
            Array.Copy(ShortToByteArray(key), 0, retval, 8, 2);
            Array.Copy(DateTimeOffsetToByteArray(dt), 0, retval, 10, 6);

            return retval;
        }

        /// <summary>
        /// IComparableの実装
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(TslhID other)
        {
            return this.AsGuid.CompareTo(other.AsGuid);
        }

        /// <summary>
        /// IComparableの実装
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Guid other)
        {
            return this.AsGuid.CompareTo(other);
        }


    }
}
