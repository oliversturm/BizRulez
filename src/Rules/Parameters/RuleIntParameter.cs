// Copyright (C) 2009-2012 Oliver Sturm <oliver@oliversturm.com> All rights reserved.

using System;
using System.Globalization;

namespace pct.BizRulez.Rules.Parameters {
  public class RuleIntParameter : RuleParameter {
    public RuleIntParameter(string name) : base(name) {}
    public RuleIntParameter(string name, int value) : base(name, value) {}
    public RuleIntParameter(string name, string storageString) : base(name, storageString) {}

    public static string StaticParameterTypeId { get { return "int"; } }
    public override string ParameterTypeId { get { return StaticParameterTypeId; } }

    public override string SyntaxDescription {
      get {
        return "Enter an integer number. The value can be negative.";
      }
    }

    public new int Value {
      get {
        return (int) base.Value;
      }
    }

    public override object ValueFromStorage(string storageString) {
      return int.Parse(storageString, NumberStyles.Integer, CultureInfo.InvariantCulture);
    }

    public override RuleParameter Clone() {
      return new RuleIntParameter(Name, Value);
    }

    public override bool Validate() {
      return typeof(int).IsAssignableFrom(base.Value.GetType());
    }

  }
}
