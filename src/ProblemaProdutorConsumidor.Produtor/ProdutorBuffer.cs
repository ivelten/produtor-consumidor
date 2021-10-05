using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ProblemaProdutorConsumidor.Produtor
{
    public class ProdutorBuffer : IDisposable
    {
        private readonly List<int> _buffer;
        private readonly int _tamanhoBuffer;
        private readonly object _objetoSemaforo;
        private readonly CancellationTokenSource _cancellationTokenProducao;
        private readonly Random _random;

        public ProdutorBuffer(int tamanhoBuffer, object objetoSemaforo)
        {
            _buffer = new List<int>(tamanhoBuffer);
            _tamanhoBuffer = tamanhoBuffer;
            _random = new Random();

            _objetoSemaforo = objetoSemaforo;
            _cancellationTokenProducao = new CancellationTokenSource();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            // Ao descartar o objeto da memória, força a thread de produção a parar
            _cancellationTokenProducao.Cancel();
        }

        public int? FornecerItem()
        {
            // Tenta retornar um item do buffer, travando o código no semáforo para evitar retornar o mesmo item duas vezes
            lock (_objetoSemaforo)
            {
                if (_buffer.Count > 0)
                {
                    var item = _buffer[0];

                    _buffer.RemoveAt(0);

                    return item;
                }
            }

            return null;
        }

        public Task Produzir()
        {
            return Task.Factory.StartNew(() =>
            {
                while (!_cancellationTokenProducao.IsCancellationRequested)
                {
                    Thread.Sleep(500);

                    var itensDisponiveis = _tamanhoBuffer - _buffer.Count;

                    if (itensDisponiveis > 0)
                    {
                        var qtd = _random.Next(1, itensDisponiveis);

                        // Trava o código no objeto semáforo, para que ninguém altere o buffer enquanto se adicionam itens
                        lock (_objetoSemaforo)
                        {
                            Console.WriteLine($"[ Produtor ]: Produzindo {qtd} itens para o buffer.");

                            // Produz uma quantidade randômica de itens igual ou menor que o espaço livre do buffer
                            for (var i = 1; i <= qtd; i++)
                            {
                                _buffer.Add(_random.Next());
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("[Produtor]: Buffer cheio. Aguardando consumo de itens.");
                    }
                }
            },
            _cancellationTokenProducao.Token);
        }
    }
}
