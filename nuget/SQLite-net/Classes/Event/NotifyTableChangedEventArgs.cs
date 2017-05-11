using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using SQLite.Classes.Definitions;
using SQLite.Enums;

namespace SQLite.Classes.Event {
    public class NotifyTableChangedEventArgs : EventArgs {
        public TableMapping Table { get; private set; }
        public NotifyTableChangedAction Action { get; private set; }

        public NotifyTableChangedEventArgs(TableMapping table, NotifyTableChangedAction action) {
            Table = table;
            Action = action;
        }
    }
}
