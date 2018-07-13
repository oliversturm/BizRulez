// Copyright (C) 2009-2012 Oliver Sturm <oliver@oliversturm.com> All rights reserved.

using System;

namespace pct.BizRulez.Rules.Parameters {
  /// <summary>
  /// The RuleParameter represents a single parameter to a rule. The rule checker
  /// will return the collection of default parameters that has to be used for it
  /// to work and these will be stored in the rule after the values have
  /// been set in the parameter.
  /// </summary>
  public abstract class RuleParameter {
    /// <summary>
    /// Create a new empty named parameter.
    /// </summary>
    /// <param name="name"></param>
    protected RuleParameter(string name) {
      this.name = name;	
    }

    /// <summary>
    /// Create a new named parameter with the given value.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    protected RuleParameter(string name, object value) {
      this.name = name;
      this.value = value;
    }

    /// <summary>
    /// Create a new named parameter and fill its value from the given storage string.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="storageString"></param>
    protected RuleParameter(string name, string storageString) {
    	this.name = name;

      // ATTENTION: This method is virtual and in derived classes it may be
      // called before the derived class's constructor has been called.
      this.value = ValueFromStorage(storageString);
    }

    /// <summary>
    /// Return the parameter type id which must be unique in the current context.
    /// </summary>
    public abstract string ParameterTypeId { get; }

    /// <summary>
    /// Return a description of the syntax that must be observed when entering values for this parameter in the rule editor.
    /// </summary>
    public abstract string SyntaxDescription { get; }

    private string name;
    /// <summary>
    /// The name of the parameter.
    /// </summary>
    public string Name {
      get {
        return name;
      }
    }

    private object value;
    /// <summary>
    /// The parameter's value. Often derived classes will offer 
    /// an overwritten implementation that return a dedicated data type.
    /// </summary>
    public object Value {
      get {
        return value;
      }
      set {
        this.value = value;
      }
    }

    /// <summary>
    /// Check if this parameter's content is valid. This is only used
    /// in derived classes when the object's value must be checked 
    /// for compatibility with some specific data type.
    /// </summary>
    public virtual bool Validate() {
      return true;
    }

    /// <summary>
    /// Return an object's value converted back from the format of a storage string. <see cref="StorageString"/>
    /// </summary>
    /// <param name="storageString"></param>
    /// <returns></returns>
    public virtual object ValueFromStorage(string storageString) {
      return storageString;
    }

    /// <summary>
    /// Get the object's value as a string that can be stored away
    /// (in an XML file). 
    /// </summary>
    public virtual string StorageString {
      get {
        return value.ToString();
      }
    }

    /// <summary>
    /// Create a clone of the parameter.
    /// </summary>
    /// <returns></returns>
    public abstract RuleParameter Clone();

  }
}
