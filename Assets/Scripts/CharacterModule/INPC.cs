using System.Threading;
using Cysharp.Threading.Tasks;

public interface INPC
{
    UniTask Behaviour(CancellationToken token);
}