using Unity.Entities;
using Unity.NetCode;

public struct Player : IComponentData
{
    [GhostField] public int PlayerLevel;
}
