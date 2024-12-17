using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct CollisionAttackerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        foreach ((RefRO<LocalTransform> transform, RefRW<CollisionAttacker> attacker, RefRO<AttackTarget> target, Entity entity) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<CollisionAttacker>, RefRO<AttackTarget>>().WithEntityAccess())
        {
            if (target.ValueRO.ActiveTarget == Entity.Null && attacker.ValueRO.DestroyOnNoTarget)
            {
                entityCommandBuffer.DestroyEntity(entity);
                continue;
            }

            LocalTransform targetTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.ActiveTarget);
            if (math.distancesq(targetTransform.Position, transform.ValueRO.Position) <= attacker.ValueRO.CollisionDistance)
            {
                attacker.ValueRW.AttackTimer += SystemAPI.Time.DeltaTime;

                if (attacker.ValueRO.AttackTimer >= attacker.ValueRO.AttackDelay)
                {
                    RefRW<Health> health = SystemAPI.GetComponentRW<Health>(target.ValueRO.ActiveTarget);
                    health.ValueRW.CurrentHealth -= attacker.ValueRO.Damage;

                    attacker.ValueRW.AttackTimer = 0f;

                    if (attacker.ValueRO.DestroyOnHit)
                    {
                        entityCommandBuffer.DestroyEntity(entity);
                    }
                }
            }
        }
    }
}
