using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.CloudFirestore;
using Foundation;
using FNO.Models;
using FNO.Utils;

namespace FNO.iOS
{
    public class StorageService
    {
        private static Firebase.CloudFirestore.Firestore Instance => Firebase.CloudFirestore.Firestore.SharedInstance;
        private static IListenerRegistration _listener;
        public static string DeviceToken;
        public static string DeviceTokenID;

        public static async Task<bool> Initialize()
        {
            if (Const.Offline)
                return true;

            Instance.Settings.TimestampsInSnapshotsEnabled = true;
            //Instance.Settings.PersistenceEnabled = false;
            var result = await Auth.DefaultInstance.SignInWithPasswordAsync(Const.StoreUser, Const.StorePassword);
            return result.User != null;
        }

        public static string SaveNewUser(UserProfile user)
        {
            if (Const.Offline)
                return "Offline";

            var data = CreateSaveData(user);
            data = AddNotifyInfo(false, data);
            var refer = Instance.GetCollection("UserProfile").AddDocument(data, (error) =>
            {
                if (error != null)
                {
                    Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
                    {
                        await App.ShowMessage("データの保存に失敗しました。アプリケーションを終了します。");
                        throw new Exception("Save failed:" + error);
                    });
                }
            });
            MonitorOwnUserProfile(refer);
            if (!string.IsNullOrEmpty(DeviceToken))
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
                {
                    var snapshot = await refer.GetDocumentAsync();
                    await SaveDeveiceTokenIfNeed(snapshot, user);
                });
            }
            return refer.Id;
        }

        private static void MonitorOwnUserProfile(Firebase.CloudFirestore.DocumentReference refer)
        {
            if (_listener != null)
            {
                _listener.Remove();
            }
            _listener = refer.AddSnapshotListener((changedSnapshot, error) =>
            {
                var data = changedSnapshot.Data[new NSString("isNeedNotification")] as NSNumber;
                if (data.BoolValue)
                {
                    var userProfile = LoadSaveData<UserProfile>(changedSnapshot.Data);
                    App.NotifyCloudDataChanged(userProfile);
                }
            });
        }

        public static void SaveUser(UserProfile user, bool isNeedNotification)
        {
            if (Const.Offline)
                return;

            var data = CreateSaveData(user);
            data = AddNotifyInfo(isNeedNotification, data);
            var margeTargets = new NSMutableArray();
            foreach (var property in user.GetType().GetRuntimeProperties())
            {
                var storageTarget = property.GetCustomAttribute<StorageTarget>();
                if (storageTarget == null)
                    continue;
                if (storageTarget.CanMarge)
                {
                    margeTargets.Add(new NSString(property.Name));
                }
            }
            Instance.GetDocument("UserProfile/" + user.Id).SetData(data, true);
        }

        private static NSDictionary<NSString, NSObject> AddNotifyInfo(bool isNeedNotification, NSDictionary<NSString, NSObject> data)
        {
            var tmp = new NSMutableDictionary<NSString, NSObject>(data.Keys, data.Values);
            tmp.SetValueForKey(new NSNumber(isNeedNotification), new NSString("isNeedNotification"));
            if (!isNeedNotification && !string.IsNullOrEmpty(DeviceTokenID))
            {
                tmp.SetValueForKey(new NSString(DeviceTokenID), new NSString("Token"));
            }
            data = new NSDictionary<NSString, NSObject>(tmp.Keys, tmp.Values);
            return data;
        }

        public static async Task<UserProfile> GetUserAsync(string id)
        {
            if (Const.Offline)
                return ViewModels.MainViewModel.OtherProfiles[MyRandom.GetRandom(ViewModels.MainViewModel.OtherProfiles.Count)];

            var docRef = Instance.GetDocument("UserProfile/" + id);
            var snapshot = await docRef.GetDocumentAsync();
            if (!snapshot.Exists)
            {
                await App.ShowMessage("ユーザーデータがありません。一定期間ログインがなかった場合は消去された可能性があります。");
                return null;
            }
            var userProfile = LoadSaveData<UserProfile>(snapshot.Data);
            userProfile.Id = id;
            MonitorOwnUserProfile(docRef);

            await SaveDeveiceTokenIfNeed(snapshot, userProfile);

            return userProfile;
        }

        private static async Task SaveDeveiceTokenIfNeed(DocumentSnapshot snapshot, UserProfile userProfile)
        {
            if (DeviceToken != null)
            {
                var tmp = new NSMutableDictionary<NSString, NSObject>();
                tmp.SetValueForKey(new NSString(DeviceToken), new NSString("Token"));
                var data = new NSDictionary<NSString, NSObject>(tmp.Keys, tmp.Values);

                DeviceTokenID = snapshot.Data[new NSString("Token")] as NSString;
                if (!string.IsNullOrEmpty(DeviceTokenID))
                {
                    var docRef2 = Instance.GetDocument("DeviceToken/" + DeviceTokenID);
                    var snapshot2 = await docRef2.GetDocumentAsync();
                    if (snapshot.Exists)
                    {
                        Instance.GetDocument("DeviceToken/" + DeviceTokenID).SetData(data, true);
                    }
                    else
                    {
                        var refer = Instance.GetCollection("DeviceToken").AddDocument(data);
                        DeviceTokenID = refer.Id;
                        SaveUser(userProfile, false);
                    }
                }
                else
                {
                    var refer = Instance.GetCollection("DeviceToken").AddDocument(data);
                    DeviceTokenID = refer.Id;
                    SaveUser(userProfile, false);
                }
                DeviceToken = null;
            }
        }

        public static async Task<UserProfile> GetUserRandomAsync()
        {
            if (Const.Offline)
                return ViewModels.MainViewModel.OtherProfiles[MyRandom.GetRandom(ViewModels.MainViewModel.OtherProfiles.Count)];

            var snapshots = await Instance.GetCollection("UserProfile").GetDocumentsAsync();
            var snapshot = snapshots.Documents[MyRandom.GetRandom((int)snapshots.Count)];
            if (!snapshot.Exists)
            {
                return await GetUserRandomAsync();
            }
            var userProfile = LoadSaveData<UserProfile>(snapshot.Data);
            userProfile.Id = snapshot.Id;
            return userProfile;
        }

        private static NSDictionary<NSString, NSObject> CreateSaveData(object model)
        {
            var result = new NSMutableDictionary<NSString, NSObject>();
            foreach (var property in model.GetType().GetRuntimeProperties())
            {
                if (property.GetCustomAttribute<StorageTarget>() == null)
                    continue;

                var name = property.Name;
                var value = property.GetValue(model);
                if (value != null)
                {
                    result.SetValueForKey(ConvertValue(value), new NSString(name));
                }
            }
            return new NSDictionary<NSString, NSObject>(result.Keys, result.Values);
        }

        private static NSObject ConvertValue(object value)
        {
            if (value != null && !(value is string) && value is IEnumerable)
            {
                var result = new List<object>();
                IEnumerable nestedlist = (IEnumerable)value;
                foreach (var nestedchild in nestedlist)
                {
                    result.Add(ConvertValue(nestedchild));
                }
                return NSArray.FromObjects(result.ToArray());
            }
            else if (value is Name)
            {
                return CreateSaveData(value);
            }
            else
            {
                return NSObject.FromObject(value);
            }
        }

        private static T LoadSaveData<T>(NSDictionary<NSString, NSObject> data)
        {
            var userProfile = Activator.CreateInstance<T>();

            foreach (var property in userProfile.GetType().GetRuntimeProperties())
            {
                if (property.GetCustomAttribute<StorageTarget>() == null)
                    continue;

                var name = new NSString(property.Name);
                if (data.ContainsKey(name))
                {
                    property.SetValue(userProfile, ConvertValue(data[name], property.PropertyType));
                }
            }

            return userProfile;
        }

        private static object ConvertValue(NSObject value, Type type)
        {
            if (value != null && value is NSArray)
            {
                if (type.GenericTypeArguments.Length > 1)
                {
                    throw new Exception("Not Support Type:" + type);
                }
                IList result = null;
                if (type.GenericTypeArguments[0].Equals(typeof(Name)))
                {
                    result = new List<Name>();
                }
                else if (type.GenericTypeArguments[0].Equals(typeof(string)))
                {
                    result = new List<string>();
                }
                else if (type.GenericTypeArguments[0].Equals(typeof(int)))
                {
                    result = new List<int>();
                }
                else
                {
                    throw new Exception("Not Support Type:" + type);
                }
                NSArray nestedlist = (NSArray)value;
                for (uint i = 0; i < nestedlist.Count; i++)
                {
                    result.Add(ConvertValue(nestedlist.GetItem<NSObject>((System.nuint)i), type.GenericTypeArguments[0]));
                }
                return result;
            }
            else if (type.Equals(typeof(Name)))
            {
                var value2 = value as NSMutableDictionary;
                var tmp = new NSMutableDictionary<NSString, NSObject>();
                for (uint i = 0; i < value2.Count; i++)
                {
                    tmp.SetValueForKey(value2.Values[i], value2.Keys[i] as NSString);
                }
                return LoadSaveData<Name>(new NSDictionary<NSString, NSObject>(tmp.Keys, tmp.Values));
            }
            else if (type.Equals(typeof(int)))
            {
                return ((NSNumber)value).Int32Value;
            }
            else if (type.Equals(typeof(bool)))
            {
                return ((NSNumber)value).BoolValue;
            }
            else if (type.Equals(typeof(string)))
            {
                return ((NSString)value).ToString();
            }
            else
            {
                return null;
            }
        }

    }
}
