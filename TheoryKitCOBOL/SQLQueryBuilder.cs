// Author: joshnies
//
// Copyright © Turring 2021. All Rights Reserved.
//
using System;
using System.Collections.Generic;

namespace TheoryKitCOBOL
{
    public enum QueryType
    {
        Select,
        Insert,
        Update,
        Delete
    }

    public class SQLQueryBuilder
    {
        public QueryType queryType;
        public List<string> selections = new List<string>();
        public List<string> from = new List<string>();
        public string where;
        public string orderBy;
        public int? offset;

        public static SQLQueryBuilder Select(params string[] columns)
        {
            var builder = new SQLQueryBuilder();
            builder.queryType = QueryType.Select;

            var selections = columns.Length == 0 ? new string[] { "*" } : columns;
            builder.selections = new List<string>(selections);

            return builder;
        }

        public SQLQueryBuilder From(params string[] tables)
        {
            from = new List<string>(tables);
            return this;
        }

        public SQLQueryBuilder Where(string condition)
        {
            where = condition;
            return this;
        }

        public SQLQueryBuilder AndWhere(string condition)
        {
            where += $" AND {condition}";
            return this;
        }

        public SQLQueryBuilder OrWhere(string condition)
        {
            where += $" OR {condition}";
            return this;
        }

        public SQLQueryBuilder StartWhereContainment(string combinedOp)
        {
            where += $" {combinedOp} (";
            return this;
        }

        public SQLQueryBuilder EndWhereContainment()
        {
            where += ")";
            return this;
        }

        public SQLQueryBuilder OrderBy(string column, string direction)
        {
            orderBy = $"{column} {direction}";
            return this;
        }

        public SQLQueryBuilder AndOrderBy(string column, string direction)
        {
            orderBy += $", {column} {direction}";
            return this;
        }

        // TODO: Remove this stub method.
        public SQLQueryBuilder Limit(int limit)
        {
            return this;
        }

        public SQLQueryBuilder OffsetBy(int offset)
        {
            this.offset = offset;
            return this;
        }

        public string Build()
        {
            switch (queryType)
            {
                case QueryType.Select:
                    return BuildSelect();
                default:
                    throw new NotImplementedException(
                        $"Query type \"{queryType}\" not yet implemented " +
                        "for SQLQueryBuilder."
                    );
            }
        }

        private string BuildSelect()
        {
            var joinedSelections = string.Join(", ", selections);
            var joinedFrom = string.Join(", ", from);
            var sqlWhere = where == null ? "" : $" WHERE {where}";
            var sqlOrderBy = orderBy == null ? "" : $" ORDER BY {orderBy}";
            var sqlOffset = offset == null ? "" : $" OFFSET {offset}";

            return $"SELECT {joinedSelections} FROM {joinedFrom}{sqlWhere}{sqlOrderBy} LIMIT 1{sqlOffset};";
        }
    }
}
