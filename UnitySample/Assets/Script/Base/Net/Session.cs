using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;

namespace Net
{
	/*
	public interface Task
	{
		int Execute();
	}

	public class EventTask<T> : Task where T : EventArgs
	{
		public event EventHandler<T> Handle;
		
		public T args;
		public object sender;
		
		public int Execute()
		{
			try
			{
				Handle(sender, args);
			}
			finally
			{			
			}
			return 0;
		}
	}

	public class TaskArgs<T> : EventArgs
	{
		public TaskArgs(T v)
		{
			param = v;
		}
		
		public TaskArgs()
		{
		}
		
		public T param;
	}
	
	public class TaskScheduler
	{
		class SyncObject
		{
			public string Name { get; set; }
		}
		public delegate void EventError(Exception e);
		
		public EventError OnError;
		public TaskScheduler()
		{
			OnError = null;
		}

		public void Exec()
		{
			lock (_syncObj)
			{
				for (int i = 0; i < 10; i++)
				{
					if (_task_queue.Count == 0)
						break;
					Task t = _task_queue.Dequeue();
					try
					{
						t.Execute();
					}
					catch(Exception e)
					{
						if (OnError != null)
							OnError(e);
					}
				}
			}
		}

		public void CommitTask(Task t)
		{
			lock (_syncObj)
			{
				_task_queue.Enqueue(t);
			}
		}
		
		private Queue<Task> _task_queue = new Queue<Task>();
		SyncObject _syncObj = new SyncObject();
	}

	public delegate void EventVerify(object sender, UInt32 stat);
	public delegate void EventText(object sender, string data);
	public delegate void EventMessage(uint msgid, byte[] data);
	public delegate void EventResponse(uint msgid, byte[] data, bool timeout);
	public delegate void EventTranslate();
	public delegate void EventErrorMessage(uint msgid, uint errid, string reason);

	public class Session : IDisposable
	{
		public event EventHandler<TaskArgs<int>> OnOpen;
		public event EventHandler<TaskArgs<int>> OnClose;

		public event EventVerify OnError;
		public event EventText OnText;
		public event EventTranslate OnTranslate;
		public event EventErrorMessage OnErrorMessage;

		public Session()
		{
			_hdr_sz = Marshal.SizeOf(typeof(pkg_hdr_t));
			_hdr_ptr = Marshal.AllocHGlobal(_hdr_sz);
			_hdrex_sz = Marshal.SizeOf(typeof(pkg_hdrex_t));
			_hdrex_ptr = Marshal.AllocHGlobal(_hdrex_sz);
			_id = _MaxSessionID++;
			_SessionDic.Add(_id, this);
		}

		~Session()
		{
			Marshal.FreeHGlobal(_hdr_ptr);
			Marshal.FreeHGlobal(_hdrex_ptr);
		}

		public bool IsConnected
		{
			get { return _sock != null && _sock.Connected; }
		}
	}
	*/

}