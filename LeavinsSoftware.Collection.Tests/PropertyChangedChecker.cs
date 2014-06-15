// Copyright (c) 2013, 2014 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace LeavinsSoftware.Collection.Tests
{
    public static class PropertyChangedChecker
    {
        private static PropertyChangedEventHandler _handler;
        private static INotifyPropertyChanged _target;
        private static readonly IDictionary<string, bool> _propertyChangedDictionary;
        private static ICollection<string> _restrictions;

        static PropertyChangedChecker()
        {
            _handler = new PropertyChangedEventHandler(TargetPropertyChanged);
            _propertyChangedDictionary = new Dictionary<string, bool>();
            _restrictions = new List<string>();
        }

        public static void ListenTo(INotifyPropertyChanged target)
        {
            _propertyChangedDictionary.Clear();
            _restrictions.Clear();

            if (_target != null)
            {
                _target.PropertyChanged -= _handler;
            }

            _target = target;
            _target.PropertyChanged += _handler;
        }

        public static void CheckOnly(string propertyName)
        {
            _restrictions.Add(propertyName);
        }

        public static void CheckOnly<TModel, TValue>(Expression<Func<TModel, TValue>> expression)
        {
            MemberExpression body = expression.Body as MemberExpression;
            CheckOnly(body.Member.Name);
        }

        static void TargetPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_restrictions.Count == 0 || _restrictions.Contains(e.PropertyName))
            {
                _propertyChangedDictionary[e.PropertyName] = true;
            }
            else
            {
                string errorMsg = string.Format("Expected: {0} Received: {1}",
                    string.Join(";", _restrictions),
                    e.PropertyName);
                
                throw new ArgumentException(errorMsg,
                                            "e");
            }
        }

        public static bool HasPropertyChanged(string propertyName)
        {
            return _propertyChangedDictionary.ContainsKey(propertyName) &&
                _propertyChangedDictionary[propertyName];
        }
    }
}
