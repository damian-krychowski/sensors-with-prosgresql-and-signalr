using System.Threading.Channels;
using System.Threading.Tasks;

namespace Sensors
{
    public class SensorDataProducer
    {
        private readonly ChannelWriter<SensorDataDto> _writer;

        public SensorDataProducer(ChannelWriter<SensorDataDto> writer)
        {
            _writer = writer;
        }

        public bool Produce(SensorDataDto sensorData)
        {
            return _writer.TryWrite(sensorData);
        }

        public void Complete()
        {
            _writer.Complete();
        }
    }
}