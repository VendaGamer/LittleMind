using System.Threading;
using JetBrains.Annotations;

public static class Extensions
{
    public static CancellationTokenSource CancelCurrentActionAndCreateNewSrc([CanBeNull]this CancellationTokenSource curTokSrc)
    {
        if(curTokSrc == null)
            return new CancellationTokenSource();
        
        curTokSrc.Cancel();
        curTokSrc.Dispose();
        return new CancellationTokenSource();
    }

    public static void CancelAndDispose([CanBeNull]this CancellationTokenSource curTokSrc)
    {
        if(curTokSrc == null)return;
        
        curTokSrc.Cancel();
        curTokSrc.Dispose();
    }
}