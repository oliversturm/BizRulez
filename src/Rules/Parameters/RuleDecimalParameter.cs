// Copyright (C) 2009-2012 Oliver Sturm <oliver@oliversturm.com> All rights reserved.

using System;
using System.Globalization;

namespace pct.BizRulez.Rules.Parameters {
  public class RuleDecimalParameter : RuleParameter {
    public RuleDecimalParameter(string name) : base(name) {}
    public RuleDecimalParameter(string name, decimal value) : base(name, value) {}
    public RuleDecimalParameter(string name, string storageString) : base(name, storageString) {}

    public static string StaticParameterTypeId { get { return "decimal"; } }
    public override string ParameterTypeId { get { return StaticParameterTypeId; } }

    public override string SyntaxDescription {
      get {
        return "Enter a floating point number, use the dot (.) as the decimal separator. This type is similar to a BCD value.";
      }
    }

    public new decimal Value {
      get {
        return (decimal) base.Value;
      }
    }

    public override object ValueFromStorage(string storageString) {
      return decimal.Parse(storageString, NumberStyles.Float, CultureInfo.InvariantCulture);
    }

    public override string StorageString {
      get {
        return Value.ToString(CultureInfo.InvariantCulture);
      }
    }

    public override RuleParameter Clone() {
      return new RuleDecimalParameter(Name, Value);
    }

    public override bool Validate() {
      return typeof(decimal).IsAssignableFrom(base.Value.GetType());
    }

  }
}
