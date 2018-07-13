// Copyright (C) 2009-2012 Oliver Sturm <oliver@oliversturm.com> All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using pct.BizRulez.Rules.Parameters;
using pct.BizRulez.Results;
using System.Globalization;
using System.Linq.Expressions;
using System.Collections;
using System.Reflection;

namespace pct.BizRulez.RuleChecking {
  public class UniquenessRuleChecker : RuleChecker {
    public override string CheckerId {
      get { return "uniqueness"; }
    }
    
    public override string Description {
      get {
        return @"The uniqueness rule checker 
		finds whether a collection read from a particular field (the CollectionFieldName parameter)
		contains only values that are unique. The HashSet class is used internally, so the 
equality comparison behavior is according to the mechanisms implemented there.
Alternatively, the combination of the string parameters
GetKeyFunctionClass and GetKeyFunctionName can be given. This is expected to specify a 
function which will be called for each element in the collection and must return 
the key value to be used for comparison.";
      }
    }

    public override bool IsCompatibleParameterSet(pct.BizRulez.Rules.Parameters.RuleParameterSet parameterSet) {
      RuleStringParameter leftValueFieldName = parameterSet["CollectionFieldName"] as RuleStringParameter;
      if (leftValueFieldName == null || String.IsNullOrEmpty(leftValueFieldName.Value))
        return false;
      var keyFunctionClass = parameterSet["GetKeyFunctionClass"] as RuleStringParameter;
      var keyFunctionName = parameterSet["GetKeyFunctionName"] as RuleStringParameter;
      return !((keyFunctionClass != null && (!String.IsNullOrEmpty(keyFunctionClass.Value))) ^
                     (keyFunctionName != null && (!String.IsNullOrEmpty(keyFunctionName.Value))));
    }

    public override string GetDefaultErrorInfoTemplate( ) {
      return @"The value {0} is not unique.";
    }

    protected override IEnumerable<Result> CheckInternal(object checkable, pct.BizRulez.Rules.Rule rule) {
      string collectionField = ((RuleStringParameter) rule.Parameters["CollectionFieldName"]).Value;
      var collection = GetValue(checkable, collectionField) as IEnumerable;

      if (collection != null) {
        var getKeyClassParameter = rule.Parameters["GetKeyFunctionClass"] as RuleStringParameter;
        var getKeyFunctionParameter = rule.Parameters["GetKeyFunctionName"] as RuleStringParameter;

        MethodInfo getKeyFunction = null;

        // simple check here, more elaborate check is being done in IsCompatibleParameterSet
        if (getKeyClassParameter != null) {
          var getKeyClassType = Type.GetType(getKeyClassParameter.Value);
          if (getKeyClassType == null) {
            yield return new Result(this, rule, ResultStatus.Fail, FailureReason.CheckNotPerformed,
              "GetKey class not found");
            yield break;
          }
          getKeyFunction = getKeyClassType.GetMethod(getKeyFunctionParameter.Value);
          if (getKeyFunction == null) {
            yield return new Result(this, rule, ResultStatus.Fail, FailureReason.CheckNotPerformed,
              "GetKey function not found");
            yield break;
          }
        }

        Hashtable hashtable;
        if (getKeyFunction != null)
          hashtable = new Hashtable(new Comparer(getKeyFunction));
        else
          hashtable = new Hashtable( );

        foreach (var item in collection) {
          if (!hashtable.ContainsKey(item))
            hashtable.Add(item, null);
          else {
            yield return new Result(this, rule, ResultStatus.Fail, FailureReason.CheckFailed,
              String.Format(rule.ErrorInfoTemplate, item));
            yield break;
          }
        }
      }

      yield return new Result(this, rule, ResultStatus.Pass, FailureReason.None, String.Empty);
    }


    class Comparer : IEqualityComparer {
      public Comparer(MethodInfo getKeyFunction) {
        this.getKeyFunction = getKeyFunction;
      }

      MethodInfo getKeyFunction;

      #region IEqualityComparer Members

      bool IEqualityComparer.Equals(object x, object y) {
        var xmapped = Map(x);
        var ymapped = Map(y);
        return xmapped.GetHashCode( ) == ymapped.GetHashCode( );
      }

      object Map(object o) {
        return getKeyFunction.Invoke(null, new object[] { o });
      }

      int IEqualityComparer.GetHashCode(object obj) {
        return Map(obj).GetHashCode( );
      }

      #endregion
    }
  }
}
