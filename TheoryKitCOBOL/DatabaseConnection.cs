// Author: joshnies
//
// Copyright © Turring 2021. All Rights Reserved.
//
using System;
using MySql.Data.MySqlClient;

namespace TheoryKitCOBOL
{
    /// <summary>
    /// Database connection class designed for COBOL compatibility.
    /// Currently only supports MySQL.
    /// </summary>
    public class DatabaseConnection
    {
        /// <summary>
        /// MySQL connection.
        /// </summary>
        private MySqlConnection mysqlConn;

        private SQLQueryBuilder queryBuilder;
        private int currentOffset;

        public DatabaseConnection(string host, string user, string password, string databaseName)
        {
            // Connect to MySQL database
            var cs = $@"server={host};userid={user};password={password};database={databaseName}";
            mysqlConn = new MySqlConnection(cs);
            mysqlConn.Open();
        }

        /// <summary>
        /// Execute query.
        /// </summary>
        /// <param name="queryBuilder">SQL query builder.</param>
        /// <returns>String value containing all values.</returns>
        public string Query(SQLQueryBuilder queryBuilder)
        {
            this.queryBuilder = queryBuilder;
            var query = this.queryBuilder.Build();

            // Execute query on database
            var cmd = new MySqlCommand(query, mysqlConn);

            switch(queryBuilder.queryType)
            {
                case QueryType.Select:
                    using (var reader = cmd.ExecuteReader())
                    {
                        var combinedValues = "";
                        // Read first record
                        if (reader.Read())
                        {
                            for (int i = 0; i < reader.VisibleFieldCount; i++)
                            {
                                var value = reader.GetValue(i);
                                combinedValues += value;
                            }
                        }
                                               
                        return combinedValues;
                    }
                default:
                    throw new NotImplementedException(
                        $"Query type \"{queryBuilder.queryType}\" not yet " +
                        "implemented for DatabaseConnection."
                    );
            }
        }

        /// <summary>
        /// Execute query for getting the next record for the current query
        /// builder.
        /// </summary>
        /// <param name="queryBuilder">SQL query builder.</param>
        /// <returns>String value containing all values.</returns>
        public string QueryNext()
        {
            queryBuilder.OffsetBy(currentOffset);
            currentOffset++;
            return Query(queryBuilder);
        }

        /// <summary>
        /// Close database connection.
        /// </summary>
        public void Close() => mysqlConn.Close();
    }
}
