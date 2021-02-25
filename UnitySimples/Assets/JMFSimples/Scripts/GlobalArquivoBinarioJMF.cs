using UnityEngine;
using System.IO; // Para salvar e abrir arquivo local
//using System.Runtime.Serialization.Formatters.Binary; // Para converter de e para binário não precisa mais
// *****           *****//
// Métodos "globais" para salvar e recuperar dados em arquivo binário
// Salva e recupera dados de um objeto ArquivoBinarioJMF para arquivo binário
// Se quiser proteger melhor os dados pode usar um checksum, criptografar, ou
// salvar em outra pasta (inclusive fazendo uma duplicata do arquivo para comparação);
// OBS: Inicialmente implementado com BinaryFormater, mas por questões de segurança:
// https://docs.microsoft.com/en-us/dotnet/standard/serialization/binaryformatter-security-guide
// Agora usa JsonUtility e BinaryReader/Writer
// Obs2: poderia deixar a parte do Json em quem chama o Save/Load. Fica para a próxima
// *****           *****//
public class GlobalArquivoBinarioJMF
{
    private static string CaminhoArquivo = Application.persistentDataPath + "/"; //Caminho para o arquivo, pode alterar
    public static void ArquivoSalvar(ArquivoBinarioJMF serializableData, string fileName)//Salva dados em serializableData
    {
        /* Código antigo
        BinaryFormatter Formatador = new BinaryFormatter();//Conversor para binário
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
            /* Código antigo
            BinaryFormatter Formatador = new BinaryFormatter();//Conversor de e para binário
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
        else //arquivo não existe, retorna null
        {
            return null; //Quem chamou deve verificar se é null ou não antes de processar a informação.
        }
    }
}