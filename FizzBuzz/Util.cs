using System;
using System.Collections.Generic;

namespace FizzBuzz
{
    public abstract class Bool
    {
        private static readonly Dictionary<bool, Bool> mapping
            = new Dictionary<bool, Bool>
            {
                { true, True.instance },
                { false, False.instance },
            };

        public static Bool wrap(bool value)
        {
            return mapping[value];
        }

        public abstract Bool ifTrue(Action do_);

        public abstract Bool ifFalse(Action do_);
    }

    public class True : Bool
    {
        public static readonly True instance = new True();

        private True() { }

        public override Bool ifTrue(Action do_)
        {
            do_.Invoke();
            return this;
        }

        public override Bool ifFalse(Action do_)
        {
            return this;
        }
    }

    public class False : Bool
    {
        public static readonly False instance = new False();

        private False() { }

        public override Bool ifTrue(Action do_)
        {
            return this;
        }

        public override Bool ifFalse(Action do_)
        {
            do_.Invoke();
            return this;
        }
    }

    public static class Extensions
    {
        public static Bool asObj(this bool self)
        {
            return Bool.wrap(self);
        }

        public static Bool ifTrue(this bool self, Action do_)
        {
            return self.asObj().ifTrue(do_);
        }

        public static Bool ifFalse(this bool self, Action do_)
        {
            return self.asObj().ifFalse(do_);
        }

        public static void whileTrue(this Func<bool> self, Action do_)
        {
            (self.Invoke())
                .ifTrue(() =>
                {
                    do_.Invoke();
                    self.whileTrue(do_);
                });
        }

        public static void whileFalse(this Func<bool> self, Action do_)
        {
            Func<bool> not = () => !self.Invoke();
            not.whileTrue(do_);
        }

        public static void timesRepeat(this int self, Action<int> do_)
        {
            0.to(self, do_);
        }

        public static void to(this int self, int value, Action<int> do_)
        {
            (self < value)
                .ifTrue(() =>
                {
                    do_.Invoke(self);
                    (self + 1).to(value, do_);
                });
        }
    }
}

