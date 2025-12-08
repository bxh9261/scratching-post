using UnityEngine;

public class CatWalker : MonoBehaviour
{
    public CatMover cat;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            cat.GoToLocation(0);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            cat.GoToLocation(1);
    }
}
