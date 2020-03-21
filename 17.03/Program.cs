using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace _17._03
{
    class Producer
    {
        
        private int num;
        public static int maxNum = 3;
        private Thread t;
        CommonData _d;
        public static int count = 0;

        public int Num
        {
            get { return num; }
            set { num = Math.Abs(value) % maxNum; }
        }
        public Producer(int num, CommonData d)
        {
            Num = num;
            Start();
            _d=d;
        
        }
        private void Generate()
        {
            Random r = new Random((int)DateTime.Now.Ticks);
            int delay = r.Next(1000, 5000);
            Thread.Sleep(delay);
            int result = r.Next(0, 100);
            Monitor.Enter(_d);
            try
            {
                while (count < 5 || _d.Filled!=Num)
                {
                    Monitor.Wait(_d);
                }
                Monitor.Pulse(_d);
                _d.Set(Num, result);
                
            }
            catch(Exception e)
            {

            }
            finally
            {
                Monitor.Exit(_d);
            }
            Monitor.Enter(_d);
            Console.WriteLine("Производитель нашел число № {0}={1}", Num, result);
            Monitor.Exit(_d);

            Monitor.Enter(_d);
            Monitor.PulseAll(_d);
            Monitor.Exit(_d);
           
            
        }
        public void Start()
        {
            if (t == null || t.IsAlive)
            {
                ThreadStart th = new ThreadStart(Generate);
                t = new Thread(th);
                t.Start();
            }
        }
    }
    class CommonData
    {
        int [] _results;
        public int Filled{ get; private set; }
        public CommonData()
        {
            _results = new int[3];
            Filled = 0;
        }
        public void Set (int index, int value)
        {
            _results[index] = value;
            Filled++;
        }
        public int[] Get()
        {
            Filled = 0;
            return _results;
        }
    }
    class Consumer
    {
        private Thread t;
        CommonData _d;
        public Consumer(CommonData d)
        {
            _d = d;
            Start();
        }
        private void Get()
        {
            int[] result = null;
            Monitor.Enter(_d);
            try
            {
                while (_d.Filled <3)
                {
                    Monitor.Wait(_d);
                    
                }
                Monitor.Pulse(_d);
                result = _d.Get();
                int d = 0;
                for (int i = 0; i < Producer.maxNum; i++)
                {
                    d += result[i];
                }
                Console.WriteLine("Результат потребителя:{0}", d);
            }
            catch(Exception e)
            {

            }
            finally
            {
                Monitor.Exit(_d);
            }
            
           
           

            Monitor.Enter(_d);
            Monitor.PulseAll(_d);
            Monitor.Exit(_d);
           

        }
        public void Start()
        {
            if (t == null || t.IsAlive)
            {
                ThreadStart th = new ThreadStart(Get);
                t = new Thread(th);
                t.Start();
            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            CommonData cd = new CommonData();
            
            while(Producer.count<5)
            {
                for (int i = 0; i < Producer.maxNum; i++)
                {
                    new Producer(i, cd);
                }
                new Consumer(cd);
                Producer.count++;
            }
           
            Console.ReadKey();
        }
    }
}

