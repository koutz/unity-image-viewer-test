using UnityEngine;

public class TouchManager : MonoBehaviour {
    
    private const float AdjustOnPanning = 0.01f;
    private const float AdjustOnZooming = 0.07f;
    private const float MaxScale = 3.0f;
    private const float MinScale = 0.8f;
    private const float ScaleFactor = 0.1f;
    
    private Vector2 screenSize;
    private Vector3 originalPos;
    private Vector3 parentOriginalPos;
    private GameObject parentObject;

	// Use this for initialization
	void Start () {
        screenSize = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        originalPos = transform.position;
        parentOriginalPos = new Vector3(originalPos.x * -1, originalPos.y * -1, originalPos.z);
        
        parentObject = new GameObject("ParentObject");
        parentObject.transform.parent = transform.parent;
        parentObject.transform.position = parentOriginalPos;
        transform.parent = parentObject.transform;  
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.touchCount == 1) {
            OnOneFingerTouch();
        } else if (Input.touchCount == 2) {
            OnTwoFingerTouch();
        }
	}
    
    private void OnOneFingerTouch() {
        Touch touch = Input.GetTouch(0);
            
        if (touch.phase == TouchPhase.Began) {
            // Double Tap
            if (touch.tapCount == 2) {
                ScaleOnDoubleTap(touch);
            }
        } else if (touch.phase == TouchPhase.Moved) {
            // Drag
            Vector3 delta = touch.deltaPosition;
            Vector3 pos = transform.position + delta * AdjustOnPanning;
            MoveToAdjustedPositionAccordingToScreen(pos);
        }
    }
    
    private void OnTwoFingerTouch() {
        Touch touch0 = Input.GetTouch(0);
        Touch touch1 = Input.GetTouch(1);
        
        if (touch0.phase == TouchPhase.Moved && touch1.phase == TouchPhase.Moved) {
            ScaleOnPanning(touch0, touch1);
        } else if (touch0.phase == TouchPhase.Ended || touch1.phase == TouchPhase.Ended || 
                    touch0.phase == TouchPhase.Canceled || touch1.phase == TouchPhase.Canceled) {
            if (parentObject.transform.localScale.x < 1 || parentObject.transform.localScale.y < 1) {
                ScaleAndMoveToOriginal();
            } else {
                Vector3 pos = transform.position;
                MoveToAdjustedPositionAccordingToScreen(pos);
            }
        }
    }
    
    private void ScaleOnPanning(Touch touch0, Touch touch1) {
        Vector2 centerPos = (touch0.position + touch1.position) / 2;
        centerPos = Camera.main.ScreenToWorldPoint(centerPos);
        
        Vector2 currDist = touch0.position - touch1.position;
        Vector2 prevDist = (touch0.position - touch0.deltaPosition) - (touch1.position - touch1.deltaPosition);
        float touchDelta = currDist.magnitude - prevDist.magnitude;
        
        Vector3 scale = MakeScaleForZoom(touchDelta);
        ScaleAndMoveToPosition(scale, centerPos);
    }
    
    private void ScaleOnDoubleTap(Touch touch) {
        if (parentObject.transform.localScale == Vector3.one) {
            Vector3 maxScale = new Vector3(MaxScale, MaxScale, 1);
            Vector2 position = Camera.main.ScreenToWorldPoint(touch.position);
            ScaleAndMoveToPosition(maxScale, position);
        } else {
            ScaleAndMoveToOriginal();
        }
    }
    
    private void ScaleAndMoveToPosition(Vector3 scale, Vector3 parentCurrPos) {
        Vector3 prevParentPos = parentObject.transform.position;
        parentObject.transform.position = parentCurrPos;
        Vector3 delta = parentObject.transform.position - prevParentPos;
        Vector3 pos = new Vector3(delta.x / parentObject.transform.localScale.x * -1, delta.y / parentObject.transform.localScale.y * -1, transform.position.z);
        transform.position = new Vector3(transform.position.x + pos.x, transform.position.y + pos.y, pos.z);
        parentObject.transform.localScale = scale;
    }
    
    private void ScaleAndMoveToOriginal() {
        parentObject.transform.localScale = Vector3.one;
        parentObject.transform.position = parentOriginalPos;
        transform.position = originalPos;
    }
    
    private Vector3 MakeScaleForZoom(float deltaMagnitude) {
        float scaleFactor = deltaMagnitude * ScaleFactor * AdjustOnZooming;
        Vector3 parentLocalScale = parentObject.transform.localScale;
        Vector3 scale = new Vector3(parentLocalScale.x + scaleFactor, parentLocalScale.y + scaleFactor, 1);
        scale.x = (scale.x > MaxScale) ? MaxScale : scale.x;
        scale.y = (scale.y > MaxScale) ? MaxScale : scale.y;
        scale.x = (scale.x < MinScale) ? MinScale : scale.x;
        scale.y = (scale.y < MinScale) ? MinScale : scale.y;
        return scale;
    }
    
    private void MoveToAdjustedPositionAccordingToScreen(Vector3 pos) {
        if (pos.x > screenSize.x * (parentObject.transform.localScale.x - 1))
            pos.x = screenSize.x * (parentObject.transform.localScale.x - 1);
        if (pos.x < screenSize.x * (parentObject.transform.localScale.x - 1) * -1)
            pos.x = screenSize.x * (parentObject.transform.localScale.x - 1) * -1;
        if (pos.y > screenSize.y * (parentObject.transform.localScale.y - 1))
            pos.y = screenSize.y * (parentObject.transform.localScale.y - 1);
        if (pos.y < screenSize.y * (parentObject.transform.localScale.y - 1) * -1)
            pos.y = screenSize.y * (parentObject.transform.localScale.y - 1) * -1;
            
        transform.position = pos;
    }
}
