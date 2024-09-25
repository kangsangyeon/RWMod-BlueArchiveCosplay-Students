using UnityEngine;

public class Contents : MonoBehaviour
{
    public ContentsAccessor Accessor;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var _psPosition = Accessor.UICamera.ScreenToWorldPoint(Input.mousePosition);
            _psPosition.z = Accessor.UIParticleHolder.transform.position.z;
            var _ps = Instantiate(GameResource.TouchFxPrefab, Accessor.UIParticleHolder.transform);
            _ps.transform.position = _psPosition;
        }
    }
}