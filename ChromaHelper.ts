import * as rm from "https://deno.land/x/remapper@4.2.0/src/mod.ts"

interface ChromaObject {
    Name: string,
    MeshType: MeshType,
    Material: string,
    Transform: ObjectTransform,
    LightID: number,
    LightType: number,
    Track: string
}

interface DummyObject {
    Name: string,
    Transform: ObjectTransform
}

type MeshType =
    "Dummy" |
    "Cube" |
    "Sphere" |
    "Capsule" |
    "Cylinder" |
    "Plane" |
    "Quad" |
    "Triangle"

type Vector3 = [number, number, number]

interface ObjectTransform {
    Position: Vector3,
    Rotation: Vector3, // Set in Euler
    LocalRotation: Vector3, // Set in Euler
    Scale: Vector3
}


export class Chrunity {
    private map: rm.AbstractDifficulty;
    Objects?: ChromaObject[]
    DummyObjects?: DummyObject[]

/**
 * @param {rm.AbstractDifficulty} map
 * @param {string} [input="chroma_objects.json"] The filename of the objects data file. If you renamed the file then provide the file name here. Otherwise leave this undefined.
 * @param {boolean} [processOnLoad=false] Choose whether or not to allow Chrunity to process the objects for you on initialization. If you are processing the objects manually: set this to false and iterate through the `Objects` array.
 * @memberof Chrunity
 */
constructor(map: rm.AbstractDifficulty, input: string = "chroma_objects.json", processOnLoad: boolean = true) {
        this.map = map;
        const file = Deno.readTextFileSync(Deno.cwd() + "\\" + input);
        const json = JSON.parse(file) as ChromaObject[];
        this.Objects = json;

        // --------

        if (!processOnLoad) return;
        
        this.Objects.forEach(object => {
            if (object.MeshType == "Dummy") {
                this.DummyObjects?.push(object as DummyObject);
            }
            const type = object.MeshType.toString();
            
            let isLight = false;
            if (object.Material != "") {
                const mat = map.geometryMaterials[object.Material];
                if(mat.shader != undefined && mat.shader.toLowerCase().includes("light")) isLight = true;
            }

            rm.geometry(map, {
                position: object.Transform.Position,
                localRotation: object.Transform.Rotation,
                scale: object.Transform.Scale,
                type: type as rm.GeoType,
                material: object.Material ?? undefined,
                track: object.Track ?? undefined,
                lightID: isLight ? object.LightID : undefined,
                lightType: isLight ? object.LightType : undefined,
            })
        })
    }

    public isLight(object: ChromaObject): boolean {
        let isLight = false;
            if (object.Material != "") {
                const mat = this.map.geometryMaterials[object.Material];
                if(mat.shader != undefined && mat.shader.toLowerCase().includes("light")) isLight = true;
            }
        return isLight;
    }
}
