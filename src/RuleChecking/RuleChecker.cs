// Copyright (C) 2009-2012 Oliver Sturm <oliver@oliversturm.com> All rights reserved.

using System;
using pct.BizRulez.Rules;
using pct.BizRulez.Rules.Parameters;
using pct.BizRulez.Results;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;

namespace pct.BizRulez.RuleChecking {
  public abstract class RuleChecker {
    /// <summary>
    /// Return the ID of the rule that must be unique in the context
    /// of the current application at least. This id will be used
    /// by the BizRulezChecker to find the rule checker when
    /// it's referenced by a Rule.
    /// </summary>
    public abstract string CheckerId { get; }

    /// <summary>
    /// Return a description of what a rule checker does.
    /// </summary>
    public abstract string Description { get; }

    ///// <summary>
    ///// Return an empty set of parameters as needed by this rule checker.
    ///// </summary>
    ///// <returns></returns>
    //public abstract RuleParameterSet GetEmptyParameterSet();

    /// <summary>
    /// Find out if the given parameter set can be used with this rule
    /// checker to actually check a rule. All needed parameters must be
    /// available and of the correct type and must contain usable
    /// values.
    /// </summary>
    /// <param name="parameterSet">The parameter set to check.</param>
    /// <returns>true if the parameter set is compatible</returns>
    public abstract bool IsCompatibleParameterSet(RuleParameterSet parameterSet);

    /// <summary>
    /// Return a default error info template with all the placeholders used by this rule checker 
    /// when error info is needed.
    /// </summary>
    /// <returns></returns>
    public abstract string GetDefaultErrorInfoTemplate();

    /// <summary>
    /// Check the given checkable using the given rule. Return a Result.
    /// </summary>
    /// <param name="checkable"></param>
    /// <param name="rule"></param>
    /// <returns></returns>
    public IEnumerable<Result> Check(object checkable, Rule rule) {
      if (!IsCompatibleParameterSet(rule.Parameters)) {
        yield return new Result(this, rule, ResultStatus.Fail, FailureReason.CheckNotPerformed, "Incompatible parameter set found");
      }
      else {
        foreach(var r in CheckInternal(checkable, rule))
          yield return r;
      }
    }

    protected abstract IEnumerable<Result> CheckInternal(object checkable, Rule rule);

    public override string ToString() {
      return CheckerId;
    }

    public static object GetValue(object source, string valueName) {
      var interfaceCheckable = source as ICheckable;
      if (interfaceCheckable != null)
        return interfaceCheckable.ValueByName(valueName);

      var accessor = GetAccessor(source.GetType( ), valueName);
      return accessor(source);
    }

    static Dictionary<Type, Dictionary<string, Func<object, object>>> accessors =
      new Dictionary<Type, Dictionary<string, Func<object, object>>>( );

    static Func<object, object> GetAccessor(Type type, string valueName) {
      Func<object, object> result;
      Dictionary<string, Func<object, object>> typeAccessors;

      // Let's see if we have an accessor already for this type/valueName
      if (accessors.TryGetValue(type, out typeAccessors)) {
        if (typeAccessors.TryGetValue(valueName, out result))
          return result;
      }
      
      // okay, create one and store it for later
      result = CreateAccessor(type, valueName);
      if (typeAccessors == null) {
        typeAccessors = new Dictionary<string, Func<object, object>>( );
        accessors[type] = typeAccessors;
      }
      typeAccessors[valueName] = result;

      return result;
    }

    static Func<object, object> CreateAccessor(Type type, string valueName) {
      var pinfo = type.GetProperty(valueName);
      var param = Expression.Parameter(typeof(object), "o");
      Expression<Func<object, object>> exp =
        Expression.Lambda<Func<object, object>>(
        Expression.Convert(
        Expression.Property(Expression.Convert(param, type), pinfo),
        typeof(object)),
        param);
      return exp.Compile();
    }
  }
}