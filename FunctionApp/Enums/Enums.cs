
namespace FunctionApp.Enums;
public enum EnAuthor 
{ 
    Unknown = 0, 
    Krishnamurti = 1, 
    EckartTolle = 2, 
    AllanWatts = 3, 
    Mooji = 4,
    RamDass = 5,
}

public enum  EnModel
{
    OpenAi3S = 0,
    OpenAi3L = 1,
    HFe5b = 2,
    HFe5l = 3,
    HFminiLM = 4,
    HFdistiluse = 5,
}

//HUGGING FACES MULTILINGUAL MODELS:
//models = {
//    "e5-base": SentenceTransformer("intfloat/multilingual-e5-base"),
//    "e5-large": SentenceTransformer("intfloat/multilingual-e5-large"),
//    "miniLM": SentenceTransformer("sentence-transformers/paraphrase-multilingual-MiniLM-L12-v2"),
//    "distiluse": SentenceTransformer("sentence-transformers/distiluse-base-multilingual-cased-v2")
//}