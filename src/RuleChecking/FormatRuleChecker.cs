// Copyright (C) 2009-2012 Oliver Sturm <oliver@oliversturm.com> All rights reserved.

using System;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.Globalization;
using pct.BizRulez.Rules.Parameters;
using pct.BizRulez.Rules;
using pct.BizRulez.Results;
using System.Collections.Generic;

namespace pct.BizRulez.RuleChecking {
  [Serializable]
  public class FormatRuleChecker : RuleChecker {
    public override string CheckerId {
      get {
        return "format";
      }
    }

    public override string Description {
      get {
        return @"The format rule checker checks a given string for a match with a regular expression. It needs two parameters, 'FieldName' and 'RegExp'. 
While the RegExp parameter must contain a valid regular expression, the FieldName parameter is used to access a named value via the interface. That value must be of type 'string'.";
      }
    }


    //public override RuleParameterSet GetEmptyParameterSet() {
    //  return new RuleParameterSet(new RuleParameter[] {
    //    new RuleStringParameter("FieldName"),
    //    new RuleStringParameter("RegExp")});
    //}

    public override bool IsCompatibleParameterSet(RuleParameterSet parameterSet) {
      RuleStringParameter param1 = parameterSet["FieldName"] as RuleStringParameter;
      RuleStringParameter param2 = parameterSet["RegExp"] as RuleStringParameter;
      return param1 != null && param2 != null && param1.Value.Length > 0 && param2.Value.Length > 0;
    }

    public override string GetDefaultErrorInfoTemplate() {
      return @"The value '{0}' doesn't match the format expression '{1}'.";
    }

    protected override IEnumerable<Result> CheckInternal(object checkable, Rule rule) {
      string fieldName = ((RuleStringParameter) rule.Parameters["FieldName"]).Value;
      string regExp = ((RuleStringParameter) rule.Parameters["RegExp"]).Value;

      object value = GetValue(checkable, fieldName);
      if (value != null) {
        string strVal = value as string;
        if (strVal == null)
          strVal = value.ToString();

        Match match = Regex.Match(strVal, regExp, RegexOptions.None);
        if (match != null && match.Success) {
          yield return new Result(this, rule, ResultStatus.Pass, FailureReason.None, null);
        }
        else {
          Result result = new Result(this, rule, ResultStatus.Fail, FailureReason.CheckFailed,
            String.Format(CultureInfo.CurrentCulture, rule.ErrorInfoTemplate, strVal, regExp));
          result.Details.Add(new ResultDetail("Match", match));
          yield return result;
        }
      }
      else {
        // I've thought about this - null is not a failure here. If null is a failure,
        // it's not this checker's job to say so.
        yield return new Result(this, rule, ResultStatus.Pass, FailureReason.None, null);
      }
    }

  }
}
