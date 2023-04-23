# InterviewQuestions
Used .NET 7.0 console application and nunit project to write test cases.
If we want to run multiplethreads we need to use below code instead of Line number 23: service.RespondToTick("KMSLTD", (decimal)78);
Random random = new Random();
            List<Thread> threads = new List<Thread>();
            for (var i = 0; i < 5; i++)
            {
                int price = random.Next(201);
                Task.Factory.StartNew(() =>
                {
                    service.RespondToTick("KMSLTD", price);
                });
            }
The above code generates Random number to send price. As per the logic, once it's buy it won't buy anything after that.
