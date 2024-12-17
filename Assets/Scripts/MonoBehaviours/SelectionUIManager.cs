using Unity.VisualScripting;
using UnityEngine;

public class SelectionUIManager : MonoBehaviour
{
    [SerializeField] private Canvas m_canvas;
    [SerializeField] private RectTransform m_selectionAreaTransform;

    private void Start()
    {
        UnitSelectionManager.Instance.OnSelectionAreaStart += HandleSelectionBegin;
        UnitSelectionManager.Instance.OnSelectionAreaEnd += HandleSelectionEnd;

        m_selectionAreaTransform.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (m_selectionAreaTransform.gameObject.activeSelf)
        {
            UpdateSelectionArea();
        }
    }

    private void HandleSelectionBegin(object sender, Vector2 startLocation)
    {
        m_selectionAreaTransform.gameObject.SetActive(true);
        UpdateSelectionArea();
    }

    private void HandleSelectionEnd(object sender, Vector2 endLocation)
    {
        m_selectionAreaTransform.gameObject.SetActive(false);
    }

    private void UpdateSelectionArea()
    {
        Rect selectionArea = UnitSelectionManager.Instance.GetSelectionArea();

        float canvasScale = m_canvas.transform.localScale.x;

        m_selectionAreaTransform.anchoredPosition = new Vector2(selectionArea.x, selectionArea.y) / canvasScale;
        m_selectionAreaTransform.sizeDelta = new Vector2(selectionArea.width, selectionArea.height) / canvasScale;
    }
}
