using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.ComponentModel;
using System.Collections;

namespace Bleroy.Sample.Dynamic {

    public class TypeDescriptorDynamicWrapper : DynamicObject {
        ICustomTypeDescriptor _descriptor;
        List<PropertyDescriptor> _properties;

        public TypeDescriptorDynamicWrapper(ICustomTypeDescriptor descriptor) {
            _descriptor = descriptor;
            _properties = _descriptor
                .GetProperties()
                .Cast<PropertyDescriptor>()
                .ToList<PropertyDescriptor>();
        }

        public override IEnumerable<string> GetDynamicMemberNames() {
            return _properties.Select(pd => pd.Name);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result) {
            var name = binder.Name;
            var property = _properties.Find(
                p => p.Name.Equals(name,
                    binder.IgnoreCase ?
                    StringComparison.OrdinalIgnoreCase :
                    StringComparison.Ordinal));
            if (property == null) {
                result = null;
                return false;
            }
            result = property.GetValue(_descriptor.GetPropertyOwner(null));
            return true;
        }
    }
}
