using UnityEngine;

namespace BA
{
    public class Contents : MonoSingleton<Contents>
    {
        public ContentsAccessor Accessor;

        private void Start()
        {
            GameResource.FullshotRT.Clear();
            GameResource.TransitionRT.Clear();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var psPosition = Accessor.UICamera.ScreenToWorldPoint(Input.mousePosition);
                psPosition.z = Accessor.UIParticleHolder.transform.position.z;
                var ps = Instantiate(GameResource.TouchFxPrefab, Accessor.UIParticleHolder.transform);
                ps.transform.position = psPosition;
            }
        }
    }
}