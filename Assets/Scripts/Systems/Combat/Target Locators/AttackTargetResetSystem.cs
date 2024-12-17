using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true),UpdateBefore(typeof(BeginSimulationEntityCommandBufferSystem))]
partial struct AttackTargetResetSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (RefRW<AttackTarget> attacker in SystemAPI.Query<RefRW<AttackTarget>>())
        {
            if (attacker.ValueRO.ActiveTarget != Entity.Null)
            {
                if (!SystemAPI.Exists(attacker.ValueRO.ActiveTarget) || !SystemAPI.HasComponent<LocalTransform>(attacker.ValueRO.ActiveTarget))
                {
                    attacker.ValueRW.ActiveTarget = Entity.Null;
                }
            }
        }
    }
}
