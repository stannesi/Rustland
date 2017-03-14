public class SceneReaction : Reaction {

	public string sceneName;
	public string startingPointInLoadedScene;
	public SavaData playerSavaData;

	private SceneController sceneController;

	// Use this for initialization
	protected override void SpecificInit()
	{
		sceneController = FindObjectOfType<SceneController>();
	}

	// Update is called once per frame
	protected override void ImmediateReaction ()
	{
		//playerSaveData.Save (PlayerMovement.startingPositionKey,
		sceneController.FadeAndLoadScene (this);
	}
}
