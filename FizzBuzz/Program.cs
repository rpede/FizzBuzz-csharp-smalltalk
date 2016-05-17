using System;

namespace FizzBuzz
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            0.to(100).@do(i => {
                (i % 3 == 0) .ifTrue(() =>
                    "Fizz".printNl()
                ) .ifFalse(() =>
                    (i % 5 == 0) .ifTrue(() =>
                        "Buzz".printNl()
                    ) .ifFalse(() =>
                        i.printNl()
                    )
                );
            });
        }
    }
}
