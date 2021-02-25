using UnityEngine;
using System.IO; // Para salvar e abrir arquivo local
using System.Runtime.Serialization.Formatters.Binary; // Para converter de e para bin�rio
// *****           *****//
// M�todos "globais" para salvar e recupera dados em arquivo bin�rio
// Salva e recupera dados de um objeto ArquivoBinarioJMF para arquivo bin�rio
// Se quiser proteger melhor os dados pode usar um checksum, criptografar, ou
// salvar em outra pasta (inclusive fazendo uma duplicata do arquivo para compara��o);
// OBS: Inicialmente implementado com BinaryFormater e alterado para BinaryReader/Writer por
// quest�es de seguran�a: https://docs.microsoft.com/en-us/dotnet/standard/serialization/binaryformatter-security-guide
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
        using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
        {
            writer.Write(serializableData.Info);
            writer.Write(serializableData.ExemploBool);
            writer.Write(serializableData.ExemploInt);
            writer.Write(serializableData.ExemploFloat);
            writer.Write(serializableData.ExemploString);
            //writer.Write(@"c:\Temp");
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
            
            ArquivoBinarioJMF deserializedData = new ArquivoBinarioJMF();


            using (BinaryReader reader = new BinaryReader(File.Open(fileName, FileMode.Open)))
            {
                deserializedData.Info = reader.ReadString();
                deserializedData.ExemploBool = reader.ReadBoolean();
                deserializedData.ExemploInt = reader.ReadInt32();
                deserializedData.ExemploFloat = reader.ReadSingle();
                deserializedData.ExemploString = reader.ReadString();
            }



            return deserializedData; //Retorna dados
        }
        else //arquivo n�o existe, retorna null
        {
            return null; //Quem chamou deve verificar se � null ou n�o antes de processar a informa��o.
        }
    }
}
