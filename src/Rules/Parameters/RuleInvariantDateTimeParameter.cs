// Copyright (C) 2009-2012 Oliver Sturm <oliver@oliversturm.com> All rights reserved.

using System;
using System.Globalization;

namespace pct.BizRulez.Rules.Parameters {
  public class RuleInvariantDateTimeParameter : RuleParameter {
    public RuleInvariantDateTimeParameter(string name) : base(name) {}
    public RuleInvariantDateTimeParameter(string name, DateTime value) : base(name, value) {}
    public RuleInvariantDateTimeParameter(string name, string storageString) : base(name, storageString) {}

    public static string StaticParameterTypeId { get { return "InvariantDateTime"; } }
    public override string ParameterTypeId { get { return StaticParameterTypeId; } }

    public override string SyntaxDescription {
      get {
        return "Enter a date and time information in the following format or a subset thereof: 'mm/dd/yyyy hh:mm:ss.fffffff'.";
      }
    }

    public new DateTime Value {
      get {
        return (DateTime) base.Value;
      }
    }

    public override bool Validate() {
      return typeof(DateTime).IsAssignableFrom(base.Value.GetType());
    }

    public override object ValueFromStorage(string storageString) {
      return DateTime.Parse(storageString, CultureInfo.InvariantCulture, 
        DateTimeStyles.AllowWhiteSpaces);
    }

    public override string StorageString {
      get {
        return Value.ToString(
          @"MM/dd/yyyy HH:mm:ss.fffffff", CultureInfo.InvariantCulture);
      }
    }

    public override RuleParameter Clone() {
      return new RuleInvariantDateTimeParameter(Name, Value);
    }
  }
}
