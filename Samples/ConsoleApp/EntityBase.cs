/*
	This code was generated by SQL Compact Code Generator version 1.2.1.5

	SQL Compact Code Generator was written by Christian Resma Helle (http://sqlcecodegen.codeplex.com)
	and is under the GNU General Public License version 2 (GPLv2)

	Generated: 07/19/2011 18:06:46
*/



namespace ConsoleApp.TestDatabaseMultiple
{
	/// <summary>
	/// Base class for all data access repositories
	/// </summary>
	public static class EntityBase
	{
		private static System.Data.SqlServerCe.SqlCeConnection connectionInstance = null;
		private static readonly object syncLock = new object();

		static EntityBase()
		{
			ConnectionString = "Data Source='TestDatabaseMultiple.sdf'";
		}

		/// <summary>
		/// Gets or sets the global SQL CE Connection String to be used
		/// </summary>
		public static System.String ConnectionString { get; set; }

		/// <summary>
		/// Gets or sets the global SQL CE Connection instance
		/// </summary>
		public static System.Data.SqlServerCe.SqlCeConnection Connection
		{
			get
			{
				lock (syncLock)
				{
					if (connectionInstance == null)
						connectionInstance = new System.Data.SqlServerCe.SqlCeConnection(ConnectionString);
					if (connectionInstance.State != System.Data.ConnectionState.Open)
						connectionInstance.Open();
					return connectionInstance;
				}
			}
			set
			{
				lock (syncLock)
					connectionInstance = value;
			}
		}

		/// <summary>
		/// Create a SqlCeCommand instance using the global SQL CE Conection instance
		/// </summary>
		public static System.Data.SqlServerCe.SqlCeCommand CreateCommand()
		{
			return CreateCommand(null);
		}

		/// <summary>
		/// Create a SqlCeCommand instance using the global SQL CE Conection instance and associate this with a transaction
		/// </summary>
		/// <param name="transaction">SqlCeTransaction to be used for the SqlCeCommand</param>
		public static System.Data.SqlServerCe.SqlCeCommand CreateCommand(System.Data.SqlServerCe.SqlCeTransaction transaction)
		{
			var command = Connection.CreateCommand();
			command.Transaction = transaction;
			return command;
		}
	}
}

