using System.Threading.Tasks;

namespace Akiled;

public interface IAkiledEnvironment
{
    Task<bool> Start();

}