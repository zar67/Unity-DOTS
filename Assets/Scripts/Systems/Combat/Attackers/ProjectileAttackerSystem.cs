using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateBefore(typeof(EndSimulationEntityCommandBufferSystem))]
partial struct ProjectileAttackerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach ((RefRW<LocalTransform> transform, RefRW<ProjectileAttacker> attacker, RefRO<AttackTarget> target, RefRW<TargetPositionMovement> movement) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<ProjectileAttacker>, RefRO<AttackTarget>, RefRW<TargetPositionMovement>>())
        {
            if (target.ValueRO.ActiveTarget != Entity.Null && SystemAPI.HasComponent<LocalTransform>(target.ValueRO.ActiveTarget))
            {
                LocalTransform targetTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.ActiveTarget);

                float distanceToTarget = math.distance(targetTransform.Position, transform.ValueRO.Position);

                if (!movement.ValueRO.IsMovingToTarget && 
                    (distanceToTarget >= attacker.ValueRO.FiringDistanceFromTarget + attacker.ValueRO.FiringDistanceReachedDistance ||
                     distanceToTarget <= attacker.ValueRO.FiringDistanceFromTarget - attacker.ValueRO.FiringDistanceReachedDistance))
                {
                    float3 directionVector = transform.ValueRO.Position - targetTransform.Position;
                    directionVector = math.normalize(directionVector);
                    float3 newTargetPosition = targetTransform.Position + (directionVector * attacker.ValueRO.FiringDistanceFromTarget);
                    movement.ValueRW.Target = newTargetPosition;
                    movement.ValueRW.IsMovingToTarget = true;
                    continue;
                }

                float3 aimDirection = targetTransform.Position - transform.ValueRO.Position;
                aimDirection = math.normalize(aimDirection);

                var targetRotation = quaternion.LookRotation(aimDirection, math.up());
                transform.ValueRW.Rotation = math.slerp(transform.ValueRO.Rotation, targetRotation, SystemAPI.Time.DeltaTime * movement.ValueRO.TurnSpeed);

                attacker.ValueRW.AttackTimer += SystemAPI.Time.DeltaTime;

                if (attacker.ValueRO.AttackTimer >= attacker.ValueRO.AttackDelay)
                {
                    Entity projectile = state.EntityManager.Instantiate(attacker.ValueRO.ProjectileEntity);
                    float3 rotatedOffset = math.rotate(transform.ValueRO.Rotation, attacker.ValueRO.ProjectileOffset);
                    SystemAPI.SetComponent(projectile, LocalTransform.FromPosition(transform.ValueRO.Position + rotatedOffset));

                    RefRW<TargetEntityMovement> projectileMovement = SystemAPI.GetComponentRW<TargetEntityMovement>(projectile);
                    projectileMovement.ValueRW.Target = target.ValueRO.ActiveTarget;

                    RefRW<AttackTarget> projectileAttackTarget = SystemAPI.GetComponentRW<AttackTarget>(projectile);
                    projectileAttackTarget.ValueRW.ActiveTarget = target.ValueRO.ActiveTarget;

                    attacker.ValueRW.AttackTimer = 0f;
                }
            }
        }
    }
}
