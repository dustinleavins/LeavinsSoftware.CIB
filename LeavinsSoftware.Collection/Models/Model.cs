// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace LeavinsSoftware.Collection.Models
{
    public abstract class Model : ModelBase
    {
        [Range(0, long.MaxValue)]
        public long Id
        {
            get
            {
                return id;
            }
            set
            {
                bool isNewModel = !HasId;

                if (id != value)
                {
                    id = value;
                    OnPropertyChanged("Id");

                    if (isNewModel)
                    {
                        OnPropertyChanged("HasId");
                    }
                }
            }
        }

        public bool HasId
        {
            get
            {
                return id != 0;
            }
        }

        private long id;
    }
}
