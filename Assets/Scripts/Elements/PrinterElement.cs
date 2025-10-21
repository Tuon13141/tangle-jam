using UnityEngine;

public class PrinterElement : MonoBehaviour
{
    [SerializeReference] SpriteRenderer m_SpriteRenderer;

    public void ChangeColorPinter(Color color)
    {
        m_SpriteRenderer.color = color;
    }
}
