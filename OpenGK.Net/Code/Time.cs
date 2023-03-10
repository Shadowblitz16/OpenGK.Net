
using System.Diagnostics;

namespace OpenGK;

public static class Time
{
    public readonly static float TimeStarted = NanoTime(); 
    public static float GetTime() => (float)((NanoTime() - TimeStarted) * 1E-9); 

    private static long NanoTime() 
    {
        long nano = 10000L * Stopwatch.GetTimestamp();
        nano /= TimeSpan.TicksPerMillisecond;
        nano *= 100L;
        return nano;
    }
}