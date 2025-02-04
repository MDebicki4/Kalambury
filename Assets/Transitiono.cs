using UnityEngine;

namespace EasyTransition
{
    public class Transitiono : MonoBehaviour
    {
        public TransitionSettings transition;
        public float startDelay;

        private TransitionManager manager;

        // Method to play the transition
        public void PlayTransition()
        {
            // Get the instance of the TransitionManager
            manager = TransitionManager.Instance();

            // Trigger the transition
            manager.Transition(transition, startDelay);
        }
    }
}