namespace TeamCon2024.Core.Retries;

public sealed record RetrySettings(int Times, TimeSpan Delay)
{
    public static RetrySettings Default { get; } = new RetrySettings(3, TimeSpan.FromMilliseconds(50));
}
