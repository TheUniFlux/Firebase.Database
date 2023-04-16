/*
Copyright (c) 2023 Xavier Arpa López Thomas Peter ('Kingdox')

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
namespace Kingdox.UniFlux.Firebase.Database
{
    public sealed partial class Firebase_Database_Module : MonoFlux
    {
        private FirebaseDatabase _database = default;
        private Dictionary<string, Action<object>> _dic_subscriptions = new Dictionary<string, Action<object>>();
        [Flux(FirebaseDatabaseService.Key.Initialize)] private void Initialize()
        {
            _database = FirebaseDatabase.DefaultInstance;
        }
        [Flux(FirebaseDatabaseService.Key.Get)] private DatabaseReference GetDatabaseReference(string path)
        {
            return _database.GetReference(path);
        }
        [Flux(FirebaseDatabaseService.Key.Get)] private async Task<object> Get(string path)
        {
            DatabaseReference dbref = GetDatabaseReference(path);
            DataSnapshot snapshot = await dbref.GetValueAsync();
            return snapshot.Value;
        }
        [Flux(FirebaseDatabaseService.Key.Get)] private async Task<object> Get((string path, object defaultValue) data)
        {
            object valueToSend = await Get(data.path);
            if (valueToSend == null) return data.defaultValue;
            if (!valueToSend.GetType().Equals(data.defaultValue.GetType())){
                Debug.LogWarning($"[Firebase Database Module]: Alerta path => ({data.path}), no son los mismos tipos, recibido: ({valueToSend}) | default: ({data.defaultValue})");
                return data.defaultValue;
            }
            return valueToSend;
        }
        [Flux(FirebaseDatabaseService.Key.Set)] private async Task Set((string path, object value) data)
        {
            DatabaseReference dbref = GetDatabaseReference(data.path);
            await dbref.SetValueAsync(data.value);
        }
        [Flux(FirebaseDatabaseService.Key.Subscribe)] private void Subscribe((string path, bool condition, Action<object> callback) data)
        {
            string[] paths = data.path.Split('/');
            if (data.condition)
            {
                if (!_dic_subscriptions.ContainsKey(data.path))
                {
                    _dic_subscriptions.Add(data.path, default);
                    GetDatabaseReference(data.path).ValueChanged += _HandleValueChanged;
                }
                _dic_subscriptions[data.path] += data.callback;
            }
            else if (_dic_subscriptions.ContainsKey(data.path))
            {
                _dic_subscriptions[data.path] -= data.callback;
                if (_dic_subscriptions[data.path] == null)
                {
                    _dic_subscriptions.Remove(data.path);
                    GetDatabaseReference(data.path).ValueChanged -= _HandleValueChanged;
                }
            }
        }
        private void _HandleValueChanged(object sender, ValueChangedEventArgs args)
        {
            if (args.DatabaseError != null)
            {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }
            string stringLinkPath = args.Snapshot.Reference.ToString();
            string _matcher = "//";
            int index = stringLinkPath.LastIndexOf(_matcher);
            stringLinkPath = stringLinkPath.Remove(0, index + _matcher.Length);
            _dic_subscriptions[stringLinkPath]?.Invoke(args.Snapshot.Value);
        }
    }
}