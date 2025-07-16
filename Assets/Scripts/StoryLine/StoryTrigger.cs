using UnityEngine;
using UnityEngine.SceneManagement;

namespace StoryLine
{
    public class StoryTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name, LoadSceneMode.Additive);
            //Play cutscene
            
        }
    }
}