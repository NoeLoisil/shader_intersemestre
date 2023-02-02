using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class FractalMaster : MonoBehaviour
{
    public ComputeShader fractalShader;
    RenderTexture target;
    public Camera cam;
    public int iteration = 2;
    public Vector3 espace = new Vector3(10,10,10);
    void Start()
    {
        Application.targetFrameRate = 60;
    }
    
    void Init ()
    {
        
    }

    // Animate properties
    void Update () 
    {
        if (Application.isPlaying && false)
        {
            cam.transform.position = new Vector3(1.5f, 0.1f, 0);
            cam.transform.RotateAround(new Vector3(0, 1, 0), Time.time*0.01f);
            Quaternion rot = cam.transform.rotation;
            rot.eulerAngles = new Vector3(0, -90-Time.time * 0.01f*180/Mathf.PI, 0);
            cam.transform.rotation = rot;
        }
    }

    void OnRenderImage (RenderTexture source, RenderTexture destination)
    {
        Init ();
        InitRenderTexture ();
        SetParameters ();

        int threadGroupsX = Mathf.CeilToInt (cam.pixelWidth / 8.0f);
        int threadGroupsY = Mathf.CeilToInt (cam.pixelHeight / 8.0f);
        fractalShader.Dispatch (0, threadGroupsX, threadGroupsY, 1);

        Graphics.Blit (target, destination);
    }

    void SetParameters ()
    {
        fractalShader.SetTexture (0, "Destination", target);
        fractalShader.SetInt ("iteration", iteration);
        fractalShader.SetVector("espace", espace);
        fractalShader.SetFloat("time", Time.time);
        fractalShader.SetMatrix("_CameraToWorld", cam.cameraToWorldMatrix);
        fractalShader.SetMatrix("_CameraInverseProjection", cam.projectionMatrix.inverse);
    }

    void InitRenderTexture ()
    {
        if (target == null || target.width != cam.pixelWidth || target.height != cam.pixelHeight)
        {
            if (target != null)
            {
                target.Release ();
            }
            target = new RenderTexture (cam.pixelWidth, cam.pixelHeight, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            target.enableRandomWrite = true;
            target.Create ();
        }
    }
}