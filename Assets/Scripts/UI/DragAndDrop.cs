using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler {
    public Canvas canvas;
    public AgentSpawner agentSpawner;
    public WorldController wc; 
    public Species species;
    private RectTransform rectTransform;
    private Vector2 originPos;

    public Boolean isSpecies = true;
    public Boolean isTree = false; 

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
        originPos = rectTransform.anchoredPosition;
    }

    public void OnPointerDown(PointerEventData eventData) {}

    public void OnBeginDrag(PointerEventData eventData) {}

    public void OnEndDrag(PointerEventData eventData) {
        rectTransform.anchoredPosition = originPos;

        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        if (Physics.Raycast(ray, out var hit)) {
            if (isTree)
                agentSpawner.AddTree(hit);
            else if (isSpecies)
                agentSpawner.AddSpecies(species, hit);
            else 
                wc.NewWater(hit);
        }
    }

    public void OnDrag(PointerEventData eventData) {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
}