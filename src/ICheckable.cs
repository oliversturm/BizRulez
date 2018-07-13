// Copyright (C) 2009-2012 Oliver Sturm <oliver@oliversturm.com> All rights reserved.

using System;

namespace pct.BizRulez {
  /// <summary>
  /// This interface can be implemented by types which want
  /// to be checked by the BizRulezChecker. The system
  /// can work with types that don't implement this interface.
  /// In these cases, the value typeof(MyType).Fullname is 
  /// used for ObjectTypeId, and an automatic system accesses
  /// property values directly instead of using ValueByName.
  /// The main purpose of this interface to allow for other
  /// types to be checked against the same rules, such as when
  /// using form validation.
  /// </summary>
  public interface ICheckable {
    /// <summary>
    /// The property ObjectTypeId must return a string value for the object type
    /// that's unique in the application's context at least. The 
    /// BizRulezChecker uses this information to find the relevant
    /// rules in its pool.
    /// </summary>
    string ObjectTypeId { get; }

    /// <summary>
    /// Get a value by name. Usually this will be used to return "field values"
    /// which may map to properties of the implementing object, but of course
    /// the name of the value may be interpreted differently by the
    /// implementation. Some rule checkers may expect the value to be of a 
    /// specific type.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    object ValueByName(string name);
  }
}
