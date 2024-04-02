
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[UpdateInGroup((typeof(GhostInputSystemGroup)))]
public partial struct InputSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Spawner>();
        state.RequireForUpdate<InputComponent>();
        state.RequireForUpdate<NetworkId>();
    }

    public void OnDestroy(ref SystemState state)
    {

    }

    public void OnUpdate(ref SystemState state)
    {
        bool left = Input.GetKey(KeyCode.A);
        bool right = Input.GetKey(KeyCode.D);
        bool jump = Input.GetKey(KeyCode.Space);

        foreach (var input in SystemAPI.Query<RefRW<InputComponent>>().WithAll<GhostOwnerIsLocal>())
        {
            input.ValueRW = default;
            if(left)
                input.ValueRW.Vertical -= 1;
            if(right)
                input.ValueRW.Vertical += 1;
            if(jump)
                input.ValueRW.Vertical += 1;
        }
    }
}