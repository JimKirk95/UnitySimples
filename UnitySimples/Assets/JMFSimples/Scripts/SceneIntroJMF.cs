using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
//*****     *****//
// Scene inicial (Aviso, carregamento)
// - Carrega dados usando GlobalPlayerPrefsJMF e aciona FadeOut
// - Aciona carregamento da Scene principal e do GSheet em segundo plano
// Guando estiver tudo carregado, habilita bot�o de Continuar
//*****     *****//
public class SceneIntroJMF : MonoBehaviour
{
    [Header("Gerenciadores:")]
    [SerializeField] private ManagerLoadSceneJMF LoadSceneManager = default; //Gerenciador de Scenes
    [SerializeField] private ManagerFadeJMF FadeManager = default; // Gerenciador de Fade
    [SerializeField] private ManagerGSheetJMF GSheetManager = default;  //Carregador de GSheet
    [Header("Carregamento UI:")]
    [SerializeField] private float TempoEspera = 1.5f; // Tempo m�nimo de carregamento (para mostrar anima��o)
    [SerializeField] private Text TextCarregando = default; //Texto carregando...
    [SerializeField] private Image RotatingImage = default; //Imagem de loading rotativa
    [SerializeField] private float RotatingTime = 0.08f; // Tempo para "girar" imagem de carregamento
    [SerializeField] private float RotatingAngle = 45f; // �ngulo para "girar" imagem de carregamento
    [SerializeField] private Button BotaoContinuar = default; // bot�o habilitado quando a cena fica pronta
    [Header("Textos para PlayerPrefs")]
    [SerializeField] private Text PlayerPrefVolume = default; //Texto para PlayerPref
    [SerializeField] private Text PlayerPrefAtualiza = default; //Texto para PlayerPref
    [SerializeField] private Text PlayerPrefExecucoes = default; //Texto para PlayerPref
    [Header("Textos para CVS load")]
    [SerializeField] private Text CSVLoadStatus = default; //Texto para loadCSV
    [SerializeField] private Text CSVDateLoad = default; //Texto para loadCCSV 
    private bool TudoCarregado = false; //Indica quando pode mudar de scene
    void Start()
    {
        TudoCarregado = false; //Flag de tudo pronto para acionar bot�o e parar anima��o
        BotaoContinuar.onClick.AddListener(AcaoLoadProximaScene); //Atribui m�todo para onClick do bot�o
        StartCoroutine(FadeCanvas()); //Inicia o Fade
        StartCoroutine(LoadingAnimation()); // Inicia anima��o de carregamento
        GlobalPlayerPrefsJMF.RecuperaPlayerPrefs(); //Manda carregar PlayerPrefs
        IniciaAudio(); //Inicia Audio usando PlayerPrefs
        GlobalPlayerPrefsJMF.AtualizaExecucoes(GlobalPlayerPrefsJMF.Execucoes + 1);//Atualiza n�mero de execu��es
        TxtPlayerPrefs(); //Atualiza PlayerPrefs
        LoadSceneManager.AcaoCarregarProximaScene(); //Manda carregar cena
        StartCoroutine(LoadingCSV()); //Inicia carregamento do Google Sheets/CSV
        StartCoroutine(VerificaStatus()); //Inicia corotina de verifica��o
    } 
    public void AcaoLoadProximaScene() //Resposta para o bot�o de prosseguir 
    {
        LoadSceneManager.AcaoMudarDeScene();
    }
    public IEnumerator FadeCanvas() //Inicia o FadeCanvas.
    {
        FadeManager.ImediatoFullIn(); //Come�a com tela preta 
        yield return null; //Pr�ximo frame
        StartCoroutine(FadeManager.CorotinaFadeOut());// Manda fazer fadeout
    }
    public IEnumerator LoadingAnimation() //Gerencia a anima��o de carregando...
    {
        BotaoContinuar.gameObject.SetActive(false); //Desabilita bot�o de Continuar
        TextCarregando.gameObject.SetActive(true); //Mostra o texto de loading
        RotatingImage.gameObject.SetActive(true); //Mostra a figura de loading
        float SpinAccumulatedTime = 0f; //Inicia cron�metro do loading
        yield return null; // volta no pr�ximo updade
        while (!TudoCarregado) //Enquanto n�o est� tudo pronto
        {
            if (SpinAccumulatedTime >= RotatingTime) //Se o cron�metro passou do valor
            {
                RotatingImage.rectTransform.Rotate(0f, 0f, -RotatingAngle, Space.Self);//Gira de RotatingAngle
                SpinAccumulatedTime -= RotatingTime; //Subtrai Rotating time do cron�metro para pr�ximo giro
            }
            SpinAccumulatedTime += Time.deltaTime;//Acumula cron�metro
            yield return null; // Pr�ximo update
        } //TudoCarregado
        BotaoContinuar.gameObject.SetActive(true); //Reabilita bot�o de Continuar
        TextCarregando.gameObject.SetActive(false); //Esconde o texto de loading
        RotatingImage.gameObject.SetActive(false); //Esconde a figura de loading
    }
    private void IniciaAudio() //Inicia o AudioSource
    {
        AudioSource meuAudio = ManagerSomSingletonJMF.staticSingleton.GetAudioSource(); // Recupera AudioSource
        if (meuAudio.isPlaying) //Se estiver tocando
            meuAudio.Stop(); //Para
        meuAudio.loop = true; // Ativa Looping de �udio
        meuAudio.volume = GlobalPlayerPrefsJMF.Volume; //Ajusta volume
        meuAudio.mute = GlobalPlayerPrefsJMF.Mudo; //Ajusta mudo ou n�o
        meuAudio.clip = ManagerSomSingletonJMF.staticSingleton.GetMusic(GlobalPlayerPrefsJMF.IndiceMusica); //Pega m�sica
        meuAudio.Play(); //toca musica
    }
    private void TxtPlayerPrefs() //Atualiza txts com informa��es do PlayerPrefs
    {
        PlayerPrefVolume.text = "Volume: <b>" + ((int)(100 * GlobalPlayerPrefsJMF.Volume)).ToString() + "%</b>";
        if (GlobalPlayerPrefsJMF.Mudo)
        {
            PlayerPrefVolume.text += " - MUDO";
        }
        if (GlobalPlayerPrefsJMF.Nunca)
        {
            PlayerPrefAtualiza.text = "Nunca atualiza";
        }
        else
        {
            if (GlobalPlayerPrefsJMF.Intervalo == 1)
            {
                PlayerPrefAtualiza.text = "Atualiza em <b>todas</b> as execu��es";
            }
            else
            {
                PlayerPrefAtualiza.text = "Atualiza a cada: <b> " + GlobalPlayerPrefsJMF.Intervalo.ToString() + " execu��es </b>";
            }
        }
        PlayerPrefExecucoes.text = "Execu��es desde a �ltima atualiza��o: <b>" + GlobalPlayerPrefsJMF.Execucoes + "</b>";
    }
    public IEnumerator LoadingCSV() //Gerencia o carregamento da planilha
    {
        yield return null; // volta no pr�ximo updade
        if ((!GlobalPlayerPrefsJMF.Nunca) && (GlobalPlayerPrefsJMF.Execucoes >= GlobalPlayerPrefsJMF.Intervalo)) // Vai tentar atualizar o .csv
        {
            GSheetManager.CarregaCSV(); //Manda carregar Google Sheets
        }
        else
        { //N�o � para atualizar via download. Usados dados salvos                 
            GSheetManager.CarregaCSV(false); //Manda carregar .csv de arquivo local
        }
        yield return null; // volta no pr�ximo updade        
        while (!GSheetManager.Finalizado)
        {
            CSVLoadStatus.text = "Status CSV: " + GlobalDownloadGSheetJMF.Status;
            yield return null; //Pr�ximo update
        }
        CSVLoadStatus.text = "Status csv: " + GlobalDownloadGSheetJMF.Status;
        CSVDateLoad.text = "�ltima atualiza��o: " + GlobalDownloadGSheetJMF.DataDownload;
        PlayerPrefExecucoes.text = "Execu��es desde a �ltima atualiza��o: <b>" + GlobalPlayerPrefsJMF.Execucoes + "</b>";
    }
    public IEnumerator VerificaStatus() //Ap�s tempo de espera verifica se est� tudo carregado.
    {
        yield return new WaitForSeconds(TempoEspera); //Espera tempo de espera
        while ((!GSheetManager.Finalizado) || (!LoadSceneManager.CenaCarregada)) //Enquanto tem alguma coisa carregando
        {
            yield return null; // volta no pr�ximo updade
        }
        TudoCarregado = true; //Tudo pronto
    }
}