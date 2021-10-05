using System;
using System.Collections.Generic;
using System.Threading;
using ProblemaProdutorConsumidor.Consumidor;
using ProblemaProdutorConsumidor.Produtor;

namespace ProblemaProdutorConsumidor.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            int tamanhoBuffer = 5;
            int quantidadeConsumidores = 3;

            if (args.Length > 0 && int.TryParse(args[0], out int tamanhoBufferArg))
                tamanhoBuffer = tamanhoBufferArg;

            if (args.Length > 1 && int.TryParse(args[1], out int quantidadeConsumidoresArg))
                quantidadeConsumidores = quantidadeConsumidoresArg;

            System.Console.WriteLine($"Criando o produtor com buffer de {tamanhoBuffer} itens.");

            var semaforo = new object();
            var produtor = new ProdutorBuffer(tamanhoBuffer, semaforo);

            var consumidores = new List<ConsumidorBuffer>();

            System.Console.WriteLine($"Criando {quantidadeConsumidores} consumidores.");

            for (int i = 1; i <= quantidadeConsumidores; i++)
                consumidores.Add(new ConsumidorBuffer(produtor));

            System.Console.WriteLine("Iniciando a produção e o consumo. Pressione Ctrl + C para terminar o processo.");

            produtor.Produzir();

            foreach (var consumidor in consumidores)
                consumidor.Consumir();

            // Ao pressionar Ctrl + C, termina as threads
            System.Console.CancelKeyPress += delegate
            {
                System.Console.WriteLine("Terminando as threads de consumo...");

                foreach (var consumidor in consumidores)
                    consumidor.Dispose();

                System.Console.WriteLine("Terminando a thread do produtor...");

                produtor.Dispose();

                Environment.Exit(0);
            };

            while (true)
            {
                Thread.Sleep(500);
            }
        }
    }
}
