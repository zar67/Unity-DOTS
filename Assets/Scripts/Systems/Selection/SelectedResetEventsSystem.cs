using Unity.Burst;
using Unity.Entities;

partial struct SelectedResetEventsSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (RefRW<Selected> selected in SystemAPI.Query<RefRW<Selected>>().WithPresent<Selected>())
        {
            selected.ValueRW.OnSelected = false;
            selected.ValueRW.OnDeselected = false;
        }
    }
}
