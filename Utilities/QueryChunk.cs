using Akiled.Database.Interfaces;
using System.Collections.Generic;
using System.Text;

namespace Akiled.Utilities
{
    public class QueryChunk
    {
        private Dictionary<string, object> parameters;
        private StringBuilder queries;
        private int queryCount;
        private readonly EndingType endingType;

        public QueryChunk()
        {
            this.parameters = new Dictionary<string, object>();
            this.queries = new StringBuilder();
            this.queryCount = 0;
            this.endingType = EndingType.Sequential;
        }

        public QueryChunk(string startQuery)
        {
            this.parameters = new Dictionary<string, object>();
            this.queries = new StringBuilder(startQuery);
            this.endingType = EndingType.Continuous;
            this.queryCount = 0;
        }

        public void AddQuery(string query)
        {
            ++this.queryCount;
            this.queries.Append(query);
            switch (this.endingType)
            {
                case EndingType.Sequential:
                    this.queries.Append(";");
                    break;
                case EndingType.Continuous:
                    this.queries.Append(",");
                    break;
            }
        }

        public void AddParameter(string parameterName, object value)
        {
            this.parameters.Add(parameterName, value);
        }

        public void Execute(IQueryAdapter dbClient)
        {
            if (this.queryCount == 0)
                return;
            this.queries = this.queries.Remove(this.queries.Length - 1, 1);
            dbClient.SetQuery((this.queries).ToString());
            foreach (KeyValuePair<string, object> keyValuePair in this.parameters)
                dbClient.AddParameter(keyValuePair.Key, (string)keyValuePair.Value);
            dbClient.RunQuery();
        }

        public void Dispose()
        {
            this.parameters.Clear();
            this.queries.Clear();
            this.parameters = (Dictionary<string, object>)null;
            this.queries = (StringBuilder)null;
        }
    }
}
