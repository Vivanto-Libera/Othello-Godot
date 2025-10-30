using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace othello
{
    public class OthelloModel
    {
        private InferenceSession _session;
        public OthelloModel(string modelPath) 
        {
            _session = new InferenceSession(modelPath);
        }
        public (float[] probs, float[] values) Predict(float[,] input1, float[,] input2, float[,] input3) 
        {
            var tensor = new DenseTensor<float>(new[] {1, 3, 8, 8});
            for (int i = 0; i < 8; i++) 
            {
                for(int j = 0; j < 8; j++) 
                {
                    tensor[0, 0, i, j] = input1[i, j];
                    tensor[0, 1, i, j] = input2[i, j];
                    tensor[0, 2, i, j] = input3[i, j];
                }
            }
            var inputs = new List<NamedOnnxValue>
                {
                    NamedOnnxValue.CreateFromTensor("input", tensor)
                };
            using var results = _session.Run(inputs);
            var prob = results.FirstOrDefault(r => r.Name == "prob")?.AsTensor<float>();
            var value = results.FirstOrDefault(r => r.Name == "value")?.AsTensor<float>(); 
            return (prob.ToArray(), value.ToArray());
        }
    }
}
