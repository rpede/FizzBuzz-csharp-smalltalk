using System;
using System.Collections.Generic;
using System.Linq;

namespace Smalltalk
{
    public abstract class Bool
    {
        private static readonly Dictionary<bool, Bool> conversion
            = new Dictionary<bool, Bool>
        {
            { true, True._ },
            { false, False._ },
        };

        public static implicit operator Bool(bool value)
        {
            return conversion[value];
        }

        public static implicit operator bool(Bool value)
        {
            return conversion.insideOut()[value];
        }

        public abstract Bool ifTrue(Action @do);

        public abstract Bool ifFalse(Action @do);

        public abstract Bool not();
    }

    public sealed class True : Bool
    {
        public static readonly True _ = new True();

        private True()
        {
        }

        public override Bool ifTrue(Action @do)
        {
            @do.Invoke();
            return this;
        }

        public override Bool ifFalse(Action @do)
        {
            return this;
        }

        public override Bool not()
        {
            return False._;
        }
    }

    public sealed class False : Bool
    {
        public static readonly False _ = new False();

        private False()
        {
        }

        public override Bool ifTrue(Action @do)
        {
            return this;
        }

        public override Bool ifFalse(Action @do)
        {
            @do.Invoke();
            return this;
        }

        public override Bool not()
        {
            return True._;
        }
    }

    public class BoolClosure
    {
        private readonly Func<Bool> inner;

        private BoolClosure(Func<Bool> expression)
        {
            this.inner = expression;
        }

        public static implicit operator BoolClosure(Func<Bool> expression)
        {
            return new BoolClosure(expression);
        }

        public static implicit operator Func<Bool>(BoolClosure expression)
        {
            return expression.inner;
        }

        public Bool invoke() {
            return inner.Invoke();
        }

        public void whileTrue(Action @do)
        {
            (inner())
                .ifTrue(() =>
                {
                    @do();
                    whileTrue(@do);
                });
        }

        public void whileFalse(Action @do)
        {
            BoolClosure not = new BoolClosure(() => inner().not());
            not.whileTrue(@do);
        }
    }

    public class Number
    {
        private readonly double inner;

        private Number(double value)
        {
            this.inner = value;
        }

        public static implicit operator double(Number value)
        {
            return value.inner;
        }

        public static implicit operator Number(double value)
        {
            return new Number(value);
        }

        public NumberRange timesRepeat()
        {
            return 0.to(this);
        }

        public NumberRange to(Number value)
        {
            return new NumberRange(@from: this, to: value);
        }

//        // Or more like Smalltalk
//        public void to(Number value, Action<Number> @do) {
//            (this < value)
//                .ifTrue(() =>
//                {
//                    @do(value);
//                    Number from = this + 1;
//                    from.to(value, @do: @do);
//                });
//        }

        public void printNl()
        {
            Console.WriteLine(this);
        }
    }

    public class NumberRange
    {
        public readonly Number to;
        public readonly Number @from;

        public NumberRange(Number @from, Number to)
        {
            this.@from = @from;
            this.to = to;
        }

        public void @do(Action<Number> block)
        {
            (@from < to)
                .ifTrue(() =>
                {
                    block.Invoke(@from);
                    (@from + 1).to(to).@do(block);
                });
        }

//        // Implementation using mutable state
//        public void @do(Action<Number> block) {
//            var count = @from;
//            Func<Bool> notFinished = () => count < to;
//            notFinished.whileTrue(@do: () => block(count++));
//        }
    }

    public class Str
    {
        private readonly string inner;

        private Str(string value)
        {
            this.inner = value;
        }

        public static implicit operator string(Str value)
        {
            return value.inner;
        }

        public static implicit operator Str(string value)
        {
            return new Str(value);
        }

        public void printNl()
        {
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

        public static void whileTrue(this Func<bool> self, Action @do)
        {
            Func<Bool> expression = () => self();
            expression.whileTrue(@do);
        }

        public static void whileFalse(this Func<bool> self, Action @do)
        {
            Func<Bool> expression = () => self();
            expression.whileFalse(@do);
        }

        public static void whileTrue(this Func<Bool> self, Action @do)
        {
            BoolClosure expression = self;
            expression.whileTrue(@do);
        }

        public static void whileFalse(this Func<Bool> self, Action @do)
        {
            BoolClosure expression = self;
            expression.whileFalse(@do);
        }

        public static NumberRange timesRepeat(this int self)
        {
            Number number = self;
            return number.timesRepeat();
        }

        public static NumberRange to(this int self, int value)
        {
            Number @from = self;
            return @from.to(value);
        }

        public static NumberRange to(this int self, Number value)
        {
            return self.to(value);
        }

        public static NumberRange timesRepeat(this double self)
        {
            Number number = self;
            return number.timesRepeat();
        }

        public static NumberRange to(this double self, double value)
        {
            Number @from = self;
            return @from.to(value);
        }

        public static void printNl(this string self)
        {
            Str str = self;
            str.printNl();
        }

        public static void printNl(this double self)
        {
            Number number = self;
            number.printNl();
        }

        public static IDictionary<V,K> insideOut<K,V>(
            this IDictionary<K,V> self
        )
        {
            return self.AsQueryable().ToDictionary(x => x.Value, x => x.Key);
        }
    }
}
