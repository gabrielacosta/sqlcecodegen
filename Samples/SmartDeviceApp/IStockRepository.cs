/*
	This code was generated by SQL Compact Code Generator version 1.2.1.5

	SQL Compact Code Generator was written by Christian Resma Helle (http://sqlcecodegen.codeplex.com)
	and is under the GNU General Public License version 2 (GPLv2)

	Generated: 07/19/2011 17:29:26
*/



namespace SmartDeviceApp.TestDatabaseMultiple
{
	/// <summary>
	/// Represents the Stock repository
	/// </summary>
	public partial interface IStockRepository : IRepository<Stock>
	{
		/// <summary>
		/// Transaction instance created from <see cref="IDataRepository" />
		/// </summary>
		System.Data.SqlServerCe.SqlCeTransaction Transaction { get; set; }

		/// <summary>
		/// Retrieves a collection of items by Id
		/// </summary>
		/// <param name="Id">Id value</param>
		System.Collections.Generic.List<Stock> SelectById(System.Int32? Id);

		/// <summary>
		/// Retrieves the first set of items specified by count by Id
		/// </summary>
		/// <param name="Id">Id value</param>
		/// <param name="count">the number of records to be retrieved</param>
		System.Collections.Generic.List<Stock> SelectById(System.Int32? Id, int count);

		/// <summary>
		/// Retrieves a collection of items by ProductId
		/// </summary>
		/// <param name="ProductId">ProductId value</param>
		System.Collections.Generic.List<Stock> SelectByProductId(System.Int32? ProductId);

		/// <summary>
		/// Retrieves the first set of items specified by count by ProductId
		/// </summary>
		/// <param name="ProductId">ProductId value</param>
		/// <param name="count">the number of records to be retrieved</param>
		System.Collections.Generic.List<Stock> SelectByProductId(System.Int32? ProductId, int count);

		/// <summary>
		/// Retrieves a collection of items by Quantity
		/// </summary>
		/// <param name="Quantity">Quantity value</param>
		System.Collections.Generic.List<Stock> SelectByQuantity(System.Int32? Quantity);

		/// <summary>
		/// Retrieves the first set of items specified by count by Quantity
		/// </summary>
		/// <param name="Quantity">Quantity value</param>
		/// <param name="count">the number of records to be retrieved</param>
		System.Collections.Generic.List<Stock> SelectByQuantity(System.Int32? Quantity, int count);

		/// <summary>
		/// Retrieves a collection of items by LastUpdated
		/// </summary>
		/// <param name="LastUpdated">LastUpdated value</param>
		System.Collections.Generic.List<Stock> SelectByLastUpdated(System.DateTime? LastUpdated);

		/// <summary>
		/// Retrieves the first set of items specified by count by LastUpdated
		/// </summary>
		/// <param name="LastUpdated">LastUpdated value</param>
		/// <param name="count">the number of records to be retrieved</param>
		System.Collections.Generic.List<Stock> SelectByLastUpdated(System.DateTime? LastUpdated, int count);

		/// <summary>
		/// Delete records by Id
		/// </summary>
		/// <param name="Id">Id value</param>
		int DeleteById(System.Int32? Id);

		/// <summary>
		/// Delete records by ProductId
		/// </summary>
		/// <param name="ProductId">ProductId value</param>
		int DeleteByProductId(System.Int32? ProductId);

		/// <summary>
		/// Delete records by Quantity
		/// </summary>
		/// <param name="Quantity">Quantity value</param>
		int DeleteByQuantity(System.Int32? Quantity);

		/// <summary>
		/// Delete records by LastUpdated
		/// </summary>
		/// <param name="LastUpdated">LastUpdated value</param>
		int DeleteByLastUpdated(System.DateTime? LastUpdated);

		/// <summary>
		/// Create new record without specifying a primary key
		/// </summary>
		void Create(System.Int32? ProductId, System.Int32? Quantity, System.DateTime? LastUpdated);

		/// <summary>
		/// Create new record specifying all fields
		/// </summary>
		void Create(System.Int32? Id, System.Int32? ProductId, System.Int32? Quantity, System.DateTime? LastUpdated);
	}
}

