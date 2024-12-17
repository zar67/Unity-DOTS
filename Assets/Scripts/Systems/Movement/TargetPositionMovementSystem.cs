using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

partial struct TargetPositionMovementSystem : ISystem
{
    /// Cannot use <see cref="BurstCompileAttribute"/> with managed (class) data like <see cref="UnityEngine.MonoBehaviour"/>.
    /// This will give an error to Unity if both are used, but will not stop compilation.
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var movementJob = new TargetPositionMovementJob()
        {
            DeltaTime = SystemAPI.Time.DeltaTime
        };

        movementJob.ScheduleParallel();
    }
}

/// Jobs run on multiple threads in parallel.
/// If there are not that many entities found by the query defined by the arguments given to the execute function) then the job may only run on one thread.
[BurstCompile]
public partial struct TargetPositionMovementJob : IJobEntity
{
    public float DeltaTime;

    /// Use <see cref="ref"/> for arguments that need to be both readable and writeable.
    /// Use <see cref="in"/> for arguments that only need to be readable.
    public void Execute(ref LocalTransform transform, ref PhysicsVelocity velocity, ref TargetPositionMovement movement)
    {
        float3 moveDirection = movement.Target - transform.Position;
        if (math.lengthsq(moveDirection) > movement.ReachedDistance)
        {
            moveDirection = math.normalize(moveDirection);
            var targetRotation = quaternion.LookRotation(moveDirection, math.up());
            transform.Rotation = math.slerp(transform.Rotation, targetRotation, DeltaTime * movement.TurnSpeed);

            velocity.Linear = moveDirection * movement.MoveSpeed;
            velocity.Angular = float3.zero;
        }
        else
        {
            movement.IsMovingToTarget = false;
            velocity.Linear = float3.zero;
        }
    }
}