using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace SQLite.AsyncSupport {
    //
    // TODO: Bind to AsyncConnection.GetConnection instead so that delayed
    // execution can still work after a Pool.Reset.
    //
    public class TableQueryAsync<T>
        where T : new() {
        TableQuery<T> _innerQuery;

        public TableQueryAsync(TableQuery<T> innerQuery) {
            _innerQuery = innerQuery;
        }

        public TableQueryAsync<T> Where(Expression<Func<T, bool>> predExpr) {
            return new TableQueryAsync<T>(_innerQuery.Where(predExpr));
        }

        public TableQueryAsync<T> Skip(int n) {
            return new TableQueryAsync<T>(_innerQuery.Skip(n));
        }

        public TableQueryAsync<T> Take(int n) {
            return new TableQueryAsync<T>(_innerQuery.Take(n));
        }

        public TableQueryAsync<T> OrderBy<U>(Expression<Func<T, U>> orderExpr) {
            return new TableQueryAsync<T>(_innerQuery.OrderBy<U>(orderExpr));
        }

        public TableQueryAsync<T> OrderByDescending<U>(Expression<Func<T, U>> orderExpr) {
            return new TableQueryAsync<T>(_innerQuery.OrderByDescending<U>(orderExpr));
        }

        public Task<List<T>> ToListAsync() {
            return Task.Factory.StartNew(() => {
                using (((SQLiteConnectionWithLock)_innerQuery.Connection).Lock()) {
                    return _innerQuery.ToList();
                }
            });
        }

        public Task<int> CountAsync() {
            return Task.Factory.StartNew(() => {
                using (((SQLiteConnectionWithLock)_innerQuery.Connection).Lock()) {
                    return _innerQuery.Count();
                }
            });
        }

        public Task<T> ElementAtAsync(int index) {
            return Task.Factory.StartNew(() => {
                using (((SQLiteConnectionWithLock)_innerQuery.Connection).Lock()) {
                    return _innerQuery.ElementAt(index);
                }
            });
        }

        public Task<T> FirstAsync() {
            return Task<T>.Factory.StartNew(() => {
                using (((SQLiteConnectionWithLock)_innerQuery.Connection).Lock()) {
                    return _innerQuery.First();
                }
            });
        }

        public Task<T> FirstOrDefaultAsync() {
            return Task<T>.Factory.StartNew(() => {
                using (((SQLiteConnectionWithLock)_innerQuery.Connection).Lock()) {
                    return _innerQuery.FirstOrDefault();
                }
            });
        }
    }
}
