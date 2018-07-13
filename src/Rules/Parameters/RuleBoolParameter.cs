// Copyright (C) 2009-2012 Oliver Sturm <oliver@oliversturm.com> All rights reserved.

using System;
using System.Globalization;

namespace pct.BizRulez.Rules.Parameters {
  public class RuleBoolParameter : RuleParameter {
    public RuleBoolParameter(string name) : base(name) {}
    public RuleBoolParameter(string name, bool value) : base(name, value) {}
    public RuleBoolParameter(string name, string storageString) : base(name, storageString) {}

    public static string StaticParameterTypeId { get { return "bool"; } }
    public override string ParameterTypeId { get { return StaticParameterTypeId; } }
    
    public override string SyntaxDescription {
      get {
        return "Enter 'true' or 'false'.";
      }
    }

    public new bool Value {
      get {
        return (bool) base.Value;
      }
    }

    public override object ValueFromStorage(string storageString) {
      return Convert.ToBoolean(storageString, CultureInfo.InvariantCulture);
    }

    public override RuleParameter Clone() {
      return new RuleBoolParameter(Name, Value);
    }

    public override bool Validate() {
      return typeof(bool).IsAssignableFrom(base.Value.GetType());
    }
  }
}
