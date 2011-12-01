using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleSoccer.Net
{
    public static class AutoList<T>
    {
        private static List<T> _dataList = new List<T>();

        public static List<T> GetAllMembers()
        {
            return _dataList;
        }
    }
}
