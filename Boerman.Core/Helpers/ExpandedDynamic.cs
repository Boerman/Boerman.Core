using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using Boerman.Core.Extensions;

namespace Boerman.Core.Helpers
{
    // See: http://stackoverflow.com/a/36558165/1720761
    public class ExpandedDynamic : DynamicObject
    {
        private readonly Dictionary<string, object> _customProperties = new Dictionary<string, object>();
        private readonly object _currentObject;

        public ExpandedDynamic()
        {
            _currentObject = new {};
        }

        public dynamic this[string s]
        {
            get {
                TryGetMember(s, out object obj);
                return obj;
            }
            set { TrySetMember(s, value); }
        }

        public ExpandedDynamic(object sealedObject)
        {
            _currentObject = sealedObject;
        }

        private PropertyInfo GetPropertyInfo(string propertyName)
        {
            return _currentObject.GetType().GetProperties().FirstOrDefault
             (propertyInfo => propertyInfo.Name == propertyName);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return TryGetMember(binder.Name, out result);
        }

        private bool TryGetMember(string memberName, out object result)
        {
            var prop = GetPropertyInfo(memberName);
            if (prop != null)
            {
                result = prop.GetValue(_currentObject);
                return true;
            }
            result = _customProperties[memberName];
            return true;
        }

        public override bool TrySetMember(SetMemberBinder setMemberBinder, object value)
        {
            return TrySetMember(setMemberBinder.Name, value);
        }

        private bool TrySetMember(string memberName, object value)
        {
            var prop = GetPropertyInfo(memberName);
            if (prop != null)
            {
                prop.SetValue(_currentObject, value);
                return true;
            }
            if (_customProperties.ContainsKey(memberName))
                _customProperties[memberName] = value;
            else
                _customProperties.Add(memberName, value);
            return true;
        }

        public override string ToString()
        {
            var props = new Dictionary<string, string>();
            
            foreach (var property in _currentObject.GetType().GetProperties())
            {
                props.Add(property.Name, property.GetValue(_currentObject).ToString());
            }

            foreach (var item in _customProperties)
            {
                props.Add(item.Key, item.Value.ToString());
            }

            return string.Join("\r\n", props.OrderBy(q => q.Key).Select(q => $"{q.Key}: {q.Value}"));
        }
    }
}
