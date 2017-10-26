using EfCoreLongStringTest;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace EfPoolingTest
{
    [TestClass]
    public class UnitTest1
    {
        const int count = 1000000; // number of entries and test runs
        const int batchsize = 1000; // batch size for the initial insert

        [ClassInitialize]
        public static void Init(TestContext _)
        {
            var hasEntries = false;
            using (var context = new MyContext())
            {
                // ensure a fresh database
                context.Database.EnsureCreated();
                hasEntries = context.MyTable.Any();
            }

            // create entries to later be used for testing
            // we use that many entries to ensure that the results cannot be cached
            if (!hasEntries)
            {
                var entries = new List<MyTable>(batchsize);
                for (int i = 0; i < count;)
                {
                    entries.Clear();
                    using (var context = new MyContext())
                    {
                        for (int j = 0; j < batchsize; i++)
                        {
                            j++;
                            entries.Add(new MyTable { MyColumn = i.ToString() });
                        }
                        context.AddRange(entries);
                        context.SaveChanges();
                    }
                }
            }
        }

        [TestMethod]
        public void NormalContext()
        {
            // Connection count max = 1
            for (int i = 1; i <= count; i++)
            {
                using (var context = new MyContext())
                {
                    var myTable = context.Set<MyTable>().FirstOrDefault(m => m.Id == i);
                    Debug.WriteLine("NormalContext " + myTable.MyColumn);
                }
            }
        }

        [TestMethod]
        public async Task NormalContextAsync()
        {
            // Connection count max = 1
            for (int i = 1; i <= count; i++)
            {
                using (var context = new MyContext())
                {
                    var myTable = await context.Set<MyTable>().FirstOrDefaultAsync(m => m.Id == i).ConfigureAwait(false);
                    Debug.WriteLine("NormalContextAsync " + myTable.MyColumn);
                }
            }
        }

        [TestMethod]
        public void NormalContextParallelAsyncConfigureAwaitFalse()
        {
            // Connection count max = 100 - MaxPoolSize
            // Throws System.InvalidOperationException Timeout expired.
            Parallel.For(1, count + 1, i =>
            {
                using (var context = new MyContext())
                {
                    var myTable = context.Set<MyTable>().FirstOrDefaultAsync(m => m.Id == i).ConfigureAwait(false).GetAwaiter().GetResult();
                    Debug.WriteLine("NormalContextParallelAsyncConfigureAwaitFalse " + myTable.MyColumn);
                }
            });
        }

        [TestMethod]
        public void NormalContextParallelAsyncConfigureAwaitTrue()
        {
            // Connection count max = 100 - MaxPoolSize
            // Throws System.InvalidOperationException Timeout expired.
            Parallel.For(1, count + 1, i =>
            {
                using (var context = new MyContext())
                {
                    var myTable = context.Set<MyTable>().FirstOrDefaultAsync(m => m.Id == i).ConfigureAwait(true).GetAwaiter().GetResult();
                    Debug.WriteLine("NormalContextParallelAsyncConfigureAwaitTrue " + myTable.MyColumn);
                }
            });
        }

        [TestMethod]
        public void NormalContextParallel()
        {
            // Connection count max = 4 - MaxDegreeOfParallelism
            Parallel.For(1, count + 1, i =>
            {
                using (var context = new MyContext())
                {
                    var myTable = context.Set<MyTable>().FirstOrDefault(m => m.Id == i);
                    Debug.WriteLine("NormalContextParallel " + myTable.MyColumn);
                }
            });
        }
    }
}