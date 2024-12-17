using Unity.Burst;
using Unity.Entities;

[UpdateBefore(typeof(EndSimulationEntityCommandBufferSystem))]
partial struct RangedAttackSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach ((RefRW<RangedAttack> rangedAttack, RefRO<Attacker> attacker) in SystemAPI.Query<RefRW<RangedAttack>, RefRO<Attacker>>())
        {
            if (attacker.ValueRO.ActiveTarget != Entity.Null)
            {
                rangedAttack.ValueRW.AttackTimer += SystemAPI.Time.DeltaTime;

                if (rangedAttack.ValueRO.AttackTimer >= rangedAttack.ValueRO.AttackDelay)
                {
                    RefRW<Health> health = SystemAPI.GetComponentRW<Health>(attacker.ValueRO.ActiveTarget);
                    health.ValueRW.CurrentHealth -= rangedAttack.ValueRO.Damage;
                    // Shoot

                    rangedAttack.ValueRW.AttackTimer = 0f;
                }
            }
        }
    }
}
