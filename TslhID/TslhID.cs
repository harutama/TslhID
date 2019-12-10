using HashDepot;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Harutama.TimeShortLongHashID
{

    /// <summary>
    /// GUIDを再発明したもの。
    /// ミリ秒を保持する時間(Time)とユーザー指定の値(Short)とユーザーから指定された文字列をハッシュした結果(LongHash)がGUIDの形式で保持されます。
    /// </summary>
    public partial class TslhID : IComparable<TslhID>, IComparable<Guid>
    {
        //メモ
        //https://blogs.msdn.microsoft.com/sqlprogrammability/2006/11/06/how-are-guids-compared-in-sql-server-2005/
        //https://stackoverflow.com/questions/7810602/sql-server-guid-sort-algorithm-why

        private static readonly string HashSalt;

        private static readonly string PrefixIndex;

        private static readonly string SuffixIndex;

        /// <summary> Key for SipHash. </summary>
        private static readonly byte[] SipHashKey;


        /// <summary>
        /// 
        /// </summary>
        private readonly ArraySegment<Byte> ArrayExpression;

        /// <summary>
        /// このインスタンスのGUID表現
        /// </summary>
        public Guid AsGuid { get => new Guid(this.ArrayExpression.Array); }

        /// <summary>
        /// このインスタンスをSqlGuid表現で返します。
        /// SQL Server の uniqueidentifier 型でソートしたときに正しく順序付けられるようバイトオーダーが変更されます。
        /// </summary>
        /// <seealso cref="https://blogs.msdn.microsoft.com/dbthisne/2012/07/03/how-to-generate-sequential-guids-for-sql-server-in-net/"/>
        public SqlGuid AsSqlGuid { get => new SqlGuid(ConvertGuidAndSqlGuid(this.ArrayExpression.Array)); }

        /// <summary>
        /// 時間
        /// </summary>
        public DateTimeOffset Datetime { get => ByteArrayToDateTimeOffset(ArrayExpression.Skip(10).Take(6).ToArray(), TimeSpan.Zero); }

        /// <summary>
        /// ユーザーが指定したshort値
        /// </summary>
        public short UserKey { get => ByteArrayToShort(ArrayExpression.Skip(8).Take(2).ToArray()); }

        /// <summary>
        /// ユーザーが指定した文字列のハッシュ
        /// </summary>
        public long HashValue { get => ByteArrayToLong(ArrayExpression.Skip(0).Take(8).ToArray()); }


        static TslhID()
        {
            var configBuilder = new ConfigurationBuilder();
            configBuilder.SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
            configBuilder.AddJsonFile(@"Harutama.TslhID.Config.json");

            var config = configBuilder.Build();

            HashSalt = config["AppSettings:HashSalt"];
            PrefixIndex = config["AppSettings:PrefixIndex"];
            SuffixIndex = config["AppSettings:SuffixIndex"];

            {
                string[] bytestring = config["AppSettings:SipHashKey"].Split('-');
                if (bytestring.Length != 16)
                {
                    throw new ApplicationException("ShipHashKey shoud have 16 bytes byte string.");
                }
                SipHashKey = bytestring.Select(s => byte.Parse(s, System.Globalization.NumberStyles.AllowHexSpecifier)).ToArray();
            }

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="datetime">datetime</param>
        /// <param name="key">user assigined key value</param>
        /// <param name="hashingValues">hash value</param>
        public TslhID(DateTimeOffset datetime, short key, params string[] hashingValues) : this(datetime, key, HashToLong(hashingValues))
        {

        }

        public TslhID(Guid guid)
        {
            this.ArrayExpression = new ArraySegment<byte>(guid.ToByteArray());
        }

        public TslhID(SqlGuid guid)
        {
            this.ArrayExpression = new ArraySegment<byte>(ConvertGuidAndSqlGuid(guid.ToByteArray()));
        }

        /// <summary>
        /// ハッシュ値を計算します
        /// </summary>
        /// <param name="hashingValues">ハッシュ対象の値の文字列表現</param>
        /// <returns>ハッシュ値</returns>
        public static long HashToLong(params string[] hashingValues)
        {
            StringBuilder sb = new StringBuilder(HashSalt);
            for (int i = 0; i < hashingValues.Length; i++)
            {
                sb.Append(hashingValues[i]).Append(PrefixIndex + i + SuffixIndex).Append(HashSalt);
            }

            ulong hash = SipHash24.Hash64(Encoding.Unicode.GetBytes(sb.ToString()), SipHashKey);
            return BitConverter.ToInt64(BitConverter.GetBytes(hash), 0);
        }
    }
}
