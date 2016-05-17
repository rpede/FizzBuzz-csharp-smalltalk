using System;
using System.Collections.Generic;
using System.Linq;

namespace FizzBuzz
{
    public abstract class Bool
    {
        private static readonly Dictionary<bool, Bool> conversionObj
            = new Dictionary<bool, Bool>
            {
                { true, True.instance },
                { false, False.instance },
            };
        private static readonly Dictionary<Bool, bool> conversionPrimitive
            = conversionObj.AsQueryable()
                .ToDictionary(k => k.Value, v => v.Key);

        public static implicit operator bool(Bool value) {
            return conversionPrimitive[value];
        }

        public static implicit operator Bool(bool value) {
            return conversionObj[value];
        }

        public abstract Bool ifTrue(Action do_);

        public abstract Bool ifFalse(Action do_);

        public abstract Bool not();
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

        public override Bool not() {
            return False.instance;
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

        public override Bool not() {
            return True.instance;
        }
    }

    public class IntegerRange {
        public readonly int to;
        public readonly int @from;

        private IntegerRange(int @from, int to) {
            this.@from = @from;
            this.to = to;
        }

        public static IntegerRange of(int @from, int to) {
            return new IntegerRange(@from, to);
        }

        public void @do(Action<int> block) {
            (@from < to)
                .ifTrue(() =>
                {
                    block.Invoke(@from);
                    of(@from + 1, to).@do(block);
                });
        }
    }

    public class Integer {
        private readonly int inner;
        private Integer(int value) {
            this.inner = value;
        }

        public static implicit operator int(Integer value) {
            return value.inner;
        }

        public static implicit operator Integer(int value) {
            return new Integer(value);
        }

        public IntegerRange timesRepeat()
        {
            return 0.to(this);
        }

        public IntegerRange to(int value)
        {
            return IntegerRange.of(@from: this, to: value);
        }

        public void printNl() {
            Console.WriteLine(this);
        }
    }

    public class BoolExpression {
        private readonly Func<Bool> inner;
        private BoolExpression(Func<Bool> expression) {
            this.inner = expression;
        }
        public static implicit operator BoolExpression(Func<Bool> expression) {
            return new BoolExpression(expression);
        }
        public static implicit operator Func<Bool>(BoolExpression expression) {
            return expression.inner;
        }

        public void whileTrue(Action @do)
        {
            (inner.Invoke())
                .ifTrue(() =>
                {
                    @do.Invoke();
                    whileTrue(@do);
                });
        }

        public void whileFalse(Action do_)
        {
            not().whileTrue(do_);
        }

        public BoolExpression not() {
            return new BoolExpression(() => inner.Invoke().not());
        }
    }

    public class Str {
        private readonly string inner;

        private Str(string value) {
            this.inner = value;
        }

        public static implicit operator string(Str value) {
            return value.inner;
        }

        public static implicit operator Str(string value) {
            return new Str(value);
        }

        public void printNl() {
            Console.WriteLine(this);
        }
    }

    public static class Extensions
    {
        public static Bool ifTrue(this bool self, Action @do)
        {
            Bool value = self;
            return value.ifTrue(@do);
        }

        public static Bool ifFalse(this bool self, Action @do)
        {
            Bool value = self;
            return value.ifFalse(@do);
        }

        public static void whileTrue(this Func<Bool> self, Action do_)
        {
            BoolExpression expression = self;
            expression.whileTrue(do_);
        }

        public static void whileFalse(this Func<Bool> self, Action do_)
        {
            BoolExpression expression = self;
            expression.whileFalse(do_);
        }

        public static IntegerRange timesRepeat(this int self)
        {
            Integer integer = self;
            return integer.timesRepeat();
        }

        public static IntegerRange to(this int self, int value)
        {
            Integer integer = self;
            return integer.to(value);
        }

        public static void printNl(this string self) {
            Str str = self;
            str.printNl();
        }

        public static void printNl(this int self) {
            Integer integer = self;
            integer.printNl();
        }
    }
}
