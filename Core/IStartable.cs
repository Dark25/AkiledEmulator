using System.Threading.Tasks;
using Akiled.Utilities.DependencyInjection;

namespace Akiled.Core
{
    [Transient]
    public interface IStartable
    {
         void InitiaStartlize();
    
    }
}