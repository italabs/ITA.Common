namespace ITA.Common
{
	/// <summary>
	/// The abstract ITransaction interface
	/// </summary>
	public interface ITransaction
	{
		/// <summary>
		/// The ID of the transaction
		/// </summary>
		string ID { get; }

		/// <summary>
		/// Commit transaction
		/// </summary>
		void Commit();

		/// <summary>
		/// Rollback transaction
		/// </summary>
		void Rollback();
	}
}
