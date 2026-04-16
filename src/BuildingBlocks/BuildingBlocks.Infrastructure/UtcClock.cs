using BuildingBlocks.Application;

namespace BuildingBlocks.Infrastructure;

public sealed class UtcClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
