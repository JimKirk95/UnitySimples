using UnityEngine;
using UnityEngine.Networking; // Para carregar do Google Sheets
using System.Collections; // Para usar IEnumarator com Callback
// *****           *****//
// M�todo static que faz Download de uma planilha em .csv e retorna uma string em Action callback para processamento
// Tamb�m armazena a planilha depois de processada (essa parte poderia estar em outro lugar)
// *****           *****//
public class GlobalDownloadGSheetJMF
{
    // Para usar sua pr�pria planilha Google, COPIE o identificador do link de compartilhamento
    // EXEMPLO:
    //    Link de compartilhamento:
    //    https://docs.google.com/spreadsheets/d/1Dk26trV96k0SpyfmUSybzpSWf-vYrtK2opGDX1cdv8E/edit?usp=sharing
    //    Esse aqui � o id:                      1Dk26trV96k0SpyfmUSybzpSWf-vYrtK2opGDX1cdv8E
    //    Copie o id do compartilhamento de sua planilha para a pr�xima linha:
    private const string GSheetID = "1Dk26trV96k0SpyfmUSybzpSWf-vYrtK2opGDX1cdv8E";//ID da planilha, copiado do link
    private const string url = "https://docs.google.com/spreadsheets/d/" + GSheetID + "/export?format=csv";//url completa
    //Aqui uma pequena maracutaia, vou aproveitar o mesmo script para armazenar as informa��es da planilha:
    public static bool TemDados = false; //Indica se tem informa��es vinda da planilha (atualizado externamente)
    public static string Status = "";
    public static string DataDownload = "Nenhuma atualiza��o detectada!";
    public static string[,] PlanilhaGSheet; //C�pia da planilha do Google Sheets em forma de Matriz de Strings (atualizado externamente)
    internal static IEnumerator BaixarCSV(System.Action<string> onDownloaded) //Download da planilha como corotina
    { //Ao final chama o m�todo passado como par�metro com a string
        yield return new WaitForEndOfFrame();//Espera uma atualiza��o
        string arquivoCSV = null;//String de retorno
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))//Cria conex�o com a Internet
        {
            yield return webRequest.SendWebRequest();//Pede para baixar arquivo csv
            if ((webRequest.result == UnityWebRequest.Result.ConnectionError))//N�o conseguiu baixar
            {
                arquivoCSV = null;//vai retornar null
            }
            else //Conseguiu baixar
            {
                arquivoCSV = webRequest.downloadHandler.text; //Copia para vari�vel de retorno
            }
        }
        onDownloaded(arquivoCSV);//Chama a fun��o em callback passando a string (que pode ser null)
    }
}