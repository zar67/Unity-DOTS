using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class UnitSelectionManager : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mouseWorldPosition = MousePositionManager.Instance.GetWorldPosition();

            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            /// <see cref="Allocator"/> determines the lifetime of the memory.
            /// <see cref="Allocator.Temp"/> means the memory will be cleaned up at the end of the frame.
            EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<UnitMovement>().Build(entityManager);

            NativeArray<UnitMovement> unitMovementArray = entityQuery.ToComponentDataArray<UnitMovement>(Allocator.Temp);
            for (int i = 0; i < unitMovementArray.Length; i++)
            {
                UnitMovement unitMovement = unitMovementArray[i];
                unitMovement.TargetPosition = mouseWorldPosition;
                unitMovementArray[i] = unitMovement;
            }

            // Update the data on all the components.
            // This is needed because the components are structs (a type value) not a reference value so any changes will apply to the copy of the data not the original data.
            entityQuery.CopyFromComponentDataArray(unitMovementArray);
        }
    }
}
