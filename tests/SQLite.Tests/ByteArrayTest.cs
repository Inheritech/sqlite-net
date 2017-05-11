using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLite.Attributes;

namespace SQLite.Tests {
    [TestClass]
    public class ByteArrayTest {
        public class ByteArrayClass {
            [PrimaryKey, AutoIncrement]
            public int ID { get; set; }

            public byte[] bytes { get; set; }

            public void AssertEquals(ByteArrayClass other) {
                Assert.AreEqual(other.ID, ID);
                if (other.bytes == null || bytes == null) {
                    Assert.IsNull(other.bytes);
                    Assert.IsNull(bytes);
                } else {
                    Assert.AreEqual(other.bytes.Length, bytes.Length);
                    for (int i = 0; i < bytes.Length; i++) {
                        Assert.AreEqual(other.bytes[ i ], bytes[ i ]);
                    }
                }
            }
        }

        [TestMethod]
        [Description("Create objects with varios byte arrays and check they can be stored and retrieved correctly")]
        public void ByteArrays() {
            ByteArrayClass[] byteArrays = new ByteArrayClass[]
            {
                new ByteArrayClass() { bytes = new byte[] { 1, 2, 3, 4, 250, 252, 253, 254, 255 } }, //Range check
				new ByteArrayClass() { bytes = new byte[] { 0 } }, //null bytes need to be handled correctly
				new ByteArrayClass() { bytes = new byte[] { 0, 0 } },
                new ByteArrayClass() { bytes = new byte[] { 0, 1, 0 } },
                new ByteArrayClass() { bytes = new byte[] { 1, 0, 1 } },
                new ByteArrayClass() { bytes = new byte[] { } }, //Empty byte array should stay empty (and not become null)
				new ByteArrayClass() { bytes = null } //Null should be supported
            };

            SQLiteConnection database = new SQLiteConnection(Path.GetTempFileName());

            database.CreateTable<ByteArrayClass>();

            //Insert all of the ByteArrayClass
            foreach (ByteArrayClass b in byteArrays) {
                database.Insert(b);
            }

            //Get them back out
            ByteArrayClass[] fetchedByteArrays = database.Table<ByteArrayClass>().OrderBy(x => x.ID).ToArray();

            Assert.AreEqual(fetchedByteArrays.Length, byteArrays.Length);

            //Check they are the same
            for (int i = 0; i < byteArrays.Length; i++) {
                byteArrays[ i ].AssertEquals(fetchedByteArrays[ i ]);
            }
        }
    }
}
