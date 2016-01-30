using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class HMCScrollRect : ScrollRect {

    private List<int> pointerIdList = new List<int>();
    
    public override void OnBeginDrag(PointerEventData eventData) {
        pointerIdList.Add(eventData.pointerId);
        base.OnBeginDrag(eventData);
    }
    
    public override void OnDrag(PointerEventData eventData) {
        // 複数のタッチそれぞれで同時にドラッグ処理が行われないようにする。
        if (pointerIdList[0] == eventData.pointerId) {
            base.OnDrag(eventData);
        }
    }
    
    public override void OnEndDrag(PointerEventData eventData) {
        pointerIdList.Remove(eventData.pointerId);
        base.OnEndDrag(eventData);
    }
}
