using UnityEngine;
using UnityEngine.UI;

public class UIImageController : MonoBehaviour {
    
    public GameObject viewport;
    public GameObject contentView;
    
    public Image imageView;
    
    private const float AdjustOnZooming = 0.07f;
    private const float MaxScale = 3.0f;
    private const float MinScale = 0.9f;
    private const float ScaleFactor = 0.1f;
    
    private Vector2 screenSize;
    private Vector3 originalPos;

	// Use this for initialization
	void Start () {
        originalPos = contentView.GetComponent<RectTransform>().position;
        ConfigureContentView();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.touchCount == 1) {
            OnOneFingerTouch();
        } else if (Input.touchCount == 2) {
            OnTwoFingerTouch();
        }
	}
    
    public void setImageTexture(Texture2D texture) {
        Debug.Log("setImageTexture");
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        Image image = imageView.GetComponent<Image>();
        image.sprite = sprite;
        ConfigureContentView();
    }
    
    private void ConfigureContentView() {
        imageView.color = Color.white;
        
        Rect viewportRect = viewport.GetComponent<RectTransform>().rect;
        float viewportAspect = viewportRect.width / viewportRect.height;
        
        Rect spriteRect = imageView.sprite.rect;
        float spriteAspect = spriteRect.width / spriteRect.height;
        
        if (viewportAspect > spriteAspect) {
            contentView.GetComponent<RectTransform>().sizeDelta = new Vector2(viewportRect.height * spriteAspect, viewportRect.height);
        } else {
            contentView.GetComponent<RectTransform>().sizeDelta = new Vector2(viewportRect.width, viewportRect.width / spriteAspect);
        }
    }
    
    private void OnOneFingerTouch() {
        Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Began) {
            // Double Tap
            if (touch.tapCount == 2) {
                ScaleOnDoubleTap(touch);
            }
        }
    }
    
    private void OnTwoFingerTouch() {
        Touch touch0 = Input.GetTouch(0);
        Touch touch1 = Input.GetTouch(1);
        if (touch0.phase == TouchPhase.Moved && touch1.phase == TouchPhase.Moved) {
            ScaleOnPanning(touch0, touch1);
        } else if (touch0.phase == TouchPhase.Ended || touch1.phase == TouchPhase.Ended || 
                    touch0.phase == TouchPhase.Canceled || touch1.phase == TouchPhase.Canceled) {
            if (contentView.GetComponent<RectTransform>().localScale.x < Vector3.one.x || contentView.GetComponent<RectTransform>().localScale.y < Vector3.one.y) {
                ScaleAndMoveToOriginal();
            }
        }
    }
    
    private void ScaleOnPanning(Touch touch0, Touch touch1) {
        Vector2 centerPos = (touch0.position + touch1.position) / 2;
        centerPos = contentView.GetComponent<RectTransform>().position;
        
        Vector2 currDist = touch0.position - touch1.position;
        Vector2 prevDist = (touch0.position - touch0.deltaPosition) - (touch1.position - touch1.deltaPosition);
        float touchDelta = currDist.magnitude - prevDist.magnitude;
        Vector3 scale = MakeScaleForZoom(touchDelta);
        
        ScaleAndMoveToPosition(scale, centerPos);
    }
    
    private void ScaleOnDoubleTap(Touch touch) {
        if (contentView.GetComponent<RectTransform>().localScale == Vector3.one) {            
            Vector3 maxScale = new Vector3(MaxScale, MaxScale, 1);
            Vector3 position = new Vector3(touch.position.x, touch.position.y, 0);
            ScaleAndMoveToPosition(maxScale, position);
        } else {
            ScaleAndMoveToOriginal();
        }
    }
    
    private void ScaleAndMoveToPosition(Vector3 scale, Vector3 position) {
        Vector3 delta = contentView.GetComponent<RectTransform>().position - position;
        Vector3 pos = new Vector3(delta.x * contentView.GetComponent<RectTransform>().localScale.x, delta.y * contentView.GetComponent<RectTransform>().localScale.y, 0);
        contentView.GetComponent<RectTransform>().position += pos;
        contentView.GetComponent<RectTransform>().localScale = scale;
    }
    
    private void ScaleAndMoveToOriginal() {
        ScaleAndMoveToPosition(Vector3.one, originalPos);
    }
    
    private Vector3 MakeScaleForZoom(float deltaMagnitude) {
        float scaleFactor = deltaMagnitude * ScaleFactor * AdjustOnZooming;
        Vector3 localScale = contentView.GetComponent<RectTransform>().localScale; 
        Vector3 scale = new Vector3(localScale.x + scaleFactor, localScale.y + scaleFactor, 1);
        scale.x = (scale.x > MaxScale) ? MaxScale : scale.x;
        scale.y = (scale.y > MaxScale) ? MaxScale : scale.y;
        scale.x = (scale.x < MinScale) ? MinScale : scale.x;
        scale.y = (scale.y < MinScale) ? MinScale : scale.y;
        return scale;
    }
}
