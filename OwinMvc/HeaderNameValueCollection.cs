using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OwinMvc
{
    internal class HeaderNameValueCollection : System.Collections.Specialized.NameValueCollection
    {
        private IHeaderDictionary _headers;

        public HeaderNameValueCollection(IHeaderDictionary headers)
        {
            this._headers = headers;
        }

        public override void Add(string name, string value)
        {
            this._headers.Add(name, new string[] { value });
        }

        public override string[] GetValues(string name)
        {
            return _headers.GetValues(name).ToArray();
        }

        public override int Count
        {
            get
            {
                return _headers.Count;
            }
        }

        public override System.Collections.IEnumerator GetEnumerator()
        {
            return _headers.Keys.GetEnumerator();
        }


        public override string GetKey(int index)
        {
            throw new NotImplementedException();
        }

        public override string Get(int index)
        {
            throw new NotImplementedException();
        }

        public override string Get(string name)
        {
            return _headers.Get(name);
        }

        public override void Set(string name, string value)
        {
            _headers.Set(name, value);
        }


        public override string[] AllKeys
        {
            get
            {
                return _headers.Keys.ToArray();
            }
        }



        
    }


    //internal class HeaderKeyCollection : System.Collections.Specialized.NameObjectCollectionBase.KeysCollection
    //{
    //    private ICollection<string> _keys;
    //    public HeaderKeyCollection(ICollection<string> keys)
    //    {
    //        this._keys = keys;
    //    }
    //    public override string Get(int index)
    //    {
    //        return base.Get(index);
    //    }
    //}
}
