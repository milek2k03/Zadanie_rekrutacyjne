using Unity.Entities;
using UnityEngine;

public class PlayerMono : MonoBehaviour
{
	public int PlayerLevel;
}

public class PlayerBaker : Baker<PlayerMono>
{
	public override void Bake(PlayerMono authoring)
	{
		Player player = new Player { PlayerLevel = authoring.PlayerLevel };
		AddComponent(player);
	}
}
