using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour {

   	// Use this for initialization
	void Start () {
        Debug.Log("Start: ActiveScene=" + SceneManager.GetActiveScene().name);
    }
    
    public void OnClick() {
        Debug.Log("OnClick");
        if (SceneManager.GetActiveScene().name == "SpriteImageScene") {
            SceneManager.LoadScene("UIImageScene");
        } else if (SceneManager.GetActiveScene().name == "UIImageScene") {
            SceneManager.LoadScene("SpriteImageScene");
        }
    }
}
