// Copyright (C) 2009-2012 Oliver Sturm <oliver@oliversturm.com> All rights reserved.

using System;
using System.Runtime.Serialization;
using pct.BizRulez.Results;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Globalization;

namespace pct.BizRulez.Results {
  [Serializable]
  public class ResultsException : Exception, ISerializable {
    private static string defaultMessage = "A data validation error has occurred.";

    private IEnumerable<Result> results;

    public ResultsException(IEnumerable<Result> results)
      : base(defaultMessage) {
      this.results = results;
    }
    public ResultsException(IEnumerable<Result> results, Exception innerException)
      : base(defaultMessage, innerException) {
      this.results = results;
    }
    protected ResultsException(SerializationInfo info, StreamingContext context)
      : base(info, context) {
      this.results = (IEnumerable<Result>) info.GetValue("Results", typeof(IEnumerable<Result>));
    }
    [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
      base.GetObjectData(info, context);
      info.AddValue("Results", Results);
    }
    public override string Message {
      get {
        return String.Format(CultureInfo.CurrentCulture, "{0}, Results: {1}", base.Message, Results);
      }
    }
    public IEnumerable<Result> Results {
      get {
        return results;
      }
    }
  }
}
