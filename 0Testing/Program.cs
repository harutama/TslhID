using Harutama.TimeShortLongHashID;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace _0Testing
{
    class Program
    {
        static void Main(string[] args)
        {

            Test();
            //Test2();
            //Test3();

        }


        private static void Test()
        {
            var popo = new TslhID(DateTimeOffset.Now, 12321, "mooooooooooooooooooji", "mojiji", "#$!$#(*^*!");
            //Assert.AreEqual(new TslhID(popo.AsGuid), new TslhID(popo.AsSqlGuid));

            var aaaa = new TslhID(new DateTimeOffset(2020, 12, 12, 12, 12, 12, 999, new TimeSpan(9, 0, 0)), 30000, "kekeke", "popopo");

            var xxxx = new TslhID(new DateTimeOffset(2020, 12, 12, 12, 12, 12, 998, new TimeSpan(9, 0, 0)), -30000, "kekeke", "popopo");

            {
                SortedList<SqlGuid, TslhID> sql = new SortedList<SqlGuid, TslhID>();
                SortedList<Guid, TslhID> net = new SortedList<Guid, TslhID>();

                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        var obj = new TslhID(new DateTimeOffset(2020, 12, 12, 12, 12, i, j, new TimeSpan(9, 0, 0)), Convert.ToInt16(i - j), i + "ppp", j + "ppp");

                        sql.Add(new SqlGuid(obj.AsGuid), obj);
                        net.Add(obj.AsGuid, obj);
                    }
                }

                foreach (var o in sql)
                {
                    Console.WriteLine($"{o.Key.ToString()}\t{o.Value.Datetime.ToString()}\t{o.Value.UserKey}\t{o.Value.HashValue}");
                }

                Console.WriteLine();

                foreach (var o in net)
                {
                    Console.WriteLine($"{o.Key.ToString()}\t{o.Value.Datetime.ToString()}\t{o.Value.UserKey}\t{o.Value.HashValue}");
                }
            }

            var a = new TslhID(new DateTimeOffset(2020, 12, 12, 12, 12, 12, 001, new TimeSpan(9, 0, 0)), -30000, "kekeka", "popopo");
            var b = new TslhID(new DateTimeOffset(2020, 12, 12, 12, 12, 12, 002, new TimeSpan(9, 0, 0)), -30000, "kekekb", "popopo");
            var c = new TslhID(new DateTimeOffset(2020, 12, 12, 12, 12, 12, 003, new TimeSpan(9, 0, 0)), -30000, "kekekc", "popopo");
            var d = new TslhID(new DateTimeOffset(2020, 12, 12, 12, 12, 12, 004, new TimeSpan(9, 0, 0)), -30000, "kekekd", "popopo");
            var e = new TslhID(new DateTimeOffset(2020, 12, 12, 12, 12, 12, 005, new TimeSpan(9, 0, 0)), -30000, "kekeke", "popopo");
        }

        private static void Test2()
        {
            for (long l = 0L; l < 5; l++)
            {
                var si = BitConverter.GetBytes(l);
                var usi = BitConverter.GetBytes(Convert.ToUInt64(l));

                Console.WriteLine(BitConverter.ToString(si) + "    " + BitConverter.ToString(usi));
            }


            {
                var now = DateTimeOffset.Now.Ticks;

                for (long i = 0; i < 10; i++)
                {
                    now = now + i;

                    Console.WriteLine(BitConverter.ToString(BitConverter.GetBytes(now)));
                    Console.WriteLine(BitConverter.ToString(BitConverter.GetBytes(Convert.ToUInt64(now))));
                }

            }
        }


        static void Test3()
        {

            for (long l = -62135596800000; l < long.MaxValue; l += 100000)
            {
                var hash = TslhID.HashToLong(l + "もじ" + l);
                var sh = Convert.ToInt16(l % short.MaxValue);
                var dt = DateTimeOffset.FromUnixTimeMilliseconds(l);

                var id = new TslhID(dt, sh, l + "もじ" + l);

                Assert.AreEqual(hash, id.HashValue);
                Assert.AreEqual(sh, id.UserKey);
                Assert.AreEqual(dt.ToUnixTimeMilliseconds(), id.Datetime.ToUnixTimeMilliseconds());

                Console.WriteLine(id.AsGuid);
            }

        }


    }
}
