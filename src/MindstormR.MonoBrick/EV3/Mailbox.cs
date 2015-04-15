using System;

namespace MonoBrick.EV3
{
		/// <summary>
		/// Class for EV3's mailbox brick.
		/// </summary>
		public class Mailbox
		{
				private Connection<Command, Reply> connection;
				internal Connection<Command,Reply> Connection{
					get{ return connection;}
					set{ connection = value;}
				}

				/// <summary>
				/// Initializes a new instance of the <see cref="MonoBrick.EV3.Mailbox"/> class.
				/// </summary>
				public Mailbox ()
				{
					
				}
				
				/// <summary>
				/// Send a byte array to the mailbox
				/// </summary>
				/// <param name="mailboxName">Mailbox name to send to.</param>
				/// <param name="data">Data to send.</param>
				/// <param name="reply">If set to <c>true</c> reply from the brick will be send.</param>
				public void Send (string mailboxName, byte[] data, bool reply = false)
				{
					var command = new Command (SystemCommand.WriteMailbox, 100, reply);
					Int16 payloadLength = (Int16)data.Length;
					byte nameLength = (byte)(mailboxName.Length + 1);
					command.Append (nameLength);
					command.Append (mailboxName);
					command.Append(payloadLength);
					command.Append(data);
					connection.Send(command);
					if(reply){
						var brickReply = connection.Receive();
						Error.CheckForError(brickReply,100);
					}
				}
				
				/// <summary>
				/// Send a string message to the mailbox
				/// </summary>
				/// <param name="mailboxName">Mailbox name to send to.</param>
				/// <param name="message">Message to send.</param>
				/// <param name="reply">If set to <c>true</c> reply from brick will be send.</param>
				public void Send (string mailboxName, string message, bool reply = false)
				{
					byte[] data = new byte[message.Length+1];
					int i = 0;
					while(i < message.Length)
					{
	    				data[i] = (byte)message[i];
	    				i++;
					}
					data[i] = 0;
					Send(mailboxName,data,reply);
				}
		}
}

