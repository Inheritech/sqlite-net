using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLite.Attributes;

namespace SQLite.Tests {

    [TestClass]
    public class CompositeKeysTest {
        public class CompositeTable {
            [CompositeKey]
            public int id { get; set; }
            [CompositeKey]
            public string sid { get; set; }
            public string value { get; set; }
        }

        public CompositeTable[] tableArray = new CompositeTable[]
        {
            new CompositeTable()
            {
                id = 2,
                sid = "4",
                value = "This is step 1"
            },
            new CompositeTable()
            {
                id = 4,
                sid = "6",
                value = "This is step 2"
            },
            new CompositeTable()
            {
                id = 3,
                sid = "28",
                value = "This is step 3"
            }
        };

        public CompositeTable clone = new CompositeTable()
        {
            id = 3,
            sid = "28",
            value = "This is not step 3"
        };

        [TestMethod]
        [Description("Check if composite keys work")]
        public void TestComposite() {
            string tempFile = Path.GetTempFileName();
            SQLiteConnection conn = new SQLiteConnection(tempFile);
            conn.CreateTable<CompositeTable>();
            foreach(var tab in tableArray) {
                conn.Insert(tab);
            }

            var table1 = conn.Get<CompositeTable>(2);
            Assert.AreEqual(table1.value, tableArray[ 0 ].value);

            var table2 = conn.Query<CompositeTable>("SELECT * FROM CompositeTable WHERE sid = ?", "6");
            Assert.AreEqual(table2[0].value, tableArray[ 1 ].value);

        }

        [TestMethod]
        [ExpectedException(typeof(SQLite.Exceptions.SQLiteException))]
        [Description("Check if the constraint made with composite keys works")]
        public void CompositeKeyConstraint() {
            string tempFile = Path.GetTempFileName();
            SQLiteConnection conn = new SQLiteConnection(tempFile);
            conn.CreateTable<CompositeTable>();
            foreach (var tab in tableArray) {
                conn.Insert(tab);
            }

            conn.Insert(clone);
        }

    }
}
