using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

using SkiaSharp;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace UnoOnnxSamples.Models
{
    public class MNISTClassifier
    {

        #region Variable(s)

        const int DimBatchSize = 1;
        const int DimNumberOfChannels = 3;
        const int ImageSizeX = 28;
        const int ImageSizeY = 28;
        const string ModelInputName = "dense_6_input";
        const string ModelOutputName = "output";

        byte[] _model;
        byte[] _sampleImage;
        InferenceSession _session;
        Task _initTask;

        public string[] EmbeddedResources { get; } = typeof(MainPage).Assembly.GetManifestResourceNames();

        #endregion

        #region Method(s)

        async Task InitTask()
        {
            var assembly = GetType().Assembly;

            
            // Get model
            var modelResource = EmbeddedResources.First(item => item.EndsWith("mnist_model.onnx"));
            using var modelStream = assembly.GetManifestResourceStream(modelResource);
            using var modelMemoryStream = new MemoryStream();

            modelStream.CopyTo(modelMemoryStream);
            _model = modelMemoryStream.ToArray();

            // Create InferenceSession (runtime representation of the model with optional SessionOptions)
            // This can be reused for multiple inferences to avoid unnecessary allocation/dispose overhead
            // https://onnxruntime.ai/docs/api/csharp-api#inferencesession
            // https://onnxruntime.ai/docs/api/csharp-api#sessionoptions
            _session = new InferenceSession(_model);

            // Get sample image
            var imageResource = EmbeddedResources.First(item => item.EndsWith("handwritten.jpeg"));
            using var sampleImageStream = assembly.GetManifestResourceStream(imageResource);
            using var sampleImageMemoryStream = new MemoryStream();

            sampleImageStream.CopyTo(sampleImageMemoryStream);
            _sampleImage = sampleImageMemoryStream.ToArray();
        }

        Task InitAsync()
        {
            if (_initTask == null || _initTask.IsFaulted)
                _initTask = InitTask();

            return _initTask;
        }

        public async Task<string> GetPredictionAsync(byte[] image)
        {
            await InitAsync().ConfigureAwait(false);
            using var sourceBitmap = SKBitmap.Decode(image);
            var pixels = sourceBitmap.Bytes;
            var bytesPerPixel = sourceBitmap.BytesPerPixel;

            // Rescale.
            if (sourceBitmap.Width != ImageSizeX || sourceBitmap.Height != ImageSizeY)
            {
                float ratio = (float)Math.Min(ImageSizeX, ImageSizeY) / Math.Min(sourceBitmap.Width, sourceBitmap.Height);

                using SKBitmap scaledBitmap = sourceBitmap.Resize(new SKImageInfo(ImageSizeX, ImageSizeY), SKFilterQuality.High);               
                
                pixels = scaledBitmap.Bytes;
                bytesPerPixel = scaledBitmap.BytesPerPixel;
            }
            
            //var bytesPerPixel = sourceBitmap.BytesPerPixel;
            var rowLength = ImageSizeX * bytesPerPixel;
            var channelLength = ImageSizeX * ImageSizeY;
            var channelData = new float[channelLength * 3];
            var channelDataIndex = 0;

            for (int y = 0; y < ImageSizeY; y++)
            {
                var rowOffset = y * rowLength;

                for (int x = 0, columnOffset = 0; x < ImageSizeX; x++, columnOffset += bytesPerPixel)
                {
                    var pixelOffset = rowOffset + columnOffset;

                    var pixelR = pixels[pixelOffset];
                    var pixelG = pixels[pixelOffset + 1];
                    var pixelB = pixels[pixelOffset + 2];

                    var rChannelIndex = channelDataIndex;
                    var gChannelIndex = channelDataIndex + channelLength;
                    var bChannelIndex = channelDataIndex + (channelLength * 2);
                    
                    channelData[rChannelIndex] = (255f - pixelR) / 255f;
                    channelData[gChannelIndex] = (255f - pixelG) / 255f;
                    channelData[bChannelIndex] = (255f - pixelB) / 255f;

                    channelDataIndex++;
                }
            }
            var floatArray = pixels.Select(x => Convert.ToSingle(x/255.0)).ToArray();
            //var matrix = floatArray.ToTensor().Reshape(new[] { ImageSizeX, ImageSizeY });
            var input = new DenseTensor<float>(floatArray, _session.InputMetadata[ModelInputName].Dimensions);
            //var input = new DenseTensor<byte>(pixels, new[] { DimBatchSize, 4, ImageSizeX, ImageSizeY });
            //var input = new DenseTensor<float>(channelData, new[] { DimBatchSize, 1, ImageSizeX, ImageSizeY });
            var inputmetat = _session.InputMetadata;
            using var results = _session.Run(new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor(ModelInputName, input) });

            var output = results.FirstOrDefault(i => i.Name == ModelOutputName);

            if (output == null)
                return "Unknown";
            
            var scores = output.AsTensor<float>().ToList();
            var highestScore = scores.Max();
            var highestScoreIndex = scores.IndexOf(highestScore);

            return highestScoreIndex.ToString();
        }

        public async Task<byte[]> GetSampleImageAsync()
        {
            await InitAsync().ConfigureAwait(false);            
            return _sampleImage;
        }

        #endregion
    }
}
