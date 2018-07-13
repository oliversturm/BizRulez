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
  public class NonEmptyStringRuleChecker : RuleChecker {
    public override string CheckerId {
      get {
        return "nonemptystring";
      }
    }

    public override string Description {
      get {
        return @"The non empty string rule checker checks 
		if the value returned from a given string field does not 
		is not empty. The 'FieldName' parameter is used to determine 
		the name of the field to be queried.";
      }
    }

    //public override RuleParameterSet GetEmptyParameterSet( ) {
    //  return new RuleParameterSet(new RuleParameter[] {
		  //                                                      new RuleStringParameter("FieldName")});
    //}

    public override bool IsCompatibleParameterSet(RuleParameterSet parameterSet) {
      RuleStringParameter param1 = parameterSet["FieldName"] as RuleStringParameter;
      return param1 != null && param1.Value.Length > 0;
    }

    public override string GetDefaultErrorInfoTemplate( ) {
      return @"The value of the field is empty.";
    }

    protected override IEnumerable<Result> CheckInternal(object checkable, Rule rule) {
      string fieldName = ((RuleStringParameter) rule.Parameters["FieldName"]).Value;

      object value = GetValue(checkable, fieldName);
      string s = value as string;
      if (s == null) {
        yield return new Result(this, rule, ResultStatus.Fail, FailureReason.CheckNotPerformed,
          String.Format(CultureInfo.CurrentCulture, "The field {0} is not a string field.", fieldName));
        yield break;
      }

      if (s != String.Empty)
        yield return new Result(this, rule, ResultStatus.Pass, FailureReason.None, null);
      else {
        Result result = new Result(this, rule, ResultStatus.Fail, FailureReason.CheckFailed,
          rule.ErrorInfoTemplate);
        yield return result;
      }

    }
  }
}
