using System;

namespace CMaurer.Common
{
    public interface IProgressCallBack
    {
        void Progress(string key, string value);
        void Progress(string key, string value1, string value2);
        void ReportError(string message);
        void DoEvents();
    }
}
