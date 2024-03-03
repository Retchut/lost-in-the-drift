using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject renderOutputObj = GameObject.FindWithTag("RenderOutput");
        if (renderOutputObj)
        {
            Material renderTextureMaterial = renderOutputObj.GetComponent<RawImage>().material;
            if (renderTextureMaterial)
                renderTextureMaterial.SetFloat("_Intensity", 1.5f); // reset gamma correction
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene(1);
        }
    }
}
