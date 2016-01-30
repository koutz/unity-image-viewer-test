using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImageDownloader : MonoBehaviour {

    const string ImageUrl = "http://hmcreation.net/unity/images/";
    
	// Use this for initialization
	void Start () {
        string imageUrl = ImageUrl + "image0.jpg"; 
        StartCoroutine("DownloadImage", imageUrl);
	}
	
	public void OnClickDownloadImage(string imageName) {
        string imageUrl = ImageUrl + imageName;
        StartCoroutine("DownloadImage", imageUrl);
    }
    
    private IEnumerator DownloadImage(string imageUrl) {
        Debug.Log("imageUrl=" + imageUrl);
        
        WWW www = new WWW(imageUrl);
        yield return www;
        
        GameObject scrollView = GameObject.Find("ScrollView");
        UIImageController controller = scrollView.GetComponent<UIImageController>();
        controller.setImageTexture(www.textureNonReadable);
    }
}
