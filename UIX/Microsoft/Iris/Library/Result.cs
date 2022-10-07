// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Library.Result
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Library
{
    public struct Result
    {
        public static Result Success = new Result(null);
        private string _error;

        public static Result Fail(string error) => new Result(error);

        public static Result Fail(string format, object param) => new Result(string.Format(format, param));

        public static Result Fail(string format, object param1, object param2) => new Result(string.Format(format, param1, param2));

        public static Result Fail(string format, object param1, object param2, object param3) => new Result(string.Format(format, param1, param2, param3));

        public bool Failed => _error != null;

        public string Error => _error;

        public override string ToString() => _error;

        private Result(string error) => _error = error;
    }
}
