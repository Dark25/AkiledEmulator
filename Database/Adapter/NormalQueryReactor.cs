using Akiled.Database.Interfaces;
using System;

namespace Akiled.Database.Adapter
{
    public class NormalQueryReactor : QueryAdapter, IQueryAdapter, IRegularQueryAdapter, IDisposable
    {
        public NormalQueryReactor(IDatabaseClient client)
          : base(client) => this.Command = client.createNewCommand();

        public void Dispose()
        {
            this.Command.Dispose();
            this.Client.reportDone();
            GC.SuppressFinalize(this);
        }
    }
}