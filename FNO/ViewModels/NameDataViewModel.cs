using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PropertyChanged;
using Xamarin.Forms;
using FNO.Utils;

/***
 * 初期版Eternal Glowからコピペ。
 * */
namespace FNO
{
    [AddINotifyPropertyChangedInterface]
    public class NameModelViewModel
    {
        public string Hissatsu { get; private set; }
        public string Color { get; private set; }
        public string Spot { get; private set; }

        private static NameData _data;

        public NameModelViewModel()
        {
            Load();

            Hissatsu = GetData(_data.Hissatu);
            Color = GetData(_data.Color);
            Spot = GetData(_data.Spot);
        }

        public void Load()
        {
            if (_data != null)
                return;

            var assembly = typeof(NameModelViewModel).GetTypeInfo().Assembly;
            var resourceName = "FNO.NameData.json";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                _data = JsonConvert.DeserializeObject<NameData>(reader.ReadToEnd());
            }
        }

        public string GetData(NameDataDetail mode)
        {
            return mode.First[MyRandom.GetRandom(mode.First.Count)]
                    + mode.Second[MyRandom.GetRandom(mode.Second.Count)];
        }

    }

    public class NameData
    {
        public NameDataDetail Hissatu = new NameDataDetail();
        public NameDataDetail Color = new NameDataDetail();
        public NameDataDetail Spot = new NameDataDetail();
    }
    public class NameDataDetail
    {
        public List<string> First = new List<string>();
        public List<string> Second = new List<string>();
    }
}
