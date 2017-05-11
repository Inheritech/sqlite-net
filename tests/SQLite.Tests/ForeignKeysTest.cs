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
    public class ForeignKeysTest {

        public class ObjectTable {
            [PrimaryKey]
            public int id { get; set; }
            public string value { get; set; }
        }

        public class TagTable {
            [ForeignKey(typeof(ObjectTable))]
            public int id { get; set; }
            public string tag { get; set; }
        }

        public ObjectTable[] objArray = new ObjectTable[]
        {
            new ObjectTable()
            {
                id = 245,
                value = "I am object 245"
            },
            new ObjectTable()
            {
                id = 232,
                value = "I am object 232"
            }
        };

        public TagTable[] tagArray = new TagTable[]
        {
            new TagTable()
            {
                id = 245,
                tag = "Valid"
            },
            new TagTable()
            {
                id = 232,
                tag = "Invalid"
            }
        };

        [TestMethod]
        public void InnerJoin() {
            string tempFile = Path.GetTempFileName();
            SQLiteConnection conn = new SQLiteConnection(tempFile);
            conn.CreateTable<ObjectTable>();
            conn.CreateTable<TagTable>();

            foreach(var obj in objArray) {
                conn.Insert(obj);
            }

            foreach(var tag in tagArray) {
                conn.Insert(tag);
            }

            string query = "SELECT ObjectTable.* FROM TagTable INNER JOIN ObjectTable ON TagTable.id = ObjectTable.id WHERE TagTable.tag IN (?);";
            var results = conn.Query<ObjectTable>(query, "Valid");
            var resultsInvalid = conn.Query<ObjectTable>(query, "Invalid");

            Assert.AreEqual(results[ 0 ].value, objArray[ 0 ].value);
            Assert.AreEqual(resultsInvalid[ 0 ].value, objArray[ 1 ].value);
        }

    }

}
