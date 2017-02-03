using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	// Use this for initialization
	public void LoadLevel () {
		SceneManager.LoadScene("Garrage Outdoor");
	}

	public void Credits () {
		SceneManager.LoadScene("Credits");
	}
	public void Back () {
		SceneManager.LoadScene("Main Menu");
	}
	

}
