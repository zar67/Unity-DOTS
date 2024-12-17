using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

partial struct TargetBasedMovementSystem : ISystem
{
    /// Cannot use <see cref="BurstCompileAttribute"/> with managed (class) data like <see cref="UnityEngine.MonoBehaviour"/>.
    /// This will give an error to Unity if both are used, but will not stop compilation.
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var unitMovementJob = new TargetBasedMovementJob()
        {
            DeltaTime = SystemAPI.Time.DeltaTime
        };

        unitMovementJob.ScheduleParallel();
    }
}

/// Jobs run on multiple threads in parallel.
/// If there are not that many entities found by the query defined by the arguments given to the execute function) then the job may only run on one thread.
[BurstCompile]
public partial struct TargetBasedMovementJob : IJobEntity
{
    public float DeltaTime;

    /// Use <see cref="ref"/> for arguments that need to be both readable and writeable.
    /// Use <see cref="in"/> for arguments that only need to be readable.
    public void Execute(ref LocalTransform transform, ref PhysicsVelocity velocity, in TargetBasedMovement movement)
    {
        float3 moveDirection = movement.TargetPosition - transform.Position;
        if (math.lengthsq(moveDirection) > movement.TargetDistance)
        {
            moveDirection = math.normalize(moveDirection);
            var targetRotation = quaternion.LookRotation(moveDirection, math.up());
            transform.Rotation = math.slerp(transform.Rotation, targetRotation, DeltaTime * movement.TurnSpeed);

            velocity.Linear = moveDirection * movement.MoveSpeed;
            velocity.Angular = float3.zero;
        }
        else
        {
            velocity.Linear = float3.zero;
        }
    }
}