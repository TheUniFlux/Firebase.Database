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
using Firebase.Database;
namespace Kingdox.UniFlux.Firebase.Database
{
    public static partial class FirebaseDatabaseService // Data
    {
        private static partial class Data
        {

        }
    }
    public static partial class FirebaseDatabaseService // Key
    {
        public static partial class Key
        {
            private const string _FirebaseDatabaseService =  nameof(FirebaseDatabaseService) + ".";
            public const string Initialize   = _FirebaseDatabaseService + nameof(Initialize);
            public const string Set   = _FirebaseDatabaseService + nameof(Set);
            public const string Get   = _FirebaseDatabaseService + nameof(Get);
            public const string Subscribe   = _FirebaseDatabaseService + nameof(Subscribe);
        }
    }
    public static partial class FirebaseDatabaseService // Methods
    {
        public static void Initialize() => Key.Initialize.Dispatch();
        public static DatabaseReference Get_DatabaseReference(string path) => Key.Get.Dispatch<string, DatabaseReference>(path);
        public static Task<object> Get(string path) => Key.Get.Task<string,object>(path);
        public static Task<object> Get((string path, object defaultValue) data) => Key.Get.Task<(string path, object defaultValue), object>(data);
        public static Task Set((string path, object value) data) => Key.Set.Task(data);
        public static void Subscribe((string path, bool condition, Action<object> callback) data) => Key.Subscribe.Dispatch(data);
    }
}