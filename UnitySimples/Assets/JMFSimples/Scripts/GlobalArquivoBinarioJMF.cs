using UnityEngine;
using System.IO; // Para salvar e abrir arquivo local
//using System.Runtime.Serialization.Formatters.Binary; // Para converter de e para bin�rio n�o precisa mais
// *****           *****//
// M�todos "globais" para salvar e recuperar dados em arquivo bin�rio
// Salva e recupera dados de um objeto ArquivoBinarioJMF para arquivo bin�rio
// Se quiser proteger melhor os dados pode usar um checksum, criptografar, ou
// salvar em outra pasta (inclusive fazendo uma duplicata do arquivo para compara��o);
// OBS: Inicialmente implementado com BinaryFormater, mas por quest�es de seguran�a:
// https://docs.microsoft.com/en-us/dotnet/standard/serialization/binaryformatter-security-guide
// Agora usa JsonUtility e BinaryReader/Writer
// Obs2: poderia deixar a parte do Json em quem chama o Save/Load. Fica para a pr�xima
// *****           *****//
public class GlobalArquivoBinarioJMF
{
    private static string CaminhoArquivo = Application.persistentDataPath + "/"; //Caminho para o arquivo, pode alterar
    public static void ArquivoSalvar(ArquivoBinarioJMF serializableData, string fileName)//Salva dados em serializableData
    {
        /* C�digo antigo
        BinaryFormatter Formatador = new BinaryFormatter();//Conversor para bin�rio
        FileStream stream = new FileStream(CaminhoArquivo + fileName, FileMode.Create);//Acesso ao arquivo
        Formatador.Serialize(stream, serializableData);//Converte e salva no arquivo
        stream.Close(); // Fecha arquivo //*/

        string json = JsonUtility.ToJson(serializableData);
        using (BinaryWriter writer = new BinaryWriter(File.Open(CaminhoArquivo + fileName, FileMode.Create)))
        {
            writer.Write(json);
        }
    }
    public static ArquivoBinarioJMF ArquivoRecuperar(string fileName)//Carregar dados
    {
        if (File.Exists(CaminhoArquivo + fileName)) // Se o arquivo existe
        {
            /* C�digo antigo
            BinaryFormatter Formatador = new BinaryFormatter();//Conversor de e para bin�rio
            FileStream stream = new FileStream(CaminhoArquivo + fileName, FileMode.Open);//Acesso ao arquivo
            ArquivoBinarioJMF deserializedData = Formatador.Deserialize(stream) as ArquivoBinarioJMF;//Converte do arquivo          
            stream.Close();//Fecha arquivo */            

            string json;            
            using (BinaryReader reader = new BinaryReader(File.Open(CaminhoArquivo + fileName, FileMode.Open)))
            {
                json = reader.ReadString();
            }
            ArquivoBinarioJMF deserializedData = JsonUtility.FromJson<ArquivoBinarioJMF>(json);

            return deserializedData; //Retorna dados
        }
        else //arquivo n�o existe, retorna null
        {
            return null; //Quem chamou deve verificar se � null ou n�o antes de processar a informa��o.
        }
    }
}