using Akiled.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Akiled.HabboHotel.Users.Messenger
{
    public class SearchResultFactory
    {
        public static List<SearchResult> GetSearchResult(string query)
        {
            List<SearchResult> list = new List<SearchResult>();
            DataTable table;
            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.SetQuery("SELECT id,username,look FROM users WHERE username LIKE @query LIMIT 50");
                queryreactor.AddParameter("query", (query.Replace("%", "\\%").Replace("_", "\\_") + "%"));
                table = queryreactor.GetTable();
            }
            foreach (DataRow dataRow in table.Rows)
            {
                SearchResult searchResult = new SearchResult(Convert.ToInt32(dataRow[0]), (string)dataRow[1], (string)dataRow[2]);
                list.Add(searchResult);
            }
            return list;
        }
    }
}
