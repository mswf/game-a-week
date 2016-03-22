// press F6 for a hard reset of the entire scene

// defining the spotlight object
var light : GameObject = new GameObject();
light.name = "Directional light";
light.AddComponent(Light);
light.GetComponent(Light).type = LightType.Directional;
light.transform.rotation = Quaternion.Euler(50, 40, 76);
light.GetComponent(Light).intensity = 0;

// set the ambient light to black
RenderSettings.ambientLight = Camera.main.backgroundColor;
