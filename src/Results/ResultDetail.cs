// Copyright (C) 2009-2012 Oliver Sturm <oliver@oliversturm.com> All rights reserved.

using System;

namespace pct.BizRulez.Results {
  public class ResultDetail {
    public ResultDetail(string name, object value) {
      this.Name = name;
      this.Value = value;
    }

    public string Name { get; private set; }

    public object Value { get; private set; }

    public override string ToString() {
      return Name;
    }
  }
}
