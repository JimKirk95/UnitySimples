using UnityEngine;
//*****     *****//
// Reajusta a altura dos itens de um elemento de UI Dropdown, com base na altura do Label do Dropdown
// Adicione este script ao Label do Dropdown
// Abra o Template -> Viewport -> Content do Dropdown e arraste o Item para a propriedade Item deste Script no Inspector
//*****     *****//
//[ExecuteInEditMode] //Descomente para que os ajustes sejam feitos em modo de edi��o tamb�m.
public class CircunventDropdownJMF : MonoBehaviour
{
    [Tooltip("Template>Viweport>Content>Item")]
    [SerializeField] private RectTransform Item = default; //RectTransform do Item que ser� ajustado
    private RectTransform rectTransform = null; //Refer�ncia para a RectTransform do Label
    private Vector2 newSizeDelta; // Para ajustar a RectTransform do Item
    private void OnValidate()
    {
        rectTransform = gameObject.GetComponent<RectTransform>(); //Copia refer�ncia para o RectTransform do Label
    }
    void Awake() //Inicializa��o da Scene
    {
        rectTransform = gameObject.GetComponent<RectTransform>();  //Copia refer�ncia para o RectTransform do Label
    }
    private void OnRectTransformDimensionsChange() //Quando o RectTransform do Label for alterado
    {
        if ((rectTransform != null) && (Item != null)) //Se j� tem o Rectransform e o Item
        {
            newSizeDelta.x = Item.sizeDelta.x; //Copia o comprimento do Item
            newSizeDelta.y = rectTransform.rect.height; //Usa a altura do Label
            Item.sizeDelta = newSizeDelta; //Atribui o novo tamanho para o Item
        }
    }
    private void Start()
    {
        if ((rectTransform != null) && (Item != null)) //Se j� tem o Rectransform e o Item
        {
            newSizeDelta.x = Item.sizeDelta.x; //Copia o comprimento do Item
            newSizeDelta.y = rectTransform.rect.height; //Usa a altura do Label
            Item.sizeDelta = newSizeDelta; //Atribui o novo tamanho para o Item
        }
    }
}
