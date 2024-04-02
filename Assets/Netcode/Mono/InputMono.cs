using Unity.Entities;
using UnityEngine;

public class InputMono : MonoBehaviour
{

}

[DisallowMultipleComponent]
public class InputBaker : Baker<InputMono>
{
    public override void Bake(InputMono authoring)
    {
        AddComponent(GetEntity(TransformUsageFlags.Dynamic), new InputComponent());
    }
}
