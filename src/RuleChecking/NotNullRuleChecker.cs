// Copyright (C) 2009-2012 Oliver Sturm <oliver@oliversturm.com> All rights reserved.

using System;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Runtime.Serialization;
using pct.BizRulez.Rules.Parameters;
using pct.BizRulez.Rules;
using pct.BizRulez.Results;
using System.Collections.Generic;

namespace pct.BizRulez.RuleChecking {
  public class NotNullRuleChecker : RuleChecker {
    public override string CheckerId {
      get {
        return "notnull";
      }
    }

    public override string Description {
      get {
        return @"The not null rule checker checks if the value returned from a given field does not equal null or System.DBNull.Value. The 'FieldName' parameter is used to determine the name of the field to be queried and the data type of the returned value is not checked.";
      }
    }

    //public override RuleParameterSet GetEmptyParameterSet() {
    //  return new RuleParameterSet(new RuleParameter[] {
    //                                                    new RuleStringParameter("FieldName")});
    //}

    public override bool IsCompatibleParameterSet(RuleParameterSet parameterSet) {
      RuleStringParameter param1 = parameterSet["FieldName"] as RuleStringParameter;
      return param1 != null && param1.Value.Length > 0;
    }

    public override string GetDefaultErrorInfoTemplate() {
      return @"The value '{0}' equals either null or System.DBNull.Value.";
    }

    protected override IEnumerable<Result> CheckInternal(object checkable, Rule rule) {
      string fieldName = ((RuleStringParameter) rule.Parameters["FieldName"]).Value;

      object value = GetValue(checkable, fieldName);

      if (value != null && value != DBNull.Value) 
        yield return new Result(this, rule, ResultStatus.Pass, FailureReason.None, null);
      else {
        Result result = new Result(this, rule, ResultStatus.Fail, FailureReason.CheckFailed,
          String.Format(CultureInfo.CurrentCulture, rule.ErrorInfoTemplate, value));
        yield return result;
      }

    }
  }

}