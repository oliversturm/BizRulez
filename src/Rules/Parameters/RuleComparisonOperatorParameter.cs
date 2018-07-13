// Copyright (C) 2009-2012 Oliver Sturm <oliver@oliversturm.com> All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;

namespace pct.BizRulez.Rules.Parameters {
  public class RuleComparisonOperatorParameter : RuleParameter {
    public enum Operator {
      Unknown = 0,
      Smaller = 1,
      SmallerOrEqual = 2,
      Equal = 3,
      NotEqual = 4,
      GreaterOrEqual = 5,
      Greater = 6
    }

    public static string StringForOperator(Operator op) {
      return Enum.GetName(typeof(Operator), op);
    }

    public static Operator OperatorForString (string stringOp) {
      object val = stringOperatorMap[stringOp.Trim()];
      return val != null ? (Operator) val : Operator.Unknown;
    }

    private static Dictionary<string,Operator> stringOperatorMap;
    static RuleComparisonOperatorParameter() {
      stringOperatorMap = new Dictionary<string, Operator>( );
      stringOperatorMap[StringForOperator(Operator.Smaller)] = Operator.Smaller;
      stringOperatorMap[StringForOperator(Operator.SmallerOrEqual)] = Operator.SmallerOrEqual;
      stringOperatorMap[StringForOperator(Operator.Equal)] = Operator.Equal;
      stringOperatorMap[StringForOperator(Operator.NotEqual)] = Operator.NotEqual;
      stringOperatorMap[StringForOperator(Operator.GreaterOrEqual)] = Operator.GreaterOrEqual;
      stringOperatorMap[StringForOperator(Operator.Greater)] = Operator.Greater;
      stringOperatorMap["<"] = Operator.Smaller;
      stringOperatorMap["<="] = Operator.SmallerOrEqual;
      stringOperatorMap["=="] = Operator.Equal;
      stringOperatorMap["!="] = Operator.NotEqual;
      stringOperatorMap[">="] = Operator.GreaterOrEqual;
      stringOperatorMap[">"] = Operator.Greater;
    }

    public RuleComparisonOperatorParameter(string name) : base(name) {}
    public RuleComparisonOperatorParameter(string name, Operator value) : base(name, value) {}
    public RuleComparisonOperatorParameter(string name, string storageString) : base(name, storageString) {}

    public static string StaticParameterTypeId { get { return "ComparisonOperator"; } }
    public override string ParameterTypeId { get { return StaticParameterTypeId; } }

    public override string SyntaxDescription {
      get {
        return "Enter one of the comparison operators (short form in braces): Smaller (<), SmallerOrEqual (<=), Equal (==), NotEqual (!=), GreaterOrEqual (>=), Greater (>).";
      }
    }

    public new Operator Value {
      get {
        return (Operator) base.Value;
      }
    }

    public override object ValueFromStorage(string storageString) {
      return OperatorForString(storageString);
    }

    public override string StorageString {
      get {
        return StringForOperator(Value);
      }
    }

    public override RuleParameter Clone() {
      return new RuleComparisonOperatorParameter(Name, Value);
    }

    public override bool Validate() {
      return typeof(Operator).IsAssignableFrom(base.Value.GetType()) &&
        Value != Operator.Unknown;
    }
  }
}
