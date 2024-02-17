using System;

namespace Magicube.Data.Abstractions {
    public class PageSearchModel {
        private int _pageSize = 30;

        public int PageIndex { get; set; }

        public int PageSize {
            get {
                return _pageSize; 
            }
            set {
                _pageSize = Math.Min(value, 100);
            }
        }
    }
}