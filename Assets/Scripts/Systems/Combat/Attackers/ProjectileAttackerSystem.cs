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
        foreach ((RefRO<LocalTransform> transform, RefRW<ProjectileAttacker> attacker, RefRO<AttackTarget> target) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<ProjectileAttacker>, RefRO<AttackTarget>>())
        {
            if (target.ValueRO.ActiveTarget != Entity.Null)
            {
                attacker.ValueRW.AttackTimer += SystemAPI.Time.DeltaTime;

                if (attacker.ValueRO.AttackTimer >= attacker.ValueRO.AttackDelay)
                {
                    Entity projectile = state.EntityManager.Instantiate(attacker.ValueRO.ProjectileEntity);
                    var rotatedOffset = math.rotate(transform.ValueRO.Rotation, attacker.ValueRO.ProjectileOffset);
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
