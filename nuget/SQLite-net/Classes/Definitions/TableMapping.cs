﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SQLite.Attributes;
using SQLite.Enums;
using ConcurrentStringDictionary = System.Collections.Generic.Dictionary<string, object>;
using SQLite.Extensions;

namespace SQLite.Definitions {

    public class TableMapping {
        public Type MappedType { get; private set; }

        public string TableName { get; private set; }

        public Column[] Columns { get; private set; }

        public Column PK { get; private set; }

        public ForeignKey FK { get; private set; }

        public Column[] CK { get; private set; }

        public string GetByPrimaryKeySql { get; private set; }

        Column _autoPk;
        Column[] _insertColumns;
        Column[] _insertOrReplaceColumns;

        public TableMapping(Type type, CreateFlags createFlags = CreateFlags.None) {
            MappedType = type;

#if USE_NEW_REFLECTION_API
            var tableAttr = (TableAttribute)System.Reflection.CustomAttributeExtensions
                .GetCustomAttribute(type.GetTypeInfo(), typeof(TableAttribute), true);
#else
			var tableAttr = (TableAttribute)type.GetCustomAttributes (typeof (TableAttribute), true).FirstOrDefault ();
#endif

            TableName = tableAttr != null ? tableAttr.Name : MappedType.Name;

#if !USE_NEW_REFLECTION_API
			var props = MappedType.GetProperties (BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);
#else
            var props = from p in MappedType.GetRuntimeProperties()
                        where ((p.GetMethod != null && p.GetMethod.IsPublic) || (p.SetMethod != null && p.SetMethod.IsPublic) || (p.GetMethod != null && p.GetMethod.IsStatic) || (p.SetMethod != null && p.SetMethod.IsStatic))
                        select p;
#endif
            var cols = new List<Column>();
            foreach (var p in props) {
#if !USE_NEW_REFLECTION_API
				var ignore = p.GetCustomAttributes (typeof(IgnoreAttribute), true).Length > 0;
#else
                var ignore = p.GetCustomAttributes(typeof(IgnoreAttribute), true).Count() > 0;
#endif
                if (p.CanWrite && !ignore) {
                    cols.Add(new Column(p, createFlags));
                }
            }
            Columns = cols.ToArray();

            List<Column> ck = new List<Column>();
            foreach (var c in Columns) {
                if (c.IsAutoInc && c.IsPK) {
                    _autoPk = c;
                }
                if (c.IsPK) {
                    PK = c;
                }
                if (c.IsCK) {
                    ck.Add(c);
                }
                if (c.IsFK) {
                    FK = c.FK;
                }
            }

            CK = ck.ToArray();

            HasAutoIncPK = _autoPk != null;

            if (PK != null || ck.Count != 0) {
                if (PK != null && ck.Count == 0) {
                    GetByPrimaryKeySql = string.Format("select * from \"{0}\" where \"{1}\" = ?", TableName, PK.Name);
                }
                if (PK == null && ck.Count > 0) {
                    GetByPrimaryKeySql = string.Format("select * from \"{0}\" where \"{1}\" = ?", TableName, CK[0].Name);
                }
            } else {
                // People should not be calling Get/Find without a PK
                GetByPrimaryKeySql = string.Format("select * from \"{0}\" limit 1", TableName);
            }
            _insertCommandMap = new ConcurrentStringDictionary();
        }

        public TableMapping(Type type, CreateFlags createFlags, CreateFlags createFlags1 = default(CreateFlags)) : this(type, createFlags) {
            this.createFlags1 = createFlags1;
        }

        public bool HasAutoIncPK { get; private set; }

        public void SetAutoIncPK(object obj, long id) {
            if (_autoPk != null) {
                _autoPk.SetValue(obj, Convert.ChangeType(id, _autoPk.ColumnType, null));
            }
        }

        public Column[] InsertColumns
        {
            get
            {
                if (_insertColumns == null) {
                    _insertColumns = Columns.Where(c => !c.IsAutoInc).ToArray();
                }
                return _insertColumns;
            }
        }

        public Column[] InsertOrReplaceColumns
        {
            get
            {
                if (_insertOrReplaceColumns == null) {
                    _insertOrReplaceColumns = Columns.ToArray();
                }
                return _insertOrReplaceColumns;
            }
        }

        public Column FindColumnWithPropertyName(string propertyName) {
            var exact = Columns.FirstOrDefault(c => c.PropertyName == propertyName);
            return exact;
        }

        public Column FindColumn(string columnName) {
            var exact = Columns.FirstOrDefault(c => c.Name.ToLower() == columnName.ToLower());
            return exact;
        }

        ConcurrentStringDictionary _insertCommandMap;
        private CreateFlags createFlags1;

        public PreparedSqlLiteInsertCommand GetInsertCommand(SQLiteConnection conn, string extra) {
            object prepCmdO;

            if (!_insertCommandMap.TryGetValue(extra, out prepCmdO)) {
                var prepCmd = CreateInsertCommand(conn, extra);
                prepCmdO = prepCmd;
                if (!_insertCommandMap.TryAdd(extra, prepCmd)) {
                    // Concurrent add attempt beat us.
                    prepCmd.Dispose();
                    _insertCommandMap.TryGetValue(extra, out prepCmdO);
                }
            }
            return (PreparedSqlLiteInsertCommand)prepCmdO;
        }

        PreparedSqlLiteInsertCommand CreateInsertCommand(SQLiteConnection conn, string extra) {
            var cols = InsertColumns;
            string insertSql;
            if (!cols.Any() && Columns.Count() == 1 && Columns[ 0 ].IsAutoInc) {
                insertSql = string.Format("insert {1} into \"{0}\" default values", TableName, extra);
            } else {
                var replacing = string.Compare(extra, "OR REPLACE", StringComparison.OrdinalIgnoreCase) == 0;

                if (replacing) {
                    cols = InsertOrReplaceColumns;
                }

                insertSql = string.Format("insert {3} into \"{0}\"({1}) values ({2})", TableName,
                                   string.Join(",", (from c in cols
                                                     select "\"" + c.Name + "\"").ToArray()),
                                   string.Join(",", (from c in cols
                                                     select "?").ToArray()), extra);

            }

            var insertCommand = new PreparedSqlLiteInsertCommand(conn);
            insertCommand.CommandText = insertSql;
            return insertCommand;
        }

        protected internal void Dispose() {
            foreach (var pair in _insertCommandMap) {
                ((PreparedSqlLiteInsertCommand)pair.Value).Dispose();
            }
            _insertCommandMap = null;
        }

        public class Column {
            PropertyInfo _prop;

            public string Name { get; private set; }

            public string PropertyName { get { return _prop.Name; } }

            public Type ColumnType { get; private set; }

            public string Collation { get; private set; }

            public bool IsAutoInc { get; private set; }
            public bool IsAutoGuid { get; private set; }

            public bool IsPK { get; private set; }

            public bool IsCK { get; private set; }

            public bool IsFK { get; private set; }

            public IEnumerable<IndexedAttribute> Indices { get; set; }

            public bool IsNullable { get; private set; }

            public int? MaxStringLength { get; private set; }

            public bool StoreAsText { get; private set; }

            public ForeignKey FK;

            public Column(PropertyInfo prop, CreateFlags createFlags = CreateFlags.None) {
                var colAttr = (ColumnAttribute)prop.GetCustomAttributes(typeof(ColumnAttribute), true).FirstOrDefault();

                _prop = prop;
                Name = colAttr == null ? prop.Name : colAttr.Name;
                //If this type is Nullable<T> then Nullable.GetUnderlyingType returns the T, otherwise it returns null, so get the actual type instead
                ColumnType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                Collation = Orm.Collation(prop);

                IsPK = Orm.IsPK(prop) ||
                    (((createFlags & CreateFlags.ImplicitPK) == CreateFlags.ImplicitPK) &&
                         string.Compare(prop.Name, Orm.ImplicitPkName, StringComparison.OrdinalIgnoreCase) == 0);

                IsCK = Orm.IsCK(prop);

                IsFK = Orm.IsFK(prop);
                if (IsFK) {
                    ForeignKeyAttribute attr = prop.GetCustomAttribute<ForeignKeyAttribute>(true);
                    TableMapping refTable = new TableMapping(attr.ReferenceTable);
                    string refColumnName = attr.ReferenceColumn ?? this.Name;
                    var refColumns = refTable.Columns.Select(c => c).Where(c => c.Name == refColumnName).ToArray();
                    if (refColumns.Length > 0) {
                        FK = new ForeignKey(this, attr.ReferenceTable, refColumns[ 0 ]);
                    } else {
                        throw new Exception(string.Format("The referenced column \"{0}\" in {1} doesn't exist", refColumnName, this.Name));
                    }
                }

                var isAuto = Orm.IsAutoInc(prop) || (IsPK && ((createFlags & CreateFlags.AutoIncPK) == CreateFlags.AutoIncPK));
                IsAutoGuid = isAuto && ColumnType == typeof(Guid);
                IsAutoInc = isAuto && !IsAutoGuid;

                Indices = Orm.GetIndices(prop);
                if (!Indices.Any()
                    && !IsPK
                    && ((createFlags & CreateFlags.ImplicitIndex) == CreateFlags.ImplicitIndex)
                    && Name.EndsWith(Orm.ImplicitIndexSuffix, StringComparison.OrdinalIgnoreCase)
                    ) {
                    Indices = new IndexedAttribute[] { new IndexedAttribute() };
                }
                IsNullable = !(IsPK || Orm.IsMarkedNotNull(prop));
                MaxStringLength = Orm.MaxStringLength(prop);

                StoreAsText = prop.PropertyType.GetTypeInfo().GetCustomAttribute(typeof(StoreAsTextAttribute), false) != null;
            }

            public void SetValue(object obj, object val) {
                _prop.SetValue(obj, val, null);
            }

            public object GetValue(object obj) {
                return _prop.GetValue(obj, null);
            }
        }
    }
}
