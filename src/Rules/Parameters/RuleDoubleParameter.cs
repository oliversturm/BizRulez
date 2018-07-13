// Copyright (C) 2009-2012 Oliver Sturm <oliver@oliversturm.com> All rights reserved.

using System;
using System.Globalization;

namespace pct.BizRulez.Rules.Parameters {
  public class RuleDoubleParameter : RuleParameter {
    public RuleDoubleParameter(string name) : base(name) {}
    public RuleDoubleParameter(string name, double value) : base(name, value) {}
    public RuleDoubleParameter(string name, string storageString) : base(name, storageString) {}

    public static string StaticParameterTypeId { get { return "double"; } }
    public override string ParameterTypeId { get { return StaticParameterTypeId; } }

    public override string SyntaxDescription {
      get {
        return "Enter a floating point number, use the dot (.) as the decimal separator. This type is NOT a BCD value.";
      }
    }

    public new double Value {
      get {
        return (double) base.Value;
      }
    }

    public override object ValueFromStorage(string storageString) {
      return double.Parse(storageString, NumberStyles.Float, CultureInfo.InvariantCulture);
    }

    public override string StorageString {
      get {
        return Value.ToString(CultureInfo.InvariantCulture);
      }
    }

    public override RuleParameter Clone() {
      return new RuleDoubleParameter(Name, Value);
    }

    public override bool Validate() {
      return typeof(double).IsAssignableFrom(base.Value.GetType());
    }

  }
}
