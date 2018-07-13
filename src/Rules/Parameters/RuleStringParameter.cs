// Copyright (C) 2009-2012 Oliver Sturm <oliver@oliversturm.com> All rights reserved.

using System;

namespace pct.BizRulez.Rules.Parameters {
  public class RuleStringParameter : RuleParameter {
    public RuleStringParameter(string name) : base(name, String.Empty) {}
    public RuleStringParameter(string name, string value) : base(name, value) {}

    public static string StaticParameterTypeId { get { return "string"; } }
    public override string ParameterTypeId { get { return StaticParameterTypeId; } }

    public override string SyntaxDescription {
      get {
        return "Enter any string of arbitrary length.";
      }
    }

    public new string Value {
      get {
        return (string) base.Value;
      }
    }

    public override RuleParameter Clone() {
      return new RuleStringParameter(Name, Value);
    }

    public override bool Validate() {
      return typeof(string).IsAssignableFrom(base.Value.GetType());
    }

  }
}
