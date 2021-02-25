using UnityEngine;
using UnityEngine.UI;
//*****     *****
// Scene Principal, mas n�o faz muita coisa (nem tem Update())
// Implementa o Save/Load, mostra informa��es do Google Sheets 
// e chama uma tela de configura��o de prefer�ncias
//*****     *****
public class ScenePrincipalJMF : MonoBehaviour
{
    const string FileName = "appdatajson.jmf"; //Podia ser mais criativo ou gen�rico?
    [Header("Dados Save/Load")]
    [SerializeField] private Toggle SavedataBool = default;
    [SerializeField] private InputField SavedataInt = default;
    [SerializeField] private InputField SavedataFloat = default;
    [SerializeField] private InputField SavedataString = default;
    [Header("Dados Google Sheet")]
    [SerializeField] private Text GSheetB2 = default;
    [SerializeField] private Text GSheetC2 = default;
    [SerializeField] private Text GSheetD2 = default;
    [Header("Dados Google Sheet")]
    [SerializeField] private Canvas CanvasPreferencias = default;
    void Start()
    {
        CanvasPreferencias.gameObject.SetActive(false);
        if (GlobalDownloadGSheetJMF.TemDados) //Se tver dados da planilha, atualiza os campos
        {
            UpdateCSVData();
        }
    }
    public void UpdateCSVData() //Estou pegando as celuas (B2, C2 e D2 da minha planilha)
    { //Modifique � vontade. E pode usar uma planilha sua no GlobalDownloadGSheetJMF
        if (GlobalDownloadGSheetJMF.TemDados)
        {
            GSheetB2.text = GlobalDownloadGSheetJMF.PlanilhaGSheet[1, 1];
            GSheetC2.text = GlobalDownloadGSheetJMF.PlanilhaGSheet[1, 2];
            GSheetD2.text = GlobalDownloadGSheetJMF.PlanilhaGSheet[1, 3];
        }
    }
    public void AcaoSaveData() //Salva os dados em arquivo
    {
        int tempInt; //Vari�vel tempor�ria
        float tempFloat; //Vari�vel tempor�ria
        ArquivoBinarioJMF ParaSalvar = new ArquivoBinarioJMF();//cria vari�vel serializ�vel
        ParaSalvar.ExemploBool = SavedataBool.isOn; //Copia bool
        ParaSalvar.ExemploInt = int.TryParse(SavedataInt.text, out tempInt) ? tempInt : 0; //Copia int
        ParaSalvar.ExemploFloat = float.TryParse(SavedataFloat.text, out tempFloat) ? tempFloat : 0f; //Copia float
        ParaSalvar.ExemploString = SavedataString.text; //Copia String
        GlobalArquivoBinarioJMF.ArquivoSalvar(ParaSalvar, FileName);//Chama manager para salvar
    }
    public void AcaoLoadData() //Recupera os dados do arquivo
    {
        ArquivoBinarioJMF loadedData = GlobalArquivoBinarioJMF.ArquivoRecuperar(FileName);
        if (loadedData != null && !loadedData.Equals(null)) //Verifica se o componente existe
        {
            SavedataBool.isOn = loadedData.ExemploBool;
            SavedataInt.text = loadedData.ExemploInt.ToString();
            SavedataFloat.text = loadedData.ExemploFloat.ToString();
            SavedataString.text = loadedData.ExemploString;
        }
    }
}