using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	// Use this for initialization
	public void LoadLevel () {
		SceneManager.LoadScene("Level01");
	}
	public void Credits () {
		SceneManager.LoadScene("Credits");
	}
	public void Back () {
		SceneManager.LoadScene("MainMenu");
	}
	

}
