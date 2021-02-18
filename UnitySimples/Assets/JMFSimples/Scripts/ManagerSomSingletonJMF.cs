using UnityEngine;
//*****    *****//
// Singleton simples
// Uma �nica c�pia persiste entre cenas
// Pode ser usado para Gameobjects com MonoBehaviour simples
// Para scrips com corotinas pode ser necess�ria uma implementa��o mais complexa
// Neste Template o GameObject persistente tem um AudioSource (para m�sica de fundo)
// E tem tamb�m array de AudioClip, com algumas m�sicas para trocar no AudioSource
// O Fade Manager e o Load Scene Manager foram colocado como Child para completar o FadeOut na Scene Principal
// Se quiser usar eles na Scene Principal, ou em outras Scenes, basta acrescentar eles como atributos
//*****     *****//
[RequireComponent(typeof(AudioSource))] //AudioSource obrigat�rio para o GameObject
public class ManagerSomSingletonJMF : MonoBehaviour
{
    [SerializeField] private AudioSource MusicPlayer = default; //Audio Sorce atrelado ao GameObject
    [SerializeField] private AudioClip[] Musicas; //Lista de m�sicas para ser usada depois
    public static ManagerSomSingletonJMF staticSingleton { get; private set; } //S� pode haver um!
    void Awake()
    {
        if (staticSingleton == null)//n�o tem ningu�m ainda
        {
            staticSingleton = this; //Eu sou o highlander!
            DontDestroyOnLoad(this.gameObject);//E vivo para sempre!
        }
        else if (staticSingleton != this)//N�o! J� tem outro :-(
        {
            Destroy(this.gameObject); //S� pode haver um e n�o sou eu :-( bye bye!
        }
    }
    void Start()
    {
        if (MusicPlayer == null) //N�o atribuiu o AudioSource
        {
            MusicPlayer = GetComponent<AudioSource>(); //Pega o Audiosorce
        }
    }
    public AudioSource GetAudioSource()
    {
        if (MusicPlayer == null) //N�o deve acontecer, mas por precau��o
        {
            MusicPlayer = gameObject.AddComponent<AudioSource>();
        }
        return MusicPlayer;
    }
    public AudioClip GetMusic(int i) //Retorna um Clip de m�sica
    {
        if (Musicas.Length > i) //Se tem m�sicas suficientes na lista
        {
            return Musicas[i]; //Retorna m�sica do indice passado
        }
        else //Indice fora da lista
        {
            if (Musicas.Length > 0) //Se tem pelo menos uma m�sica
            {
                return Musicas[Musicas.Length - 1]; //Retorna �ltima m�sica
            }
            else //N�o tem nenhuma m�sica na lista
            {
                //Debug.Log("Esqueceu de colocar m�sicas na lista?");
                return null; //Retorna null
            }
        }
    }
}