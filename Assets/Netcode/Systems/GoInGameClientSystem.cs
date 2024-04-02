
using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.NetCode;
using UnityEngine;

[BurstCompile]
[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
public partial struct GoInGameClientSystem : ISystem
{
	[BurstCompile]
	public void OnCreate(ref SystemState state)
	{
		state.RequireForUpdate<Spawner>();
		var builder = new EntityQueryBuilder(Allocator.Temp)
			.WithAll<NetworkId>()
			.WithNone<NetworkStreamInGame>();
		state.RequireForUpdate(state.GetEntityQuery(builder));
	}

	[BurstCompile]
	public void OnDestroy(ref SystemState state)
	{

	}

	[BurstCompile]
	public void OnUpdate(ref SystemState state)
	{
		var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
		foreach (var (id, entity) in SystemAPI.Query<RefRO<NetworkId>>().WithEntityAccess().WithNone<NetworkStreamInGame>())
		{
			commandBuffer.AddComponent<NetworkStreamInGame>(entity);
			var req = commandBuffer.CreateEntity();
			commandBuffer.AddComponent<GoInGameRPC>(req);
			commandBuffer.AddComponent(req, new SendRpcCommandRequest { TargetConnection = entity });
		}

		commandBuffer.Playback(state.EntityManager);
	}
}

[BurstCompile]
[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct GoInGameServerSystem : ISystem
{
	public ComponentLookup<NetworkId> NetworkId;

	[BurstCompile]
	public void OnCreate(ref SystemState state)
	{
		state.RequireForUpdate<Spawner>();
		var builder = new EntityQueryBuilder(Allocator.Temp)
			.WithAll<GoInGameRPC>()
			.WithAll<ReceiveRpcCommandRequest>();
		state.RequireForUpdate(state.GetEntityQuery(builder));

		NetworkId = state.GetComponentLookup<NetworkId>(true);

	}

	[BurstCompile]
	public void OnDestroy(ref SystemState state)
	{

	}

	[BurstCompile]
	public void OnUpdate(ref SystemState state)
	{
		Entity prefab = SystemAPI.GetSingleton<Spawner>().TheBoyWhoLived;
		var worldName = state.WorldUnmanaged.Name;
		var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
		NetworkId.Update(ref state);

		foreach (var (reqSrc, reqEntity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>>().WithAll<GoInGameRPC>().WithEntityAccess())
		{
			commandBuffer.AddComponent<NetworkStreamInGame>(reqSrc.ValueRO.SourceConnection);
			var networkId = NetworkId[reqSrc.ValueRO.SourceConnection];
			var player = commandBuffer.Instantiate(prefab);
			commandBuffer.SetComponent(player, new GhostOwner { NetworkId = networkId.Value });
			Debug.Log($"{worldName} connecting {networkId.Value}");
			commandBuffer.AppendToBuffer(reqSrc.ValueRO.SourceConnection, new LinkedEntityGroup { Value = player });
			commandBuffer.DestroyEntity(reqEntity);
		}

		commandBuffer.Playback(state.EntityManager);
	}
}

