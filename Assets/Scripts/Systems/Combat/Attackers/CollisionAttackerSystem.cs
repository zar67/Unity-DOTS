using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct CollisionAttackerSystem : ISystem
{
    //[BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        foreach ((RefRO<LocalTransform> transform, RefRW<CollisionAttacker> attacker, RefRO<AttackTarget> target, RefRW<TargetPositionMovement> movement, Entity entity) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<CollisionAttacker>, RefRO<AttackTarget>, RefRW<TargetPositionMovement>>().WithEntityAccess())
        {
            if (target.ValueRO.ActiveTarget == Entity.Null || !SystemAPI.HasComponent<LocalTransform>(target.ValueRO.ActiveTarget))
            {
                if (attacker.ValueRO.DestroyOnNoTarget)
                {
                    entityCommandBuffer.DestroyEntity(entity);
                }
                continue;
            }

            LocalTransform targetTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.ActiveTarget);

            float distanceToTarget = math.distance(targetTransform.Position, transform.ValueRO.Position);

            if (!movement.ValueRO.IsMovingToTarget && distanceToTarget > attacker.ValueRO.CollisionDistance)
            {
                float3 directionVector = transform.ValueRO.Position - targetTransform.Position;
                directionVector = math.normalize(directionVector);
                float3 newTargetPosition = targetTransform.Position + (directionVector * attacker.ValueRO.CollisionDistance);
                movement.ValueRW.Target = newTargetPosition;
                movement.ValueRW.IsMovingToTarget = true;
                continue;
            }

            attacker.ValueRW.AttackTimer += SystemAPI.Time.DeltaTime;

            if (attacker.ValueRO.AttackTimer >= attacker.ValueRO.AttackDelay)
            {
                RefRW<Health> health = SystemAPI.GetComponentRW<Health>(target.ValueRO.ActiveTarget);
                health.ValueRW.CurrentHealth -= attacker.ValueRO.Damage;
                health.ValueRW.OnHealthChanged = true;

                attacker.ValueRW.AttackTimer = 0f;

                if (attacker.ValueRO.DestroyOnHit)
                {
                    entityCommandBuffer.DestroyEntity(entity);
                }
            }
        }
    }
}
