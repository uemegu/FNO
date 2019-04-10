using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Threading;
using Newtonsoft.Json;
using PropertyChanged;
using Xamarin.Forms;
using FNO.Utils;

/***
 * Glorious Futureからコピペ。
 * */
namespace Glorious
{
    [AddINotifyPropertyChangedInterface]
    public class PredictViewModel
    {
        public History Word { get; set; } = new History();
        private PredictDataDetail _data;

        public PredictViewModel()
        {
            Load();
            Start();
        }

        public void Load()
        {
            var assembly = typeof(PredictViewModel).GetTypeInfo().Assembly;
            var resourceName = "FNO.PredictData.json";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                _data = JsonConvert.DeserializeObject<PredictData>(reader.ReadToEnd()).Future;
            }
        }

        public void Start()
        {
            Word.Word1 = _data.First[MyRandom.GetRandom(_data.First.Count)];
            Word.Word2 = _data.Second[MyRandom.GetRandom(_data.Second.Count)];
            Word.Word3 = _data.Third[MyRandom.GetRandom(_data.Third.Count)];
            Word.Word4 = _data.Forth[MyRandom.GetRandom(_data.Forth.Count)];
            Word.Word5 = _data.Fifth[MyRandom.GetRandom(_data.Fifth.Count)];
            Word.Word6 = _data.Sixth[MyRandom.GetRandom(_data.Sixth.Count)];
        }

    }

    public class PredictData
    {
        public PredictDataDetail Future = new PredictDataDetail();
    }

    public class PredictDataDetail
    {
        public List<string> First = new List<string>();
        public List<string> Second = new List<string>();
        public List<string> Third = new List<string>();
        public List<string> Forth = new List<string>();
        public List<string> Fifth = new List<string>();
        public List<string> Sixth = new List<string>();
    }

    [AddINotifyPropertyChangedInterface]
    public class History
    {
        public string Word1 { get; set; } = " ";
        public string Word2 { get; set; } = " ";
        public string Word3 { get; set; } = " ";
        public string Word4 { get; set; } = " ";
        public string Word5 { get; set; } = " ";
        public string Word6 { get; set; } = " ";

        public override string ToString()
        {
            return Word1 + Word2 + Word3 + Word4 + Word5 + Word6;
        }
    }
}
