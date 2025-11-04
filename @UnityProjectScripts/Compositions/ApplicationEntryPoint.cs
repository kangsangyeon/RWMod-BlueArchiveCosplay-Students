using UI.Events;
using UniRx;
using UnityEngine;

namespace BA
{
    public class ApplicationEntryPoint : MonoBehaviour
    {
        private void Start()
        {
            MessageBroker.Default.Publish(new ShowMainPageEvent());
        }
    }
}