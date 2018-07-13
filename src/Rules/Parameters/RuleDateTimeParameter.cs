// Copyright (C) 2009-2012 Oliver Sturm <oliver@oliversturm.com> All rights reserved.

using System;
using System.Globalization;

namespace pct.BizRulez.Rules.Parameters {
  /// <summary>
  /// This implementation of the datetime parameter converts values
  /// to universal time for storage and back _to the local time
  /// of the executing system_ (!) on loading. If this is not the
  /// intended behaviour, look at the RuleInvariantDateTimeParameter.
  /// The reason we need to do that is that there seems to be no
  /// proper way to parse a string with the time zone information
  /// included... while the format string can create such a string just fine,
  /// its value can't be parsed back into a datetime. Plus: The
  /// DateTime doesn't provide access to the time zone of the stored
  /// value anyway. Plus: In many cases, the time reference is meant
  /// to be relevant to the local system anyway, so conversion to
  /// the local zone would still be needed.
  /// </summary>
  public class RuleDateTimeParameter : RuleInvariantDateTimeParameter {
    public RuleDateTimeParameter(string name) : base(name) {}
    public RuleDateTimeParameter(string name, DateTime value) : base(name, value) {}
    public RuleDateTimeParameter(string name, string storageString) : base(name, storageString) {}

    public static new string StaticParameterTypeId { get { return "DateTime"; } }
    public override string ParameterTypeId { get { return StaticParameterTypeId; } }

    public override string SyntaxDescription {
      get {
        return "Enter a date and time information in the following format or a subset thereof: 'mm/dd/yyyy hh:mm:ss.fffffff'. On the loading system, the value will be converted to the local time zone.";
      }
    }

    public override object ValueFromStorage(string storageString) {
      DateTime val = (DateTime) base.ValueFromStorage(storageString);
      return val.ToLocalTime();
    }

    public override string StorageString {
      get {
        return Value.ToUniversalTime().ToString(
          @"MM/dd/yyyy HH:mm:ss.fffffff", CultureInfo.InvariantCulture);
      }
    }

    public override RuleParameter Clone() {
      return new RuleDateTimeParameter(Name, Value);
    }

  }
}
