using System;
using System.Threading;
using System.Threading.Tasks;
using ProblemaProdutorConsumidor.Produtor;

namespace ProblemaProdutorConsumidor.Consumidor
{
    public class ConsumidorBuffer : IDisposable
    {
        private readonly ProdutorBuffer _produtor;
        private readonly CancellationTokenSource _cancellationTokenConsumo;

        public ConsumidorBuffer(ProdutorBuffer produtor)
        {
            _produtor = produtor ?? throw new ArgumentNullException(nameof(produtor));
            _cancellationTokenConsumo = new CancellationTokenSource();
        }

        public Task Consumir()
        {
            return Task.Factory.StartNew(() =>
            {
                while (!_cancellationTokenConsumo.IsCancellationRequested)
                {
                    Thread.Sleep(500);

                    var item = _produtor.FornecerItem();

                    if (item.HasValue)
                        Console.WriteLine($"[Consumidor]: Consumiu item da fila: {item}");
                }
            },
            _cancellationTokenConsumo.Token);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            // Ao descartar o objeto da memória, força a thread de consumo a parar
            _cancellationTokenConsumo.Cancel();
        }
    }
}
