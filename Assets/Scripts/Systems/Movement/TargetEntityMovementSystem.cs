using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

partial struct TargetEntityMovementSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach ((RefRW<LocalTransform> transform, RefRW<PhysicsVelocity> velocity, RefRO<TargetEntityMovement> movement) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<PhysicsVelocity>, RefRO<TargetEntityMovement>>())
        {
            if (movement.ValueRO.Target != Entity.Null && SystemAPI.HasComponent<LocalTransform>(movement.ValueRO.Target))
            {
                LocalTransform targetTransform = SystemAPI.GetComponent<LocalTransform>(movement.ValueRO.Target);
                float3 moveDirection = targetTransform.Position - transform.ValueRO.Position;
                if (math.distancesq(targetTransform.Position, transform.ValueRO.Position) > movement.ValueRO.ReachedDistance)
                {
                    moveDirection = math.normalize(moveDirection);
                    var targetRotation = quaternion.LookRotation(moveDirection, math.up());
                    transform.ValueRW.Rotation = math.slerp(transform.ValueRO.Rotation, targetRotation, SystemAPI.Time.DeltaTime * movement.ValueRO.TurnSpeed);

                    velocity.ValueRW.Linear = moveDirection * movement.ValueRO.MoveSpeed;
                    velocity.ValueRW.Angular = float3.zero;
                }
                else
                {
                    velocity.ValueRW.Linear = float3.zero;
                }
            }
        }
    }
}