using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; //Para usar o gerenciador de Scenes do Unity
//*****     *****//
// Carrega a pr�xima scene em background
// Espera comando para mudar de cena
// Se tiver um FadeManager, chama o FadeManager para executar a mudan�a de cena
// Sem FadeManager, muda a cena diretamente.
//*****     *****//
public class ManagerLoadSceneJMF : MonoBehaviour
{
    [SerializeField] private ManagerFadeJMF GerenciadorDeFade = default; //Gerenciador de Fade para transi��o suave
    private bool carregandoScene = false; //Indica se j� est� carregando uma Scene
    private bool mudarScene = false; //Indica que j� pode mudar de Scene
    public float PercentualCarregado { get; private set; } = 0f;//Percentual j� carregado da Scene para uso externo
    public bool CenaCarregada { get; private set; } = false; //Scene pronta para mudar para uso externo
    void Start()
    {
        carregandoScene = false; // N�o est� carregando a Scene ainda
        PercentualCarregado = 0f; // 0% Carregado
        CenaCarregada = false; // N�o foi carregada
        mudarScene = false; // N�o � para mudar ainda
    }
    public void AcaoCarregarProximaScene() //Dispara o carregamento da pr�xima Scene
    {
        if (carregandoScene != true) //Se j� n�o estiver carregando
        {
            carregandoScene = true; //Indica que j� est� carregando
            PercentualCarregado = 0f; //S� pra garantir
            CenaCarregada = false; //S� pra garantir
            mudarScene = false; //S� por garantia
            StartCoroutine(CorotinaCarregaProximaScene()); //Dispara carregamendo da pr�xima Scene
        }
    }
    private IEnumerator CorotinaCarregaProximaScene()//Carrega cena principal em paralelo
    {
        if (SceneManager.GetActiveScene().buildIndex + 1 < SceneManager.sceneCountInBuildSettings)
        {//Verifica se tem mais Scenes pois vai carregar a pr�xima
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);//Manda carregar cena
            asyncOperation.allowSceneActivation = false;//N�o deixa abrir automaticamente
            yield return null; //Deixa para pr�xima atualiza��o
            while (!asyncOperation.isDone)//Enquanto est� carregando
            {
                PercentualCarregado = asyncOperation.progress;
                if (asyncOperation.progress >= 0.9f)//Cena pronta
                {
                    CenaCarregada = true;
                    if (mudarScene)//Se a vari�vel foi alterada para true
                    {
                        asyncOperation.allowSceneActivation = true; //Deixa a cena carregar
                        if (GerenciadorDeFade != null && !GerenciadorDeFade.Equals(null))//Se o componente existe
                        { 
                            StartCoroutine(GerenciadorDeFade.CorotinaFadeOut()); //Manda fazer FadeOut
                        }
                    }
                }
                yield return null;//Continua no while na pr�xima itera��o
            }
        }
    }
    public void AcaoMudarDeScene()//Dispa a mudan�a para a pr�xima scene
    {
        if (GerenciadorDeFade != null && !GerenciadorDeFade.Equals(null))//Se o componente existe
        {   //Manda fazer FadeIn e ao finalizar chamar AcaoMudarSceneDoFade
            StartCoroutine(GerenciadorDeFade.CorotinaFadeIn(AcaoMudarDeSceneDoFade));
        }
        else //se n�o tem FadeManager
        {
            mudarScene = true; // Altera diretamente o indicador de mudan�a de Scene
        }
    }
    public void AcaoMudarDeSceneDoFade() //Retorno do FadeIn
    {
        mudarScene = true; //Altera o indicador de mudan�a de Scene
    }
}