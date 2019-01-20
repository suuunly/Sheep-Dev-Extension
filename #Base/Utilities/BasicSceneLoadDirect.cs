using UnityEngine;
using UnityEngine.SceneManagement;

public class BasicSceneLoadDirect : MonoBehaviour
{

	public string Scene;

	public void LoadScene()
	{
		SceneManager.LoadScene(Scene);
	}
}
