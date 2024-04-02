using Unity.Entities;
using Unity.NetCode;

using UnityEngine;

public class SpawnerMono : MonoBehaviour
{
	public GameObject PlayerPrefab;
}

public class SpawnerBaker : Baker<SpawnerMono>
{
	public override void Bake(SpawnerMono authoring)
	{
		Spawner spawner = default;
		spawner.TheBoyWhoLived = GetEntity(authoring.PlayerPrefab);
		AddComponent(spawner);
	}
}
