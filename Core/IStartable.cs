using Akiled.Utilities.DependencyInjection;
using System.Threading.Tasks;

namespace Akiled.Core
{
    [Transient]
    public interface IStartable
    {
        Task Start();

    }
}