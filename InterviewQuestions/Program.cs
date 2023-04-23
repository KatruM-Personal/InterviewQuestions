using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Net.Mail;
using System.Threading;

namespace InterviewQuestions
{
    public class Program
    {
        static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddScoped<IOrderService, OrderService>();
                    services.AddScoped<IOrder, Order>(
                    serviceProvider => new Order(serviceProvider.GetRequiredService<IOrderService>(), 100));
                }).Build();
            var service = host.Services.GetRequiredService<IOrder>();
            service.RespondToTick("KMSLTD", (decimal)78);
        }
    }
    public interface IOrder : IPlaced, IErrored
    {
        void RespondToTick(string code, decimal price);
    }

    public interface IOrderService
    {
        void Buy(string code, int quantity, decimal price);

        void Sell(string code, int quantity, decimal price);
    }

    public interface IPlaced
    {
        event PlacedEventHandler Placed;
    }

    public delegate void PlacedEventHandler(PlacedEventArgs e);

    public class PlacedEventArgs
    {
        public PlacedEventArgs(string code, decimal price)
        {
            Code = code;
            Price = price;
        }

        public string Code { get; }

        public decimal Price { get; }
    }

    public interface IErrored
    {
        event ErroredEventHandler Errored;
    }

    public delegate void ErroredEventHandler(ErroredEventArgs e);

    public class ErroredEventArgs : ErrorEventArgs
    {
        public ErroredEventArgs(string code, decimal price, Exception ex) : base(ex)
        {
            Code = code;
            Price = price;
        }

        public string Code { get; }

        public decimal Price { get; }
    }

    public class OrderService : IOrderService
    {
        public void Buy(string code, int quantity, decimal price)
        {
            Console.WriteLine("Buy Item values : " + code + " : " + price);
        }

        public void Sell(string code, int quantity, decimal price)
        {
            Console.WriteLine("Sell Item values: " + code + " : " + price);
        }
    }

    public class Order : IOrder
    {
        private readonly IOrderService orderService;
        private readonly decimal threshold;
        private readonly object lockObject = new object();
        private bool hasPlacedOrder;
        private bool hasErrored;

        public Order(IOrderService orderService, decimal threshold)
        {
            this.orderService = orderService;
            this.threshold = threshold;
        }

        public event PlacedEventHandler Placed;

        public event ErroredEventHandler Errored;

        public void RespondToTick(string code, decimal price)
        {
            if (hasPlacedOrder || hasErrored)
            {
                return;
            }

            try
            {
                if (price < threshold)
                {
                    lock (lockObject)
                    {
                        if (hasPlacedOrder || hasErrored)
                        {
                            return;
                        }

                        orderService.Buy(code, 1, price);
                        hasPlacedOrder = true;
                    }

                    Placed?.Invoke(new PlacedEventArgs(code, price));
                }
            }
            catch (Exception ex)
            {
                hasErrored = true;
                Errored?.Invoke(new ErroredEventArgs(code, price, ex));
            }
        }
    }
}
