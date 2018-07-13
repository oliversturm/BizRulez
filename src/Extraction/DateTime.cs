// Copyright (C) 2009-2012 Oliver Sturm <oliver@oliversturm.com> All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pct.BizRulez.Extraction {
  static public class DateTime {
    public static System.DateTime ExtractDate(System.DateTime source) {
      return source.Date;
    }
  }
}
