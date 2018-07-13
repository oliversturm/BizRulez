// Copyright (C) 2009-2012 Oliver Sturm <oliver@oliversturm.com> All rights reserved.

using System;
using System.Globalization;
using pct.BizRulez.Rules.Parameters;
using pct.BizRulez.Rules;
using pct.BizRulez.Results;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;

namespace pct.BizRulez.RuleChecking {
  public class ComparisonRuleChecker : RuleChecker {
    public override string CheckerId {
      get {
        return "comparison";
      }
    }

    public override string Description {
      get {
        return @"The comparison rule checker can compare two values using a variety of 
comparison operators. To select the operator, you need to configure the parameter 
'ComparisonOperator'.
The left hand value for the comparison is always read from the object using the field 
name from the 'LeftValueFieldName' parameter. 
The right side value can either be given literally in the RightValue parameter or read 
from a field, whose name is given in RightValueFieldName. 
The two parameters ExtractionFunctionClass and ExtractionFunctionName can be used
to specify a static function that will be called with the left value and the right 
value, but the latter only if it has been read from a field. The extraction function
can replace the original value for comparison.
The types of the values that are eventually compared must have compatible types.";
      }
    }

    //public override RuleParameterSet GetEmptyParameterSet() {
    //  return new RuleParameterSet(new RuleParameter[] {
    //    new RuleStringParameter("LeftValueFieldName"),
    //    new RuleComparisonOperatorParameter("ComparisonOperator", RuleComparisonOperatorParameter.Operator.Equal),
    //    new RuleDecimalParameter("RightValueNumerical", 0),
    //    new RuleInvariantDateTimeParameter("RightValueDateTime", DateTime.Now),
    //    new RuleStringParameter("RightValueFieldName"),
    //    new RuleBoolParameter("RightValueUseDateTime", false),
    //    new RuleBoolParameter("RightValueUseFieldName", false)
    //    });
    //}

    public override bool IsCompatibleParameterSet(RuleParameterSet parameterSet) {
      RuleStringParameter leftValueFieldName = parameterSet["LeftValueFieldName"] as RuleStringParameter;
      RuleComparisonOperatorParameter comparisonOperator = parameterSet["ComparisonOperator"] as RuleComparisonOperatorParameter;

      if (leftValueFieldName== null || String.IsNullOrEmpty( leftValueFieldName.Value))
        return false;

      if (comparisonOperator == null || comparisonOperator.Value == RuleComparisonOperatorParameter.Operator.Unknown)
        return false;

      var rightValueParameter = parameterSet["RightValue"];
      var rightValueFieldParameter = parameterSet["RightValueFieldName"] as RuleStringParameter;
      var extractionClassParameter = parameterSet["ExtractionFunctionClass"] as RuleStringParameter;
      var extractionFunctionParameter = parameterSet["ExtractionFunctionName"] as RuleStringParameter;

      // everything's fine if we have either a right value OR a right value field name,
      // we can't have both.
      // plus, if we have an extraction function class, we also need a function name
      return ((rightValueParameter != null) ^
              (rightValueFieldParameter != null && (!String.IsNullOrEmpty(rightValueFieldParameter.Value)))) &&
             ((extractionClassParameter == null && extractionFunctionParameter == null) ||
              (extractionClassParameter != null && extractionFunctionParameter != null &&
               !String.IsNullOrEmpty(extractionClassParameter.Value) &&
               !String.IsNullOrEmpty(extractionFunctionParameter.Value)));
    }

    public override string GetDefaultErrorInfoTemplate() {
      return @"The comparison {0} {1} {2} returned false.";
    }

    protected override IEnumerable<Result> CheckInternal(object checkable, Rule rule) {
      object leftValue = GetValue(checkable, (rule.Parameters["LeftValueFieldName"] as RuleStringParameter).Value);
      RuleComparisonOperatorParameter.Operator comparisonOperator = (rule.Parameters["ComparisonOperator"] as RuleComparisonOperatorParameter).Value;

      var extractionClassParameter = rule.Parameters["ExtractionFunctionClass"] as RuleStringParameter;
      var extractionFunctionParameter = rule.Parameters["ExtractionFunctionName"] as RuleStringParameter;

      MethodInfo extractionFunction = null;

      // simple check here, more elaborate check is being done in IsCompatibleParameterSet
      if (extractionClassParameter != null) {
        var extractionClassType = Type.GetType(extractionClassParameter.Value);
        if (extractionClassType == null) {
          yield return new Result(this, rule, ResultStatus.Fail, FailureReason.CheckNotPerformed, "Extraction class not found");
          yield break;
        }
        extractionFunction = extractionClassType.GetMethod(extractionFunctionParameter.Value);
        if (extractionFunction == null) {
          yield return new Result(this, rule, ResultStatus.Fail, FailureReason.CheckNotPerformed, "Extraction function not found");
          yield break;
        }

        // No yield returning in the body of a catch clause. Great.
        // I'll just do it myself then.
        Exception leftValueException = null;
        try {
          leftValue = extractionFunction.Invoke(null, new object[] {leftValue});
        }
        catch (Exception ex) {
          leftValueException = ex;
        }

        if (leftValueException != null) {
          yield return new Result(this, rule, ResultStatus.Fail, FailureReason.CheckNotPerformed, "Extraction function call failed for left value: " + leftValueException.Message);
          yield break;
        }
      }

      object rightValue;
      var rightValueParameter = rule.Parameters["RightValue"];
      if (rightValueParameter != null) {
        rightValue = rightValueParameter.Value;
      }
      else {
        rightValue = GetValue(checkable, (rule.Parameters["RightValueFieldName"] as RuleStringParameter).Value);
        Exception rightValueException = null;
        if (extractionFunction != null) {
          try {
            rightValue = extractionFunction.Invoke(null, new object[] {rightValue});
          }
          catch (Exception ex) {
            rightValueException = ex;
          }
          if (rightValueException != null) {
            yield return new Result(this, rule, ResultStatus.Fail, FailureReason.CheckNotPerformed, "Extraction function call failed for right value: " + rightValueException.Message);
            yield break;
          }
        }
      }

      if (rightValue == null) {
        if (leftValue == null)
          yield return new Result(this, rule, ResultStatus.Pass, FailureReason.None, String.Empty);
        else
          yield return CreateResult(rule, ResultStatus.Fail, FailureReason.CheckFailed, leftValue, comparisonOperator, rightValue);
        yield break;
      }

      // I wouldn't initialize this at all, because the following lines
      // of code ensure that compResult is initialized before it reaches
      // the switch block below, or otherwise the method quits.
      // But the compiler doesn't understand this due to the added 
      // complexity with the exception handling.
      int compResult = 0;
      Exception comparisonException = null;
      try {
        compResult = Comparer.Default.Compare(leftValue, rightValue);
      }
      catch (Exception ex) {
        comparisonException = ex;
      }
      if (comparisonException != null) {
        yield return CreateResult(rule, ResultStatus.Fail, FailureReason.CheckNotPerformed, leftValue,
          comparisonOperator, rightValue, new ResultDetail("ExceptionMessage", comparisonException.Message));
        yield break;
      }

      bool compResultB = false;
      switch (comparisonOperator) {
        case RuleComparisonOperatorParameter.Operator.Smaller:
          compResultB = compResult < 0;
          break;
        case RuleComparisonOperatorParameter.Operator.SmallerOrEqual:
          compResultB = compResult <= 0;
          break;
        case RuleComparisonOperatorParameter.Operator.Equal:
          compResultB = compResult == 0;
          break;
        case RuleComparisonOperatorParameter.Operator.NotEqual:
          compResultB = compResult != 0;
          break;
        case RuleComparisonOperatorParameter.Operator.GreaterOrEqual:
          compResultB = compResult >= 0;
          break;
        case RuleComparisonOperatorParameter.Operator.Greater:
          compResultB = compResult > 0;
          break;
      }

      if (compResultB) 
        yield return new Result(this, rule, ResultStatus.Pass, FailureReason.None, null);
      else {
        yield return CreateResult(rule, ResultStatus.Fail, FailureReason.CheckFailed,
          leftValue, comparisonOperator, rightValue);
      }
    }

    private Result CreateResult(Rule rule, ResultStatus status, FailureReason reason,
      object left, RuleComparisonOperatorParameter.Operator op, object right,
      params ResultDetail[] details) {
      Result result = new Result(this, rule, status, reason,
        String.Format(CultureInfo.CurrentCulture, rule.ErrorInfoTemplate, left,
        RuleComparisonOperatorParameter.StringForOperator(op), right));
      foreach (ResultDetail detail in details)
        result.Details.Add(detail);
      return result;
    }
  }
}
