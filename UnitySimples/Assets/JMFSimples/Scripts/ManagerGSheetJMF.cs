using System.Collections;
using UnityEngine;
//*****     *****//
// Gerencia o processamento do csv do Google Sheets
//*****     *****//
public class ManagerGSheetJMF : MonoBehaviour
{
    private const string Verifica = "JMFUTGS"; //Conte�do da primeira c�lula da planilha
    private const string BackupFilename = "csvlocal.jmf"; //Nome to arquivo de backup local
    public float Progresso { get; private set; } = 0f;//Percentual j� carregado para uso externo
    private bool LoadingCSVGSheet = false;// Flag que indica se est� carregando 
    public bool Finalizado { get; private set; } = false;//Percentual j� carregado para uso externo
    public void CarregaCSV(bool download = true) // Fun��o p�blica para disparar carregamento e processamento
    {//Passar false para processamento apenas local
        if (!LoadingCSVGSheet) //Se j� n�o estiver carregando
        {
            Finalizado = false;
            LoadingCSVGSheet = true;//Est� carregando
            Progresso = 0f; //Inicia progresso
            if (download) //Atualiza a planilha
            {
                GlobalDownloadGSheetJMF.Status = "Fazendo download da planilha..."; //Altera status
                StartCoroutine(GlobalDownloadGSheetJMF.BaixarCSV(VerificaDownload)); //Manda carregar e Verificar
            }
            else //Usa planilha local
            {
                GlobalDownloadGSheetJMF.Status = "N�o atualiza! "; //Altera status
                CarregaCSVLocal(); //Chama m�todo de carregamento de arquivo local
            }
        }
    }
    public void VerificaDownload(string data)//Chamado depois do download em Action callback
    {
        if (string.IsNullOrEmpty(data)) // string null, problema no download
        {
            GlobalDownloadGSheetJMF.Status = "Problemas de conex�o! "; //Atualiza Status
            CarregaCSVLocal(); //Chama m�todo de carregamento de arquivo local
        }
        else //Tem donwload, mas talvez n�o seja a planilha correta
        {
            if (CSVValido(data))//Confere, a string corresponde � planilha
            {
                GlobalDownloadGSheetJMF.DataDownload = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm");//Pega data do sistema
                ArquivoBinarioJMF CSVParaSalver = new ArquivoBinarioJMF(GlobalDownloadGSheetJMF.DataDownload, data);//cria vari�vel serializ�vel
                GlobalArquivoBinarioJMF.ArquivoSalvar(CSVParaSalver, BackupFilename); //Salva nova vers�o do csv
                GlobalPlayerPrefsJMF.AtualizaExecucoes(0); //Indica que conseguiu atualizar nessa execu��o
                GlobalDownloadGSheetJMF.Status = "Planilha nova "; //Atualiza status
                StartCoroutine(ProcessData(ProcessamentoFinalizado, data));//Chama processamento do .csv
            }
            else //download feito, mas o csv n�o corresponde � planilha esperada
            {
                GlobalDownloadGSheetJMF.Status = "Download corrompido! "; //Atualiza status
                CarregaCSVLocal(); //Chama m�todo de carregamento de arquivo local
            }
        }
    }
    private bool CSVValido(string csv) //verifica se a string corresponde � planilha desejada
    {
        string checkData = csv.Substring(0, 7); //Pega os 7 primeiros caracteres
        return checkData.Equals(Verifica); //Se for igual, retorna true
    }
    private void CarregaCSVLocal()
    {
        GlobalDownloadGSheetJMF.Status += "C�pia local ";
        ArquivoBinarioJMF SavedDada = GlobalArquivoBinarioJMF.ArquivoRecuperar(BackupFilename); //Tenta carregar arquivo
        if (SavedDada == null)//N�o tem arquivo local
        {
            LoadingCSVGSheet = false;//Finalizou carregamento (sem carregar)
            GlobalDownloadGSheetJMF.Status += "n�o encontrada!";
            GlobalDownloadGSheetJMF.DataDownload = "Nenhuma atualiza��o detectada!";
            GlobalDownloadGSheetJMF.PlanilhaGSheet = null;
            GlobalDownloadGSheetJMF.TemDados = false;
            Finalizado = true;
        }
        else//Dados do arquivo local
        {
            GlobalDownloadGSheetJMF.DataDownload = SavedDada.Info; //Pega data do download
            StartCoroutine(ProcessData(ProcessamentoFinalizado, SavedDada.ExemploString));//Chama processamento do .cs
        }
    }
    public IEnumerator ProcessData(System.Action<string[,]> onCompleted, string data) //Separa .csv em matrix
    {
        string[] DataLinhas = data.Split("\n"[0]);//separa em linhas ('\r') ou (["\r\n"]) 
        string[] LinhaColunas = DataLinhas[0].Split(',');//Separa o cabe�alho em colunas para saber o n�mero de colunas
        int NumeroColunas = LinhaColunas.Length;//Vari�vel auxiliar, guarda o n�mero de colunas
        string[,] Planilha = new string[DataLinhas.Length, NumeroColunas];//Cria planilha
        string[] Colunas; //Vari�vel auxiliar para separa��o em Colunas
        int nColuna; //indicador da coluna atual - poderia ser criada localmente
        int subIndice; //indicador de conjunto de colunas que est� sendo processada podria ser criada localmente
        int offsetInicio; //Vari�vel auxiliar para eliminar strings vazias
        int offsetFim; //Vari�vel auxiliar para eliminar strings vaziar
        Progresso = 0.1f;//J� carregou arquivo, progresso de 10%
        for (int nLinha = 0; nLinha < DataLinhas.Length; nLinha++)//Processa cada uma das linhas
        {
            LinhaColunas = DataLinhas[nLinha].Split('"');//separa usando aspas
            if (LinhaColunas.Length == 1)//n�o tem aspas, ent�o n�o tem ',' .
            {
                Colunas = DataLinhas[nLinha].Split(',');//separa colunas usando ','.
                for (nColuna = 0; nColuna < NumeroColunas; nColuna++)//Para cada c�lula separado
                {
                    Planilha[nLinha, nColuna] = Colunas[nColuna];//Copia dados da c�lula para Planilha
                }
            }
            else//tem aspas, ent�o tem c�lula com ','
            {
                nColuna = 0; //Inicia coluna em zero
                subIndice = 0; //Inicia sub�ndice em zero
                while (nColuna < NumeroColunas)//Enquanto n�o processou todas as colunas
                {
                    if (subIndice % 2 == 1)//c�lula que cont�m ',' e gerou as aspas. Copia inteira
                    {
                        Planilha[nLinha, nColuna] = LinhaColunas[subIndice];//Copia c�lula que tem ','
                        nColuna++;//incrementa indicador da coluna
                    }
                    else //conjunto de celulas (sem ','), antes e/ou depois de uma c�lula com ','. Precisa separar
                    {
                        if (LinhaColunas[subIndice].Length > 1)//N�o � s� uma v�rgula, se for ignora
                        {
                            Colunas = LinhaColunas[subIndice].Split(',');//separa celulas individuais                                    
                            offsetInicio = (subIndice == 0) ? 0 : 1;//Se n�o for o primeiro, pula um
                            offsetFim = (subIndice == LinhaColunas.Length - 1) ? 0 : 1; //se n�o o �ltimo pula um
                            for (int i = offsetInicio; i < Colunas.Length - offsetFim; i++)//para cada c�lula
                            {
                                Planilha[nLinha, nColuna] = Colunas[i]; //copia c�lula
                                nColuna++; //pr�xima c�lula
                            }
                        }
                    }
                    subIndice++;//pr�ximo subconjunto de c�lulas
                    Progresso = (1f + nLinha + (1f*nColuna / NumeroColunas)) / DataLinhas.Length;
                    yield return null;
                }
            }
            Progresso = (2f + nLinha) / DataLinhas.Length;
            yield return null;
        } // separa string .csv em matriz de strings (c�lulas)
        onCompleted(Planilha);//Retorna string com conte�do do .csv ou null
    } // Matriz repassada para a fun��o em callback
    public void ProcessamentoFinalizado(string[,] Planilha) //Finaliza processamento do .csv
    {
        Progresso = 1.0f;//Progresso de 100%
        LoadingCSVGSheet = false;//Finalizou carregamento
        GlobalDownloadGSheetJMF.Status += "processada!";
        GlobalDownloadGSheetJMF.PlanilhaGSheet = Planilha;
        GlobalDownloadGSheetJMF.TemDados = true;
        Finalizado = true;
    }
}