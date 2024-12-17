using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct TargetEntityMovementSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach ((RefRO<TargetEntityMovement> entityMovement, RefRW<TargetPositionMovement> positionMovement) in SystemAPI.Query<RefRO<TargetEntityMovement>, RefRW<TargetPositionMovement>>())
        {
            if (entityMovement.ValueRO.Target != Entity.Null && SystemAPI.HasComponent<LocalTransform>(entityMovement.ValueRO.Target))
            {
                LocalTransform targetPosition = SystemAPI.GetComponent<LocalTransform>(entityMovement.ValueRO.Target);
                positionMovement.ValueRW.Target = targetPosition.Position;
            }
        }
    }
}