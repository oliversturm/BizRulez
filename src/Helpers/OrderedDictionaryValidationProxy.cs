// Copyright (C) 2012 Oliver Sturm <oliver@oliversturm.com>
// All rights reserved.
//

using System;
using pct.BizRulez;
using System.Collections.Specialized;

namespace pct.BizRulez.Helpers {
  public class OrderedDictionaryValidationProxy : ICheckable {
    public OrderedDictionaryValidationProxy(OrderedDictionary values, string objectTypeId) {
      this.values = values;
      this.objectTypeId = objectTypeId;
    }

    private OrderedDictionary values;
    private string objectTypeId;

    string ICheckable.ObjectTypeId {
      get { return objectTypeId; }
    }

    object ICheckable.ValueByName(string name) {
      return values[name];
    }
  }
}
