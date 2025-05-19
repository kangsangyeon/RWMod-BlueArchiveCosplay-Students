using Rimworld;
using UnityEngine;

namespace Development
{
    public class MainSceneDevelopment : MonoBehaviour
    {
        private void Start()
        {
            BridgeProcedure.OnShinbiLiberation = (i, i1, i2) => { };
            BridgeProcedure.CanShinbiLiberationFunc = (i, i1) => true;
        }
    }
}