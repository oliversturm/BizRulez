// Copyright (C) 2009-2012 Oliver Sturm <oliver@oliversturm.com> All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using pct.BizRulez.RuleChecking;
using pct.BizRulez.Rules.Parameters;
using pct.BizRulez.Results;
using System.Globalization;
using System.Linq.Expressions;

namespace pct.BizRulez.RuleChecking {
  public class CollectionContainsRuleChecker : RuleChecker {
    public override string CheckerId {
      get { return "collectioncontains"; }
    }

    public override string Description {
      get { return @"The collection contains rule checker 
finds whether a collection read from a particular field (the CollectionFieldName parameter)
contains a value. The value can either be given literally (ElementValue parameter)
or read from a second field (the ElementFieldName parameter).
A final bool parameter called RuleSatisfiedByNull
specifies whether the collection shall actually be tested
to contain a null value, or whether a null value automatically
satisfies the rule. RuleSatisfiedByNull has a false default value.";
      }
    }

    public override bool IsCompatibleParameterSet(pct.BizRulez.Rules.Parameters.RuleParameterSet parameterSet) {
      RuleStringParameter leftValueFieldName = parameterSet["CollectionFieldName"] as RuleStringParameter;
      if (leftValueFieldName == null || String.IsNullOrEmpty(leftValueFieldName.Value))
        return false;
      var elementValueParameter = parameterSet["ElementValue"];
      var elementFieldParameter = parameterSet["ElementFieldName"] as RuleStringParameter;
      return (elementValueParameter != null) ^
        (elementFieldParameter != null && (!String.IsNullOrEmpty(elementFieldParameter.Value)));
    }

    public override string GetDefaultErrorInfoTemplate( ) {
      return @"The value {0} is not part of the collection.";
    }

    protected override IEnumerable<Result> CheckInternal(object checkable, pct.BizRulez.Rules.Rule rule) {
      var elementValueParam = rule.Parameters["ElementValue"];
      object value = null;
      if (elementValueParam != null) {
        value = elementValueParam.Value;
      }
      else {
        var elementField = ((RuleStringParameter) rule.Parameters["ElementFieldName"]).Value;
        value = GetValue(checkable, elementField);
      }

      string collectionField = ((RuleStringParameter) rule.Parameters["CollectionFieldName"]).Value;
      object collection = GetValue(checkable, collectionField);

      Type valueType = null;
      Type collectionType = collection.GetType( );

      if (value == null) {
        var nullSatisfiesParam = rule.Parameters["RuleSatisfiedByNull"] as RuleBoolParameter;
        if (nullSatisfiesParam != null &&
          nullSatisfiesParam.Value == true) {
          yield return new Result(
            this, rule, ResultStatus.Pass,
            FailureReason.None, String.Empty);
          yield break;
        }
      }
      else {
        valueType = value.GetType( );
        var wantedType = typeof(IEnumerable<>).MakeGenericType(valueType);
        if (!wantedType.IsAssignableFrom(collectionType)) {
          yield return new Result(this, rule, ResultStatus.Fail,
            FailureReason.CheckNotPerformed,
            "Collection doesn't implement " + wantedType);
          yield break;
        }
      }

      if (Contains(collection, value, collectionType, valueType))
        yield return new Result(this, rule, ResultStatus.Pass,
          FailureReason.None, String.Empty);
      else
        yield return new Result(this,rule, ResultStatus.Fail, 
          FailureReason.CheckFailed, 
          string.Format(CultureInfo.CurrentCulture, 
          rule.ErrorInfoTemplate, value));
    }

    bool Contains(object collection, object element, Type collectionType, Type valueType) {
      Func<object, object, bool> contains = null;
      var collParam = Expression.Parameter(typeof(object), "collection");
      var elementParam = Expression.Parameter(typeof(object), "element");
      Expression typedElement = elementParam;

      var typedCollection = Expression.Convert(collParam, collectionType);
      if (valueType == null) {
        var interfaces = collectionType.GetInterfaces( );
        var ienumT = typeof(IEnumerable<>);
        var ienumX = interfaces.First(i => i.IsGenericType &&
            i.GetGenericTypeDefinition( ) == ienumT);
        valueType = ienumX.GetGenericArguments( )[0];
      }

      Type[] containsTypes = new Type[] { valueType };
      typedElement = Expression.Convert(elementParam, valueType);

      var containsCall = 
        Expression.Call(typeof(Enumerable), "Contains",
        containsTypes, typedCollection, typedElement);

      var lambda = Expression.Lambda<Func<object, object, bool>>(
        Expression.Convert(containsCall, typeof(bool)),
        collParam, elementParam);
      contains = lambda.Compile( );
      return contains(collection, element);
    }
  }

}
