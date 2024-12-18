using Unity.Burst;
using Unity.Entities;

partial struct HealthResetEventsSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (RefRW<Health> health in SystemAPI.Query<RefRW<Health>>())
        {
            health.ValueRW.OnHealthChanged = false;
        }
    }
}
