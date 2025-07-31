# Chrunity
(Today, we are combining CHROMA and UNITY.)

A Unity tool allowing placement of Chroma objects (Geometry) in the Scene view.

> Warning: It is recommended you have some experience ATLEAST with the Unity UI and Components of GameObjects before using this tool.


## Installation

> Note: It is recommended for you to be using **Unity 2019.4.28**.

1. Download the Chrunity Repository as a .zip [here](https://github.com/UGEcko/Chrunity/archive/refs/heads/main.zip).

2. Open your Unity project, and copy the Chrunity_Unity folder to your Assets folder.

3. You're done! Read the [Utility Usage](https://github.com/UGEcko/Chrunity?tab=readme-ov-file#utilityusage(unity)) and [Script Usage](https://github.com/UGEcko/Chrunity?tab=readme-ov-file#scriptusage) sections to continue.


## Utility Usage (Unity)

This section covers the usage of Chrunity inside of the Unity Editor. This is where you will be placing objects and exporting them.

### Adding Chroma Objects
Adding Chroma Objects to your scene is fairly simple to do. Each Chroma Object is defined inside of a component on a GameObject:
<br>
<img width="648" height="167" alt="image" src="https://github.com/user-attachments/assets/c067b4fc-4831-475d-8249-b52597db15e4" />


* **Object Type** : The mesh type of the object (Sphere, Capsule, Cylinder, Cube, Plane, Quad, Triangle)

* **Object Name Override** : Doesn't do anything by itself.. Primarily used for filtering or identifying objects when processing the objects manually.

* **Object Material** : The material of the Geometry Object (Material MUST be defined in your script, not here.)

* **Track** : The track of the Geometry object

* **LightID / LightType** : Set the lightID and Type of the object (This will be skipped in the data processor if the material isnt a light shader.)

<br>

Note: Each object that you wish to be represented as a Geometry Object must have this component on them.. Otherwise they won't be exported.


### Using the Exporter

> The Exporter is where you export the JSON file that contains the data needed to port over the Chroma objects from Unity to your script. It also contains a few utilities to speed up your workflow.

<img width="249" height="365" alt="image" src="https://github.com/user-attachments/assets/685c19ec-b6b8-46b8-ad7f-70a1fa0689fc" />


Upon installing the Utility into your Unity project, you will be provided with a new tab: <br><img width="589" height="40" alt="image" src="https://github.com/user-attachments/assets/775d85e8-0e68-489a-8526-899c237b78f4" />


<br>

Click `Chrunity > Open Chrunity` to open the Chrunity UI.

<hr>

In order to export objects, you must provide an output directory. Click "Find output folder" to do so.

After finding your output directory, you are now able to click the "Export" button. This will write a file to your directory called `chroma_objects.json`.


### Using the Utilities

The Utilities are pretty simple, each one allows you to set the Track, Material, or LightType on selected objects. This could be *very* helpful for those who are looking to edit a large amount of Chroma objects at once.


## Script Usage (ReMapper)

The provided script is what processes the data you export from the Unity Utility. You have the choice to let the script do all of the work, or you can iterate through each exported object and do it yourself if you have other needs.

### Installation

1. Ensure you have your ReMapper script fully setup. 
2. Import the script with the following import statement: <br> `import { Chrunity } from 'https://raw.githubusercontent.com/UGEcko/Chrunity/refs/heads/main/ChromaHelper.ts'`.

### Usage

To use the script, you must create an instance of the Chrunity class:

```ts
const chrunity = new Chrunity(map); // Pass in the 'map' constant from your ReMapper script
```

By default this will work, however if you are planning on renaming the files or manually processing the object data then look below:

### Renaming Objects file

Note: You may rename the file that is exported but you must declare it in the Chrunity constructor:

```ts
const chrunity = new Chrunity(map, "newFileName.json");
```

### Manual Processing

If you are looking to manually process the Object file for whatever reason, you can do so by turning off processingOnLoad:

```ts
const chrunity = new Chrunity(map, undefined, false); // Set third parameter to false, disabling automatic processing when the class initializes.
```

Now, you can use the `Objects` (Or `DummyObjects` for objects declared as dummy object types) array to iterate over each exported object

Here is the structure of each Chroma Object:

```
{
    Name: string,
    MeshType: MeshType, // (Cube, Sphere, etc..)
    Material: string,
    Transform: ObjectTransform, // Provides Pos, LocalRot/Rot in Euler, and Scale.
    LightID: number,
    LightType: number,
    Track: string
}
```

Below is an example of iterating through each dummy object, using their names as a ChromaID and setting the transform of the Environment object:

```ts
const chrunity = new Chrunity(map, undefined, false);

chrunity.DummyObjects?.forEach(x=> {
    const chromaID = x.Name;
    rm.environment(map, {
        id: chromaID,
        duplicate: 1,
        position: x.Transform.Position,
        rotation: x.Transform.Rotation,
        scale: x.Transform.Scale
    })
})
```
