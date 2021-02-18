using System.Collections;
using UnityEngine;
//*****     *****//
// Gerencia o Fade In e o Fade Out de uma tela para transi��o de Scenes (tamb�m pode ser usado para transi��o entre telas)
// Poderia usar Animation/Aminator, mas preferi fazer em script (sou programador velha guarda e prefiro codar :-D)
//*****     *****//
public class ManagerFadeJMF : MonoBehaviour
{
    [SerializeField] private Canvas CanvasFade = default; //Tela de Fade para poder ativar e desativar
    [SerializeField] private CanvasGroup CanvasGroupFade = default; //Canvas Group para mudar o alpha
    [SerializeField] private float TempoFadeIn = 0.6f; //Tempo de Aparecimento padr�o ajust�vel no Inspector
    [SerializeField] private float TempoFadeOut = 0.9f; //Tempo de Desaparecimento padr�o ajust�vel no Inspector
    private float defaultFadeIn = 1.7f; //Velocidade de aparecimento padr�o
    private float defaultFadeOut = 1.1f; //Velocidade de desaparecimento padr�o
    private float velFadeIn = 1.7f; //Velocidade de aparecimento
    private float velFadeOut = 1.1f; //Velocidad de desaparecimento
    private float ratio = 1f; // Percentual do fade
    /* //Para testar diferentes tempos de FadeIn e FadeOut, sem ter que executar novamente,
    // comente a linha anterior, colocando mais uma / antes do /*
    // Se estiver satisfeito com as velocidade de fade pode deixar como est�
    private void Update()
    {
        velFadeIn = TempoFadeIn > 0f ? 1f / TempoFadeIn : 1f; //Converte tempo em velocidade
        velFadeOut = TempoFadeOut > 0f ? 1f / TempoFadeOut : 1f; //Converte tempo em velocidade
    } //*/
    void Start()
    {
        defaultFadeIn = TempoFadeIn > 0f ? 1f / TempoFadeIn : 1.7f; //Converte tempo em velocidade
        defaultFadeOut = TempoFadeOut > 0f ? 1f / TempoFadeOut : 1.1f; //Converte tempo em velocidade
    }
    private void SetTransparency()
    { //Eu prefiro um fade quadr�tico, mas fique � vontade para alterar
        CanvasGroupFade.alpha = (2f - ratio) * ratio;//Ajusta a transpar�ncia com base no ratio
    }
    public void ImediatoFullIn()
    {
        CanvasGroupFade.alpha = 1f; // Deixa transparente
        CanvasFade.gameObject.SetActive(true); //Mostra o canvas
        ratio = 1f;
    }
    public void ImediatoFullOut()
    {
        ratio = 0f;//Totalmente transparente
        CanvasGroupFade.alpha = 0f; // Deixa transparente
        CanvasFade.gameObject.SetActive(false); //Esconde o canvas
    }
    public IEnumerator CorotinaFadeIn(System.Action onFadedIn, float newVel = 0) //Realiza o FadeIn e retorna para fun��o
    {
        velFadeIn = newVel > 0f ? newVel : defaultFadeIn; //Se tiver passado uma velocidade v�lida, usa ela
        CanvasGroupFade.alpha = 0f; // Deixa transparente
        yield return null; //Pr�xima itera��o
        CanvasFade.gameObject.SetActive(true); //Mostra o canvas
        yield return null; //Pr�xima itera��o
        ratio = 0f; //come�a totalmente transparente
        while (ratio < 1f) //Enquanto n�o chegou em 100% de opacidade
        {
            SetTransparency(); //Aplica transpar�ncia
            ratio += velFadeIn * Time.deltaTime; //Percentual aumenta
            yield return null; //Pr�xima itera��o
        }
        ratio = 1f; //ratio chegou a 100%, mas pode ter passado
        CanvasGroupFade.alpha = 1f; // Deixa transparente
        onFadedIn();//Chama fun��o passada como par�metro
    }
    public IEnumerator CorotinaFadeOut(float newVel = 0f) //Realiza o FadeOut
    {
        velFadeOut = newVel > 0f ? newVel : defaultFadeOut; //Se tiver passado uma velocidade v�lida, usa ela
        CanvasGroupFade.alpha = 1f; // Deixa transparente
        yield return null; //Pr�xima itera��o
        CanvasFade.gameObject.SetActive(true); //Mostra o canvas
        yield return null; //Pr�xima itera��o
        ratio = 1f; //Come�a totalmente opaco
        while (ratio > 0f) //enquanto n�o chegou em 0%
        {
            SetTransparency();
            ratio -= velFadeOut * Time.deltaTime; //Percentual aumenta
            yield return null;
        }
        ratio = 0f;
        CanvasGroupFade.alpha = 0f; // Deixa transparente
        CanvasFade.gameObject.SetActive(false); //Desabilita o canvas
    }
}