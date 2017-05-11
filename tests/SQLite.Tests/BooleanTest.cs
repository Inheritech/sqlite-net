using System;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLite;
using SQLite.Attributes;

namespace SQLite.Tests {
    [TestClass]
    public class BooleanTest {

        public class VO {
            [AutoIncrement, PrimaryKey]
            public int ID { get; set; }
            public bool Flag { get; set; }
            public string Text { get; set; }

            public override string ToString() {
                return string.Format("VO:: ID:{0} Flag:{1} Text:{2}", ID, Flag, Text);
            }
        }

        public class DBAcs : SQLiteConnection {

            public DBAcs(string path) : base(path) {

            }

            public void BuildTable() {
                CreateTable<VO>();
            }

            public int CountWithFlag(bool flag) {
                var cmd = CreateCommand("SELECT COUNT(*) FROM VO WHERE Flag = ?", flag);
                return cmd.ExecuteScalar<int>();
            }
        }

        [TestMethod]
        public void TestBoolean() {
            string tmpFile = Path.GetTempFileName();
            DBAcs db = new DBAcs(tmpFile);

            db.BuildTable();
            for (int i = 0; i < 10; i++) {
                db.Insert(new VO()
                {
                    Flag = (i % 3 == 0),
                    Text = string.Format("VO{0}", i)
                });
            }

            Assert.AreEqual(4, db.CountWithFlag(true));
            Assert.AreEqual(6, db.CountWithFlag(false));

            Debug.WriteLine("VO with tru flags:");
            foreach (var vo in db.Query<VO>("SELECT * FROM VO Where Flag = ?", true)) {
                Debug.WriteLine(vo.ToString());
            }

            Debug.WriteLine("VO with false flag:");
            foreach (var vo in db.Query<VO>("SELECT * FROM VO WHERE Flag = ?", false)) {
                Debug.WriteLine(vo.ToString());
            }
        }
    }
}
