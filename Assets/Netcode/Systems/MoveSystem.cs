using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

[UpdateInGroup(typeof(PredictedFixedStepSimulationSystemGroup))]
[BurstCompile]
public partial struct MoveSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        var builder = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<InputComponent>()
            .WithAll<Simulate>();
        state.RequireAnyForUpdate(state.GetEntityQuery(builder));
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var moveJob = new MoveJob
        {
            Speed = 4 * SystemAPI.Time.DeltaTime
        };

        state.Dependency = moveJob.ScheduleParallel(state.Dependency);
    }
}

[BurstCompile]
public partial struct MoveJob : IJobEntity
{
    public float Speed;
    public void Execute(InputComponent input, ref LocalTransform transform)
    {
        var move = new float2(input.Horizontal, input.Vertical);
        move = math.normalize(move) * Speed;
        transform.Position += new float3(move.x, 0, move.y);
    }
}