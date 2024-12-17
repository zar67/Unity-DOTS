using Unity.Burst;
using Unity.Entities;

[UpdateAfter(typeof(EndSimulationEntityCommandBufferSystem))]
partial struct AttackerTargetResetSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {

        foreach (RefRW<Attacker> attacker in SystemAPI.Query<RefRW<Attacker>>())
        {
            if (!SystemAPI.Exists(attacker.ValueRO.ActiveTarget))
            {
                attacker.ValueRW.ActiveTarget = Entity.Null;
            }
        }
    }
}
