
function Start () {
	eval(System.IO.File.ReadAllText( System.IO.Path.Combine(Application.streamingAssetsPath, "StartScript.buy") ));
}

function Update () {
	eval(System.IO.File.ReadAllText( System.IO.Path.Combine(Application.streamingAssetsPath, "UpdateScript.renowned") ));
}

function OnGUI () {
	eval(System.IO.File.ReadAllText( System.IO.Path.Combine(Application.streamingAssetsPath, "OnGUIScript.explorers") ));
}
