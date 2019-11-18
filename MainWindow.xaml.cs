using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProdConsApp
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		List<string> lst = new List<string>();
		List<string> producersLst= new List<string>();
		List<string> consumersLst = new List<string>();
		List<Producer> producersThreads = new List<Producer>();
		List<Consumer> consumersThreads = new List<Consumer>();
		int indexProd = 0, indexProducer = 1, indexConsumer = 1;
		myBuffer myBuf;

		public MainWindow()
		{
			InitializeComponent();
			myBuf = new myBuffer(this);
			new Thread(() => {
				Thread.CurrentThread.IsBackground = true;
				while (true)
				{
					ProducersLB.Dispatcher.BeginInvoke((Action)(() =>
					{
						ProducersLB.ItemsSource = null;
						ProducersLB.ItemsSource = producersLst;
					}));
					Thread.Sleep(500);
				}
			}).Start();
			new Thread(() => {
				Thread.CurrentThread.IsBackground = true;
				while (true)
				{
					ConsumersLB.Dispatcher.BeginInvoke((Action)(() =>
					{
						ConsumersLB.ItemsSource = null;
						ConsumersLB.ItemsSource = consumersLst;
					}));
					Thread.Sleep(500);
				}
			}).Start();
			
		}

		// В этом классе содержится общий ресурс в виде переменной myBuf,
		// а так же мьютекс mtx
		class myBuffer
		{
			public static int Count;
			public static MainWindow mw;
			public myBuffer(MainWindow t)
			{
				mw = t;
			}
			public void insertProduct(string s, List<string> lst)
			{
				lst.Add(s);
				mw.BufferLB.Dispatcher.BeginInvoke((Action)(() =>
				{
					mw.BufferLB.ItemsSource = null;
					mw.BufferLB.ItemsSource = lst;
				}));
			}
			public void removeProduct(List<string> lst)
			{
				lst.RemoveAt(0);
				mw.BufferLB.Dispatcher.BeginInvoke((Action)(() =>
				{
					mw.BufferLB.ItemsSource = null;
					mw.BufferLB.ItemsSource = lst;
				}));
			}
			public static Mutex mtx = new Mutex();
		}
		
		// В этом классе Count инкрементируется
		class Producer
		{
			int executionTime, uniqueId, stateThread;
			public Thread Thrd;
			public static MainWindow mw;

			public void setExecTime(int n)
			{
				executionTime = n;
			}

			public int getStateThread()
			{
				return stateThread;
			}

			public void setStateThread(int n)
			{
				stateThread = n;
			}

			public Producer(string name, int n, MainWindow t)
			{
				Thrd = new Thread(this.Run);
				stateThread = 1;
				mw = t;
				uniqueId = mw.indexProducer++;
				executionTime = n;
				mw.producersLst.Add("Producer " + Convert.ToString(uniqueId)+"(Speed " + executionTime+")");
				Thrd.Name = name;
				Thrd.Start();
			}

			// Точка входа в поток
			void Run()
			{
				int execTime = executionTime;
				while (true)
				{
					if(stateThread != 1)
					{
						if(stateThread == 0)
						{
							Thread.Sleep(500);
						}
						else if(stateThread == -1)
						{

						}
					}
					else if (myBuffer.Count == 10)
						Thread.Sleep(500);				
					else if(execTime == 0)
					{
						// Получить мьютекс
						myBuffer.mtx.WaitOne();							
						myBuffer.Count++;							
						mw.myBuf.insertProduct("Product" + Convert.ToString(mw.indexProd++)+"(from Producer " + Convert.ToString(uniqueId)+")", mw.lst);
						myBuffer.mtx.ReleaseMutex();
						execTime = executionTime;
					}
					else
					{
						execTime--;
						Thread.Sleep(500);
					}
				}
			}
		}

		class Consumer
		{
			int executionTime, uniqueId, stateThread;
			public Thread Thrd;
			public static MainWindow mw;

			public void setExecTime(int n)
			{
				executionTime = n;
			}

			public int getStateThread()
			{
				return stateThread;
			}

			public void setStateThread(int n)
			{
				stateThread = n;
			}

			public Consumer(int n, MainWindow t)
			{
				Thrd = new Thread(this.Run);
				stateThread = 1;
				mw = t;
				uniqueId = mw.indexConsumer++;
				executionTime = n;
				mw.consumersLst.Add("Consumer " + Convert.ToString(uniqueId) + "(Speed " + executionTime + ")");
				Thrd.Start();
			}

			// Точка входа в поток
			void Run()
			{
				int execTime = executionTime;
				while (true)
				{
					if (stateThread != 1)
					{
						if (stateThread == 0)
						{
							Thread.Sleep(500);
						}
						else if (stateThread == -1)
						{

						}
					}
					else if (myBuffer.Count == 0)
						Thread.Sleep(500);
					else if (execTime == 0)
					{
						// Получить мьютекс
						myBuffer.mtx.WaitOne();
						myBuffer.Count--;
						mw.myBuf.removeProduct(mw.lst);
						myBuffer.mtx.ReleaseMutex();
						execTime = executionTime;
					}
					else
					{
						execTime--;
						Thread.Sleep(500);
					}
				}
			}
		}
		
		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{

		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			string inputTime = textBox1.GetLineText(0);
			int result = -1;
			try
			{
				result = Int32.Parse(inputTime);
			}
			catch (FormatException)
			{
				Console.WriteLine($"Unable to parse '{inputTime}'");
			}
			Producer p = new Producer("newProducr", result, this);
			producersThreads.Add(p);
		}

		private void Button_Click_2(object sender, RoutedEventArgs e)
		{

		}

		private void Button_Click_3(object sender, RoutedEventArgs e)
		{
			string inputTime = textBox4.GetLineText(0);
			string pdcOrCons= textBox3.GetLineText(0);
			int result = -1, result2 =-1;
			try
			{
				result = Int32.Parse(inputTime);
				result2 = Int32.Parse(pdcOrCons.Substring(1));
			}
			catch (FormatException)
			{
				Console.WriteLine($"Unable to parse '{inputTime}'");
			}
			if(pdcOrCons[0] == 'p')
			{	//изменение скорости в списке
				for(int i= producersLst[result2 - 1].Length-1; i>0; i--)
				{
					if(producersLst[result2 - 1][i] == 'd')
					{
						producersLst[result2 - 1] = producersLst[result2 - 1].Substring(0, i + 2) + inputTime + ")";
						break;
					}
				}
				producersThreads[result2 - 1].setExecTime(result);
			}
			else if(pdcOrCons[0] == 'c')
			{//изменение скорости в списке//временное решение
				for (int i = consumersLst[result2 - 1].Length - 1; i > 0; i--)
				{
					if (consumersLst[result2 - 1][i] == 'd')
					{
						consumersLst[result2 - 1] = consumersLst[result2 - 1].Substring(0, i + 2) + inputTime + ")";
						break;
					}
				}
				consumersThreads[result2 - 1].setExecTime(result);
			}
			else
			{
				//data error
			}
			//Producer p = new Producer("newProducr", result, this);
		}

		private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
		{

		}

		private void Button_Click_4(object sender, RoutedEventArgs e)
		{
			string pdcOrCons = textBox3.GetLineText(0);
			int result2 = -1;
			try
			{
				result2 = Int32.Parse(pdcOrCons.Substring(1));
			}
			catch (FormatException)
			{
				Console.WriteLine($"Unable to parse '{pdcOrCons}'");
			}
			if (pdcOrCons[0] == 'p')
			{
				if (producersThreads[result2 - 1].getStateThread() == 1)
					producersThreads[result2 - 1].setStateThread(0);
				else
					producersThreads[result2 - 1].setStateThread(1);
			}
			else if (pdcOrCons[0] == 'c')
			{
				if (consumersThreads[result2 - 1].getStateThread() == 1)
					consumersThreads[result2 - 1].setStateThread(0);
				else
					consumersThreads[result2 - 1].setStateThread(1);
				//consumersThreads[result2 - 1].setExecTime(result);
			}
			else
			{
				//data error
			}
		}

		private void BufferLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			string inputTime = textBox2.GetLineText(0);
			int result = -1;
			try
			{
				result = Int32.Parse(inputTime);
			}
			catch (FormatException)
			{
				Console.WriteLine($"Unable to parse '{inputTime}'");
			}
			Consumer cons = new Consumer(result, this);
			consumersThreads.Add(cons);
		}
	}
}
