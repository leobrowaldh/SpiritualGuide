using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp.Helpers;
internal static class AiHelper
{
    internal static float CosineSimilarity(float[] vectorA, float[] vectorB)
    {
        float dot = 0f, magA = 0f, magB = 0f;
        for (int i = 0; i < vectorA.Length; i++)
        {
            dot += vectorA[i] * vectorB[i];
            magA += vectorA[i] * vectorA[i];
            magB += vectorB[i] * vectorB[i];
        }
        return dot / (float)(Math.Sqrt(magA) * Math.Sqrt(magB) + 1e-10);
    }

}
