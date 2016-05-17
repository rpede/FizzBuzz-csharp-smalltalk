using System;

namespace FizzBuzz
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            0.to(100, do_:(i => {
                (i % 3 == 0)
                    .ifTrue(() => 
                        System.Console.WriteLine("Fizz")
                    )
                    .ifFalse(() => 
                        (i % 5 == 0)
                            .ifTrue(() => 
                                System.Console.WriteLine("Buzz")
                            )
                            .ifFalse(() => 
                                System.Console.WriteLine(i)
                            )
                    );
            }));
        }
    }
}
